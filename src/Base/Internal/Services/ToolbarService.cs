using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.Commands;
using NetFocus.DataStructure.AddIns.Codons;
using NetFocus.Components.AddIns;


using DevComponents.DotNetBar;


namespace NetFocus.DataStructure.Services
{
	public class ClickWrapper
	{
		ICommand command = null;

		public ClickWrapper(ICommand command)
		{
			this.command = command;	
		}
		public void Run(object sender,EventArgs e)
		{
			this.command.Run();
		}
	}
	
	public class ToolbarService : AbstractService
	{
		readonly static string toolBarPath     = "/DataStructure/Workbench/ToolBar";
		
		IAddInTreeNode node;
		
		public ToolbarService()//初始化插件树中的toolBar节点.
		{
			this.node  = AddInTreeSingleton.AddInTree.GetTreeNode(toolBarPath);
		}
		
		
		public Bar[] CreateToolbars()
		{
			ToolbarCodon[] codons = (ToolbarCodon[])(node.BuildChildItems(this)).ToArray(typeof(ToolbarCodon));
			
			Bar[] toolBars = new Bar[codons.Length];//创建一个工具栏数组.
			
			for (int i = 0; i < toolBars.Length; ++i) 
			{
				toolBars[i] = CreateToolBarFromCodon(codons[i]);
			}
			return toolBars;
		}
		
		
		Bar CreateToolBarFromCodon(ToolbarCodon codon)
		{
			Bar bar = new Bar();
			bar.Stretch=true;
			bar.CanHide=false;
			bar.GrabHandleStyle=eGrabHandleStyle.StripeFlat;
			bar.WrapItemsDock=false;
			bar.BackColor = SystemColors.Control;

			ResourceService ResourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));

			foreach (ToolbarItemCodon childCodon in codon.SubItems) 
			{
				ButtonItem button = new ButtonItem();
				
				if (childCodon.Icon != null) 
				{
					button.Image = ResourceService.GetBitmap(childCodon.Icon);
				}
				if (childCodon.Text != null) 
				{
					button.Text = stringParserService.Parse(childCodon.Text);
					button.ButtonStyle = eButtonStyle.ImageAndText;
				}
				
				if (childCodon.ToolTip != null) 
				{
					if (childCodon.BeginGroup == "true") 
					{
						button.BeginGroup = true;
					} 
					else 
					{
						button.Tooltip = stringParserService.Parse(childCodon.ToolTip);
					}
				}
				button.Enabled     = childCodon.Enabled;
				if (childCodon.Class != null) 
				{
					ClickWrapper clickHandler = new ClickWrapper((ICommand)childCodon.AddIn.CreateObject(childCodon.Class));
					button.Click += new EventHandler(clickHandler.Run);
				}
				bar.Items.Add(button);
			}
			return bar;
		}
		
	}

}
