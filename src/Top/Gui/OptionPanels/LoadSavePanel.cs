
using System;
using System.IO;
using System.Windows.Forms;

using NetFocus.DataStructure.Gui.Dialogs;
using NetFocus.Components.AddIns.Codons;
using NetFocus.DataStructure.TextEditor.Document;

namespace NetFocus.DataStructure.Gui.Dialogs.OptionPanels
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class LoadSavePanel : AbstractOptionPanel
	{
		const string loadUserDataCheckBox        = "loadUserDataCheckBox";
		const string createBackupCopyCheckBox    = "createBackupCopyCheckBox";
		const string lineTerminatorStyleComboBox = "lineTerminatorStyleComboBox";
		
		public override void LoadPanelContents()
		{
			SetupFromXmlFile(Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\LoadSaveOptionPanel.xfrm"));
			
			((CheckBox)ControlDictionary[loadUserDataCheckBox]).Checked     = PropertyService.GetProperty("NetFocus.DataStructure.LoadDocumentProperties", true);
			((CheckBox)ControlDictionary[createBackupCopyCheckBox]).Checked = PropertyService.GetProperty("NetFocus.DataStructure.CreateBackupCopy", false);
			
			((ComboBox)ControlDictionary[lineTerminatorStyleComboBox]).Items.Add(StringParserService.Parse("${res:Dialog.Options.IDEOptions.LoadSaveOptions.WindowsRadioButton}"));
			((ComboBox)ControlDictionary[lineTerminatorStyleComboBox]).Items.Add(StringParserService.Parse("${res:Dialog.Options.IDEOptions.LoadSaveOptions.MacintoshRadioButton}"));
			((ComboBox)ControlDictionary[lineTerminatorStyleComboBox]).Items.Add(StringParserService.Parse("${res:Dialog.Options.IDEOptions.LoadSaveOptions.UnixRadioButton}"));
			
			((ComboBox)ControlDictionary[lineTerminatorStyleComboBox]).SelectedIndex = (int)(LineTerminatorStyle)PropertyService.GetProperty("NetFocus.DataStructure.LineTerminatorStyle", LineTerminatorStyle.Windows);
		}
		
		public override bool StorePanelContents()
		{
			PropertyService.SetProperty("NetFocus.DataStructure.LoadDocumentProperties", ((CheckBox)ControlDictionary[loadUserDataCheckBox]).Checked);
			PropertyService.SetProperty("NetFocus.DataStructure.CreateBackupCopy",       ((CheckBox)ControlDictionary[createBackupCopyCheckBox]).Checked);
			PropertyService.SetProperty("NetFocus.DataStructure.LineTerminatorStyle",    (LineTerminatorStyle)((ComboBox)ControlDictionary[lineTerminatorStyleComboBox]).SelectedIndex);
			
			return true;
		}
	}
}
