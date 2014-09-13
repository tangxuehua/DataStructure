

using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;

using NetFocus.DataStructure.Internal.ExternalTool;
using NetFocus.DataStructure.Properties;
using NetFocus.Components.AddIns.Codons;

using NetFocus.DataStructure.Services;

using NetFocus.DataStructure.TextEditor;
using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.Gui.Dialogs;
using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.Gui.Views;


namespace NetFocus.DataStructure.Gui.Dialogs.OptionPanels
{
	public class BehaviorTextEditorPanel : AbstractOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlFile(Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\BehaviorTextEditorPanel.xfrm"));
			
			((CheckBox)ControlDictionary["autoinsertCurlyBraceCheckBox"]).Checked = ((IProperties)CustomizationObject).GetProperty("AutoInsertCurlyBracket", true);
			((CheckBox)ControlDictionary["hideMouseCursorCheckBox"]).Checked      = ((IProperties)CustomizationObject).GetProperty("HideMouseCursor", true);
			((CheckBox)ControlDictionary["caretBehindEOLCheckBox"]).Checked       = ((IProperties)CustomizationObject).GetProperty("CursorBehindEOL", false);
			((CheckBox)ControlDictionary["auotInsertTemplatesCheckBox"]).Checked  = ((IProperties)CustomizationObject).GetProperty("AutoInsertTemplates", true);
			
			((CheckBox)ControlDictionary["convertTabsToSpacesCheckBox"]).Checked  = ((IProperties)CustomizationObject).GetProperty("TabsToSpaces", false);
			
			ControlDictionary["tabSizeTextBox"].Text    = ((IProperties)CustomizationObject).GetProperty("TabIndent", 4).ToString();
			ControlDictionary["indentSizeTextBox"].Text = ((IProperties)CustomizationObject).GetProperty("IndentationSize", 4).ToString();
			
			((ComboBox)ControlDictionary["indentStyleComboBox"]).Items.Add(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.None}"));
			((ComboBox)ControlDictionary["indentStyleComboBox"]).Items.Add(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.Automatic}"));
			((ComboBox)ControlDictionary["indentStyleComboBox"]).Items.Add(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.Smart}"));
			
			((ComboBox)ControlDictionary["indentStyleComboBox"]).SelectedIndex = (int)(IndentStyle)((IProperties)CustomizationObject).GetProperty("IndentStyle", IndentStyle.Smart);
		
			((ComboBox)ControlDictionary["mouseWhellDirectionComboBox"]).Items.Add(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.NormalMouseDirectionRadioButton}"));
			((ComboBox)ControlDictionary["mouseWhellDirectionComboBox"]).Items.Add(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.ReverseMouseDirectionRadioButton}"));
			((ComboBox)ControlDictionary["mouseWhellDirectionComboBox"]).SelectedIndex = ((IProperties)CustomizationObject).GetProperty("MouseWheelScrollDown", true) ? 0 : 1;
		}
		
		
		public override bool StorePanelContents()
		{
			((IProperties)CustomizationObject).SetProperty("TabsToSpaces",         ((CheckBox)ControlDictionary["convertTabsToSpacesCheckBox"]).Checked);
			((IProperties)CustomizationObject).SetProperty("MouseWheelScrollDown", ((ComboBox)ControlDictionary["mouseWhellDirectionComboBox"]).SelectedIndex == 0);
			
			((IProperties)CustomizationObject).SetProperty("AutoInsertCurlyBracket", ((CheckBox)ControlDictionary["autoinsertCurlyBraceCheckBox"]).Checked);
			((IProperties)CustomizationObject).SetProperty("HideMouseCursor",        ((CheckBox)ControlDictionary["hideMouseCursorCheckBox"]).Checked);
			((IProperties)CustomizationObject).SetProperty("CursorBehindEOL",        ((CheckBox)ControlDictionary["caretBehindEOLCheckBox"]).Checked);
			((IProperties)CustomizationObject).SetProperty("AutoInsertTemplates",    ((CheckBox)ControlDictionary["auotInsertTemplatesCheckBox"]).Checked);
			
			((IProperties)CustomizationObject).SetProperty("IndentStyle", ((ComboBox)ControlDictionary["indentStyleComboBox"]).SelectedIndex);
			
			try {
				int tabSize = Int32.Parse(ControlDictionary["tabSizeTextBox"].Text);
				
				//这里要求tabSize>0 ,因为如果为零，就会出现被零除的异常，这没有意义。
				if (tabSize > 0) {
					((IProperties)CustomizationObject).SetProperty("TabIndent", tabSize);
				}
			} catch (Exception) {
			}
			
			try {
				((IProperties)CustomizationObject).SetProperty("IndentationSize", Int32.Parse(ControlDictionary["indentSizeTextBox"].Text));
			} catch (Exception) {
			}
			
			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (content != null && (content is TextEditorView)) {
				TextEditorControl textarea = content.Control as TextEditorControl;
				textarea.OptionsChanged();
			}
			
			return true;
		}
	}
}
