
using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;

using NetFocus.DataStructure.Internal.ExternalTool;
using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Services;
using NetFocus.Components.AddIns.Codons;


namespace NetFocus.DataStructure.Gui.Dialogs.OptionPanels
{
	public class ExternalToolPanel : AbstractOptionPanel
	{
		static string[] dependendControlNames = new string[] {
			"titleTextBox", "commandTextBox", 
			"titleLabel", "commandLabel", "browseButton", "moveUpButton", "moveDownButton"
		};
		
		public override void LoadPanelContents()
		{
			SetupFromXmlFile(Path.Combine(PropertyService.DataDirectory, @"resources\panels\ExternalToolOptions.xfrm"));
			
			((ListBox)ControlDictionary["toolListBox"]).BeginUpdate();
			try {
				foreach (object o in ToolLoader.Tool) {
					((ListBox)ControlDictionary["toolListBox"]).Items.Add(o);
				}
			} finally {
				((ListBox)ControlDictionary["toolListBox"]).EndUpdate();
			}
			
			((ListBox)ControlDictionary["toolListBox"]).SelectedIndexChanged += new EventHandler(selectEvent);
			ControlDictionary["removeButton"].Click   += new EventHandler(removeEvent);
			ControlDictionary["addButton"].Click      += new EventHandler(addEvent);
			ControlDictionary["moveUpButton"].Click   += new EventHandler(moveUpEvent);
			ControlDictionary["moveDownButton"].Click += new EventHandler(moveDownEvent);
			
			ControlDictionary["browseButton"].Click   += new EventHandler(browseEvent);
			
			
			selectEvent(this, EventArgs.Empty);
		}
		
		public override bool StorePanelContents()
		{
			ArrayList newlist = new ArrayList();
			foreach (ExternalTool tool in ((ListBox)ControlDictionary["toolListBox"]).Items) 
			{
				if (!FileUtilityService.IsValidFileName(tool.Command)) 
				{
					MessageService.ShowError(String.Format("The command of tool \"{0}\" is invalid.", tool.MenuCommand));
					return false;
				}
				newlist.Add(tool);
			}
			
			ToolLoader.Tool = newlist;
			ToolLoader.SaveTools();
			
			((DefaultWorkbench)WorkbenchSingleton.Workbench).CreateMenu(null,null);
			return true;
		}
		
		
		void browseEvent(object sender, EventArgs e)
		{
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				fdiag.CheckFileExists = true;
				fdiag.Filter = StringParserService.Parse("${res:DataStructure.FileFilter.ExecutableFiles}|*.exe;*.com;*.pif;*.bat;*.cmd|${res:DataStructure.FileFilter.AllFiles}|*.*");
				
				if (fdiag.ShowDialog() == DialogResult.OK) {
					ControlDictionary["commandTextBox"].Text = fdiag.FileName;
				}
			}
		}
		
		void moveUpEvent(object sender, EventArgs e)
		{
			int index = ((ListBox)ControlDictionary["toolListBox"]).SelectedIndex;
			if (index > 0) {
				((ListBox)ControlDictionary["toolListBox"]).SelectedIndexChanged -= new EventHandler(selectEvent);
				try {
					object tmp = ((ListBox)ControlDictionary["toolListBox"]).Items[index - 1];
					((ListBox)ControlDictionary["toolListBox"]).Items[index - 1] = ((ListBox)ControlDictionary["toolListBox"]).Items[index];
					((ListBox)ControlDictionary["toolListBox"]).Items[index] = tmp;
					((ListBox)ControlDictionary["toolListBox"]).SetSelected(index, false);
					((ListBox)ControlDictionary["toolListBox"]).SetSelected(index - 1, true);
				} finally {
					((ListBox)ControlDictionary["toolListBox"]).SelectedIndexChanged += new EventHandler(selectEvent);
				}
			}
			
		}
		void moveDownEvent(object sender, EventArgs e)
		{
			int index = ((ListBox)ControlDictionary["toolListBox"]).SelectedIndex;
			if (index >= 0 && index < ((ListBox)ControlDictionary["toolListBox"]).Items.Count - 1) {
				((ListBox)ControlDictionary["toolListBox"]).SelectedIndexChanged -= new EventHandler(selectEvent);
				try {
					object tmp = ((ListBox)ControlDictionary["toolListBox"]).Items[index + 1];
					((ListBox)ControlDictionary["toolListBox"]).Items[index + 1] = ((ListBox)ControlDictionary["toolListBox"]).Items[index];
					((ListBox)ControlDictionary["toolListBox"]).Items[index] = tmp;
					((ListBox)ControlDictionary["toolListBox"]).SetSelected(index, false);
					((ListBox)ControlDictionary["toolListBox"]).SetSelected(index + 1, true);
				} finally {
					((ListBox)ControlDictionary["toolListBox"]).SelectedIndexChanged += new EventHandler(selectEvent);
				}
			}
		}
		
		void propertyValueChanged(object sender, PropertyValueChangedEventArgs e)
		{
			foreach (ListViewItem item in ((ListView)ControlDictionary["toolListView"]).Items) {
				if (item.Tag != null) {
					item.Text = item.Tag.ToString();
				}
			}
			
		}
		
		void setToolValues(object sender, EventArgs e)
		{
			ExternalTool selectedItem = ((ListBox)ControlDictionary["toolListBox"]).SelectedItem as ExternalTool;
			
			selectedItem.MenuCommand        = ControlDictionary["titleTextBox"].Text;
			selectedItem.Command            = ControlDictionary["commandTextBox"].Text;		}
		
		void selectEvent(object sender, EventArgs e)
		{
			SetEnabledStatus(((ListBox)ControlDictionary["toolListBox"]).SelectedItems.Count > 0, "removeButton");
			
			ControlDictionary["titleTextBox"].TextChanged      -= new EventHandler(setToolValues);
			ControlDictionary["commandTextBox"].TextChanged    -= new EventHandler(setToolValues);			
			
			if (((ListBox)ControlDictionary["toolListBox"]).SelectedItems.Count == 1) {
				ExternalTool selectedItem = ((ListBox)ControlDictionary["toolListBox"]).SelectedItem as ExternalTool;
				SetEnabledStatus(true, dependendControlNames);
				ControlDictionary["titleTextBox"].Text      = selectedItem.MenuCommand;
				ControlDictionary["commandTextBox"].Text    = selectedItem.Command;
			} else {
				SetEnabledStatus(false, dependendControlNames);
				ControlDictionary["titleTextBox"].Text      = String.Empty;
				ControlDictionary["commandTextBox"].Text    = String.Empty;			
			}
			
			ControlDictionary["titleTextBox"].TextChanged      += new EventHandler(setToolValues);
			ControlDictionary["commandTextBox"].TextChanged    += new EventHandler(setToolValues);
		}
		
		void removeEvent(object sender, EventArgs e)
		{
			((ListBox)ControlDictionary["toolListBox"]).BeginUpdate();
			try {
				int index = ((ListBox)ControlDictionary["toolListBox"]).SelectedIndex;
				object[] selectedItems = new object[((ListBox)ControlDictionary["toolListBox"]).SelectedItems.Count];
				((ListBox)ControlDictionary["toolListBox"]).SelectedItems.CopyTo(selectedItems, 0);
				((ListBox)ControlDictionary["toolListBox"]).SelectedIndexChanged -= new EventHandler(selectEvent);
				foreach (object item in selectedItems) {
					((ListBox)ControlDictionary["toolListBox"]).Items.Remove(item);
				}
				((ListBox)ControlDictionary["toolListBox"]).SelectedIndexChanged += new EventHandler(selectEvent);
				if (((ListBox)ControlDictionary["toolListBox"]).Items.Count == 0) {
					selectEvent(this, EventArgs.Empty);
				} else {
					((ListBox)ControlDictionary["toolListBox"]).SelectedIndex = Math.Min(index,((ListBox)ControlDictionary["toolListBox"]).Items.Count - 1);
				}
			} finally {
				((ListBox)ControlDictionary["toolListBox"]).EndUpdate();
			}
		}
		
		void addEvent(object sender, EventArgs e)
		{
			((ListBox)ControlDictionary["toolListBox"]).BeginUpdate();
			try {
				((ListBox)ControlDictionary["toolListBox"]).Items.Add(new ExternalTool());
				((ListBox)ControlDictionary["toolListBox"]).SelectedIndexChanged -= new EventHandler(selectEvent);
				((ListBox)ControlDictionary["toolListBox"]).ClearSelected();
				((ListBox)ControlDictionary["toolListBox"]).SelectedIndexChanged += new EventHandler(selectEvent);
				((ListBox)ControlDictionary["toolListBox"]).SelectedIndex = ((ListBox)ControlDictionary["toolListBox"]).Items.Count - 1;
			} finally {
				((ListBox)ControlDictionary["toolListBox"]).EndUpdate();
			}
		}
	}
}
