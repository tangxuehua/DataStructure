

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.Internal.ExternalTool;
using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Gui.Views;
using NetFocus.DataStructure.Services;
using NetFocus.Components.AddIns.Codons;
using NetFocus.DataStructure.Gui.Dialogs;
using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.TextEditor;

namespace NetFocus.DataStructure.Gui.Dialogs.OptionPanels
{
	public class MarkersTextEditorPanel : AbstractOptionPanel
	{		
		public override void LoadPanelContents()
		{
			SetupFromXmlFile(Path.Combine(PropertyService.DataDirectory, @"resources\panels\MarkersTextEditorPanel.xfrm"));
			
			((CheckBox)ControlDictionary["showLineNumberCheckBox"]).Checked         = ((IProperties)CustomizationObject).GetProperty("ShowLineNumbers", true);
			((CheckBox)ControlDictionary["showInvalidLinesCheckBox"]).Checked       = ((IProperties)CustomizationObject).GetProperty("ShowInvalidLines", true);
			((CheckBox)ControlDictionary["showBracketHighlighterCheckBox"]).Checked = ((IProperties)CustomizationObject).GetProperty("ShowBracketHighlight", true);
			((CheckBox)ControlDictionary["showErrorsCheckBox"]).Checked             = ((IProperties)CustomizationObject).GetProperty("ShowErrors", true);
			((CheckBox)ControlDictionary["showHRulerCheckBox"]).Checked             = ((IProperties)CustomizationObject).GetProperty("ShowHRuler", false);
			((CheckBox)ControlDictionary["showEOLMarkersCheckBox"]).Checked         = ((IProperties)CustomizationObject).GetProperty("ShowEOLMarkers", false);
			((CheckBox)ControlDictionary["showVRulerCheckBox"]).Checked             = ((IProperties)CustomizationObject).GetProperty("ShowVRuler", false);
			((CheckBox)ControlDictionary["showTabCharsCheckBox"]).Checked           = ((IProperties)CustomizationObject).GetProperty("ShowTabs", false);
			((CheckBox)ControlDictionary["showSpaceCharsCheckBox"]).Checked         = ((IProperties)CustomizationObject).GetProperty("ShowSpaces", false);
			
			ControlDictionary["vRulerRowTextBox"].Text = ((IProperties)CustomizationObject).GetProperty("VRulerRow", 80).ToString();
			
			((ComboBox)ControlDictionary["lineMarkerStyleComboBox"]).Items.Add(ResourceService.GetString("Dialog.Options.IDEOptions.TextEditor.Markers.LineViewerStyle.None"));
			((ComboBox)ControlDictionary["lineMarkerStyleComboBox"]).Items.Add(ResourceService.GetString("Dialog.Options.IDEOptions.TextEditor.Markers.LineViewerStyle.FullRow"));
			((ComboBox)ControlDictionary["lineMarkerStyleComboBox"]).SelectedIndex = (int)(LineViewerStyle)((IProperties)CustomizationObject).GetProperty("LineViewerStyle", LineViewerStyle.None);
			
			
			
			((ComboBox)ControlDictionary["bracketMatchingStyleComboBox"]).Items.Add(ResourceService.GetString("Dialog.Options.IDEOptions.TextEditor.Markers.BracketMatchingStyle.BeforeCaret"));
			((ComboBox)ControlDictionary["bracketMatchingStyleComboBox"]).Items.Add(ResourceService.GetString("Dialog.Options.IDEOptions.TextEditor.Markers.BracketMatchingStyle.AfterCaret"));
			((ComboBox)ControlDictionary["bracketMatchingStyleComboBox"]).SelectedIndex = (int)(BracketMatchingStyle)((IProperties)CustomizationObject).GetProperty("BracketMatchingStyle", BracketMatchingStyle.After);
		}
		
		
		public override bool StorePanelContents()
		{
			((IProperties)CustomizationObject).SetProperty("ShowInvalidLines",     ((CheckBox)ControlDictionary["showInvalidLinesCheckBox"]).Checked);
			((IProperties)CustomizationObject).SetProperty("ShowLineNumbers",      ((CheckBox)ControlDictionary["showLineNumberCheckBox"]).Checked);
			((IProperties)CustomizationObject).SetProperty("ShowBracketHighlight", ((CheckBox)ControlDictionary["showBracketHighlighterCheckBox"]).Checked);
			((IProperties)CustomizationObject).SetProperty("ShowErrors",           ((CheckBox)ControlDictionary["showErrorsCheckBox"]).Checked);
			((IProperties)CustomizationObject).SetProperty("ShowHRuler",           ((CheckBox)ControlDictionary["showHRulerCheckBox"]).Checked);
			((IProperties)CustomizationObject).SetProperty("ShowEOLMarkers",       ((CheckBox)ControlDictionary["showEOLMarkersCheckBox"]).Checked);
			((IProperties)CustomizationObject).SetProperty("ShowVRuler",           ((CheckBox)ControlDictionary["showVRulerCheckBox"]).Checked);
			((IProperties)CustomizationObject).SetProperty("ShowTabs",             ((CheckBox)ControlDictionary["showTabCharsCheckBox"]).Checked);
			((IProperties)CustomizationObject).SetProperty("ShowSpaces",           ((CheckBox)ControlDictionary["showSpaceCharsCheckBox"]).Checked);
			
			try {
				((IProperties)CustomizationObject).SetProperty("VRulerRow", Int32.Parse(ControlDictionary["vRulerRowTextBox"].Text));
			} catch (Exception) {
			}
			
			((IProperties)CustomizationObject).SetProperty("LineViewerStyle", (LineViewerStyle)((ComboBox)ControlDictionary["lineMarkerStyleComboBox"]).SelectedIndex);
			((IProperties)CustomizationObject).SetProperty("BracketMatchingStyle", (BracketMatchingStyle)((ComboBox)ControlDictionary["bracketMatchingStyleComboBox"]).SelectedIndex);
			
			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (content != null && (content is TextEditorView)) {
				TextEditorControl textEditorControl = content.Control as TextEditorControl;
				textEditorControl.OptionsChanged();
			}
			
			return true;
		}
	}
}
