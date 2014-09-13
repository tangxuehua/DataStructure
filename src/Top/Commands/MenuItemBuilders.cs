using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Windows.Forms;

using Crownwood.Magic.Menus;

using DevComponents.DotNetBar;

using NetFocus.Components.AddIns;
using NetFocus.Components.AddIns.Codons;
using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.Gui.Components;
using NetFocus.DataStructure.Gui.Dialogs;
using NetFocus.DataStructure.Internal.Algorithm;
using NetFocus.DataStructure.TextEditor;
using NetFocus.DataStructure.Internal.ExternalTool;
using NetFocus.DataStructure.TextEditor.Document;


namespace NetFocus.DataStructure.Commands
{
	public class RecentFilesMenuBuilder : ISubmenuBuilder
	{
		public ButtonItem[] BuildSubmenu(object owner)
		{
			IFileService fileService = (IFileService)NetFocus.DataStructure.Services.ServiceManager.Services.GetService(typeof(IFileService));
			ResourceService ResourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			RecentOpenMemeto recentOpen = fileService.RecentOpenMemeto;
			
			if (recentOpen.RecentFile.Count > 0) 
			{
				SdMenuCommand[] items = new SdMenuCommand[recentOpen.RecentFile.Count];
				
				for (int i = 0; i < recentOpen.RecentFile.Count; ++i) 
				{
					items[i] = new SdMenuCommand(recentOpen.RecentFile[i].ToString(), new EventHandler(LoadRecentFile));
					items[i].Description = stringParserService.Parse(ResourceService.GetString("XML.MainMenu.FileMenu.LoadRecentFileDescription"),
						new string[,] { {"FILE", recentOpen.RecentFile[i].ToString()} });
				}
				return items;
			}
			
			SdMenuCommand defaultMenu = new SdMenuCommand(ResourceService.GetString("XML.MainMenu.FileMenu.NoRecentFileDescription"));
			defaultMenu.Enabled = false;
			
			return new ButtonItem[] { defaultMenu };
		}
		
		void LoadRecentFile(object sender, EventArgs e)
		{
			SdMenuCommand item = (SdMenuCommand)sender;
			IFileService fileService = (IFileService)NetFocus.DataStructure.Services.ServiceManager.Services.GetService(typeof(IFileService));
			fileService.OpenFile(item.Text);
			WorkbenchSingleton.Workbench.ActiveViewContent.ViewSelected -= AlgorithmManager.Algorithms.ClearPadsHandler;
			WorkbenchSingleton.Workbench.ActiveViewContent.ViewSelected += AlgorithmManager.Algorithms.ClearPadsHandler;
			WorkbenchSingleton.Workbench.ActiveViewContent.SelectView();	
			AlgorithmManager.Algorithms.Timer.Enabled = false;
		}
	}
	
	
	public class ViewMenuBuilder : ISubmenuBuilder
	{
		class MyMenuItem : SdMenuCommand
		{
			IPadContent padContent;
			
			bool IsPadVisible 
			{
				get 
				{
					if(WorkbenchSingleton.Workbench.WorkbenchLayout != null)
					{
						return WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible(padContent); 
					}
					return false;
				}
			}
			
			public MyMenuItem(IPadContent padContent) : base(padContent.Title)
			{
				this.padContent = padContent;
				this.Click += new EventHandler(ClickEvent);
				this.Checked = IsPadVisible;
			}
			
			void UpdateThisItem(object sender, EventArgs e)//这个函数暂时不用,因为我没有找到一个关于菜单项更新时的事件
			{
				Checked = IsPadVisible;
			}
			
			void ClickEvent(object sender, EventArgs e)
			{
				if (IsPadVisible) 
				{
					WorkbenchSingleton.Workbench.WorkbenchLayout.HidePad(padContent);
				} 
				else 
				{
					WorkbenchSingleton.Workbench.WorkbenchLayout.ShowPad(padContent);
				}
			}
		}
		
		public ButtonItem[] BuildSubmenu(object owner)
		{
			ArrayList items = new ArrayList();
			foreach (IPadContent padContent in WorkbenchSingleton.Workbench.PadContentCollection) 
			{
				items.Add(new MyMenuItem(padContent));
			}
			return (ButtonItem[])items.ToArray(typeof(ButtonItem));

		}
	}
	
	
	public class ToolMenuBuilder : ISubmenuBuilder
	{
		public ButtonItem[] BuildSubmenu(object owner)
		{
			SdMenuCommand[] items = new SdMenuCommand[ToolLoader.Tool.Count];
			for (int i = 0; i < ToolLoader.Tool.Count; ++i) 
			{
				SdMenuCommand item = new SdMenuCommand(ToolLoader.Tool[i].ToString(), new EventHandler(ToolEvt));
				item.Description = "启动工具: " + String.Join(String.Empty, ToolLoader.Tool[i].ToString().Split('&'));
				items[i] = item;
			}
			return items;
		}
		
		void ToolEvt(object sender, EventArgs e)
		{
			SdMenuCommand item = (SdMenuCommand)sender;
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			for (int i = 0; i < ToolLoader.Tool.Count; ++i) 
			{
				if (item.Text == ToolLoader.Tool[i].ToString()) 
				{
					ExternalTool tool = (ExternalTool)ToolLoader.Tool[i];
					stringParserService.Properties["StartupPath"] = Application.StartupPath;
					string command = stringParserService.Parse(tool.Command);
					
					try 
					{
						ProcessStartInfo startinfo = new ProcessStartInfo(command, "");
						startinfo.WorkingDirectory = "";
						Process.Start(startinfo);
					} 
					catch (Exception ex) 
					{
						MessageBox.Show(command + "\n" + ex.ToString(),"启动程序时出错:",MessageBoxButtons.OK,MessageBoxIcon.Error);
					}
					break;
				}
			}
		}
	}
	

	public class HighlightingTypeBuilder : ISubmenuBuilder
	{
		TextArea  control      = null;
		ButtonItem[] menuCommands = null;
		
		public ButtonItem[] BuildSubmenu(object owner)
		{
			control = (TextArea)owner;
			
			ArrayList menuItems = new ArrayList();
			
			foreach (DictionaryEntry entry in HighlightingManager.Manager.HighlightingDefinitions) 
			{
				ButtonItem item = new ButtonItem();
				item.Text = entry.Key.ToString();
				item.Click +=new EventHandler(ChangeSyntax);
				item.Checked = control.Document.HighlightingStrategy.Name == entry.Key.ToString();
				menuItems.Add(item);
			}
			
			menuCommands = (ButtonItem[])menuItems.ToArray(typeof(ButtonItem));

			return menuCommands;
		}
		
		
		void ChangeSyntax(object sender, EventArgs e)
		{
			if (control != null) 
			{
				ButtonItem item = (ButtonItem)sender;
				foreach (ButtonItem i in menuCommands) 
				{
					i.Checked = false;
				}
				item.Checked = true;
				IHighlightingStrategy strat = HighlightingManager.Manager.FindHighlighterByName(item.Text);
				if (strat == null) 
				{
					throw new Exception("高亮度策略没有找到!");
				}
				
				control.Document.HighlightingStrategy = strat;
				
				control.Refresh();
			}
		}

	}	


}
