using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Gui.Components;
using NetFocus.DataStructure.Services;
using Crownwood.Magic.Common;
using Crownwood.Magic.Docking;
using Crownwood.Magic.Menus;

using DevComponents.DotNetBar;


namespace NetFocus.DataStructure.Gui
{	
	/// <summary>
	/// This is the a Workspace with a single document interface.
	/// </summary>
	public class SdiWorkbenchLayout : IWorkbenchLayout
	{
		#region some variants

		static string defaultconfigFile = Application.StartupPath + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "data" + Path.DirectorySeparatorChar + "options" + Path.DirectorySeparatorChar + "LayoutConfig.xml";
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		static string configFile = propertyService.ConfigDirectory + "SdiLayoutConfig.xml";
		DefaultWorkbench workbench;
		DockingManager dockManager;
		OpenFileTab tabControl = new OpenFileTab();
		WindowContent leftContent = null;
		WindowContent bottomContent = null;
		Hashtable contentHash = new Hashtable();
		DockingManager.ContentHandler contentVisibleHandler = null;

		#endregion

		void RefreshMainMenu(Content content,EventArgs e)
		{
			workbench.CreateMenu(null,null);
		}
		
		//该函数的作用是当IWorkbenchWindow被关闭时关闭该窗体那的视图
		void CloseWindowEvent(object sender, EventArgs e)
		{
			IViewContent content = sender as IViewContent;
			if (content != null) 
			{
				CloseView(content);
				OnActiveViewContentChanged(this, null);

				if(workbench.ViewContentCollection.Count == 0)
				{
					tabControl.Visible = false;

					DefaultStatusBarService defaultStatusBarService = (DefaultStatusBarService)ServiceManager.Services.GetService(typeof(DefaultStatusBarService));
			
					defaultStatusBarService.SetCaretPosition(0,0,0);

				}
			}
			
		}
		
		void OnActiveViewContentChanged(object sender, EventArgs e)
		{
			if (ActiveViewContentChanged != null) 
			{
				ActiveViewContentChanged(this, e);
			}
		}
		
		
		#region implements the IWorkbenchLayout interface

		public IViewContent ActiveViewContent 
		{
			get 
			{
				if (tabControl.SelectedTab == null)  
				{
					return null;
				}
				return (IViewContent)tabControl.SelectedTab.Tag;
			}
		}

		
		public void Attach(IWorkbench currentWorkbench)
		{
			workbench = (DefaultWorkbench)currentWorkbench;
			workbench.Controls.Clear();
			
			tabControl.Style = (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("NetFocus.DataStructure.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
			tabControl.Dock = DockStyle.Fill;
			tabControl.ShrinkPagesToFit = true;
			tabControl.ShowArrows = false;
			tabControl.Appearance = Crownwood.Magic.Controls.TabControl.VisualAppearance.MultiBox;
			workbench.Controls.Add(tabControl);
			tabControl.Visible = false;
			
			dockManager = new DockingManager(workbench, VisualStyle.IDE);

			IStatusBarService statusBarService = (IStatusBarService)ServiceManager.Services.GetService(typeof(IStatusBarService));
			workbench.Controls.Add(statusBarService.Control);

			workbench.Menu = null;

			workbench.AddMenuAndToolbarControls();

			dockManager.InnerControl = tabControl;
			dockManager.OuterControl = statusBarService.Control;
			
			foreach (IViewContent content in workbench.ViewContentCollection) 
			{
				ShowView(content);
			}

			contentVisibleHandler = new DockingManager.ContentHandler(RefreshMainMenu);
			dockManager.ContentHidden += contentVisibleHandler;
			dockManager.ContentShown  += contentVisibleHandler; 

		}
		
		
		public void Detach()
		{
			if (dockManager != null) 
			{
				dockManager.SaveConfigToFile(configFile);
			}
			
			tabControl.TabPages.Clear();
			tabControl.Controls.Clear();
			
			if (dockManager != null) 
			{
				dockManager.Contents.Clear();
			}
			
			workbench.Controls.Clear();

		}
		
		public IPadContent GetPad(Type type)
		{
			foreach (IPadContent pad in workbench.PadContentCollection) 
			{
				if (pad.GetType() == type) 
				{
					return pad;
				}
			}
			return null;
		}
		
		public void ShowPad(IPadContent content)
		{
			if (contentHash[content] == null) 
			{
				IProperties properties = (IProperties)propertyService.GetProperty("Workspace.ViewMementos", new DefaultProperties());
				string type = content.GetType().ToString();
				content.Control.Dock = DockStyle.None;
				Content c1;
				if (content.Icon != null) 
				{
					ImageList imgList = new ImageList();
					imgList.Images.Add(content.Icon);
					c1 = dockManager.Contents.Add(content.Control, content.Title, imgList, 0);
				} 
				else 
				{
					c1 = dockManager.Contents.Add(content.Control, content.Title);
				}

                c1.DisplaySize = new Size(270, 200);

				contentHash[content] = c1;


                if (properties.GetProperty(type, "Left") == "Left")
                {
                    if (leftContent == null)
                    {
                        leftContent = dockManager.AddContentWithState(c1, State.DockLeft);
                    }
                    else
                    {
                        dockManager.AddContentToWindowContent(c1, leftContent);
                    }
                }
                else if (properties.GetProperty(type, "Left") == "Bottom")
                {
                    if (bottomContent == null)
                    {
                        bottomContent = dockManager.AddContentWithState(c1, State.DockBottom);
                    }
                    else
                    {
                        dockManager.AddContentToWindowContent(c1, bottomContent);
                    }
                }
			} 
			else 
			{
				Content c = (Content)contentHash[content];
				if (c != null) 
				{
					dockManager.ShowContent(c);
				}
			}
		}
		
		
		public void ShowPads(PadContentCollection contentCollection)
		{
			foreach(IPadContent pad in contentCollection)
			{
				ShowPad(pad);
			}

			if (File.Exists(configFile)) 
			{
				dockManager.LoadConfigFromFile(configFile);
			} 
			else if (File.Exists(defaultconfigFile)) 
			{
				dockManager.LoadConfigFromFile(defaultconfigFile);
			}
		}
		
		
		public void ActivatePad(IPadContent padContent)
		{
			Content content = (Content)contentHash[padContent];
			if (content != null) 
			{
				content.BringToFront();
			}
		}
		
		
		public bool IsVisible(IPadContent padContent)
		{
			Content content = (Content)contentHash[padContent];
			if (content != null) 
			{
				return content.Visible;
			}
			return false;
		}
		
		
		public void HidePad(IPadContent padContent)
		{
			Content content = (Content)contentHash[padContent];
			if (content != null) 
			{
				dockManager.HideContent(content);
			}
		}
		
		
		public void ShowView(IViewContent content)
		{
			workbench.ViewContentCollection.Add(content);

			if (propertyService.GetProperty("NetFocus.DataStructure.LoadDocumentProperties", true) && content is IMementoCapable) 
			{
				try 
				{
					IXmlConvertable memento = workbench.GetStoredMemento(content);
					if (memento != null) 
					{
						 ((IMementoCapable)content).SetMemento(memento);
					}
				} 
				catch (Exception e) 
				{
					Console.WriteLine("Can't get/set memento : " + e.ToString());
				}
			}

			content.Control.Dock = DockStyle.None;
			content.Control.Visible = true;
			tabControl.Visible = true;
			tabControl.BringToFront();

			tabControl.AddViewContentToTabPage(content);

			content.CloseEvent += new EventHandler(CloseWindowEvent);

			OnActiveViewContentChanged(this,null);
		}
				

		public void CloseView(IViewContent content)
		{
			bool tempProperties = propertyService.GetProperty("NetFocus.DataStructure.LoadDocumentProperties", true);
			if (tempProperties && content is IMementoCapable) 
			{
				workbench.StoreMemento(content);
			}
			workbench.ViewContentCollection.Remove(content);

			content.Dispose();
			
		}

		
		public void CloseViews()
		{
			//first copy all the open views
			ViewContentCollection allViews = new ViewContentCollection(workbench.ViewContentCollection);
			
			foreach (IViewContent content in allViews) 
			{
				content.CloseView(false);
			}
			
		}

		
		public event EventHandler ActiveViewContentChanged;

		#endregion


	}
}
