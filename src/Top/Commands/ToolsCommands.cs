
using System;
using System.Collections;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using NetFocus.DataStructure.Services;
using NetFocus.Components.AddIns;
using NetFocus.DataStructure.Properties;
using NetFocus.Components.AddIns.Codons;

using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.Gui.Dialogs;

namespace NetFocus.DataStructure.Commands
{
	public class OptionsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			IProperties proterties = (IProperties)propertyService.GetProperty("NetFocus.DataStructure.TextEditor.Document.DefaultDocumentProperties", new DefaultProperties());
			IAddInTreeNode treeNode = AddInTreeSingleton.AddInTree.GetTreeNode("/DataStructure/Dialogs/OptionsDialog");
			using (TreeViewOptions optionsDialog = new TreeViewOptions(proterties,treeNode)) {
				optionsDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
				
				optionsDialog.Owner = (Form)WorkbenchSingleton.Workbench;
				optionsDialog.ShowDialog();
			}
		}
	}
	
	public class ToggleFullscreenCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			((DefaultWorkbench)WorkbenchSingleton.Workbench).FullScreen = !((DefaultWorkbench)WorkbenchSingleton.Workbench).FullScreen;
		}
	}
	
	
}
