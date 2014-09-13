
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Nodes;

namespace NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Panels
{
	class RuleSetOptionPanel : NodeOptionPanel
	{
		private System.Windows.Forms.CheckBox igcaseBox;
		private System.Windows.Forms.CheckBox noEscBox;
		private System.Windows.Forms.TextBox refBox;
		private System.Windows.Forms.TextBox delimBox;
		private System.Windows.Forms.TextBox nameBox;
		
		public RuleSetOptionPanel(RuleSetNode parent) : base(parent)
		{
			SetupFromXmlFile(System.IO.Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\HighlightingEditor\RuleSet.xfrm"));
			
			nameBox  = (TextBox)ControlDictionary["nameBox"];
			refBox   = (TextBox)ControlDictionary["refBox"];
			delimBox = (TextBox)ControlDictionary["delimBox"];
			
			igcaseBox = (CheckBox)ControlDictionary["igcaseBox"];
			noEscBox  = (CheckBox)ControlDictionary["noEscBox"];
		}
		
		public override void StoreSettings()
		{
			RuleSetNode node = (RuleSetNode)parentNode;
			if (!node.IsRoot) node.Name = nameBox.Text;
			node.Reference = refBox.Text;
			node.Delimiters = delimBox.Text;
			node.NoEscapeSequences = noEscBox.Checked;
			node.IgnoreCase = igcaseBox.Checked;
		}
		
		public override void LoadSettings()
		{
			RuleSetNode node = (RuleSetNode)parentNode;
			
			nameBox.Text = node.Name;
			
			if (node.IsRoot) {
				nameBox.Text = ResourceService.GetString("Dialog.HighlightingEditor.TreeView.RootRuleSet");
				nameBox.Enabled = false;
			}
			
			refBox.Text = node.Reference;
			delimBox.Text = node.Delimiters;
			
			noEscBox.Checked = node.NoEscapeSequences;
			igcaseBox.Checked = node.IgnoreCase;
		}
		
		public override bool ValidateSettings()
		{
			if (nameBox.Text == "") {
				ValidationMessage(ResourceService.GetString("Dialog.HighlightingEditor.RuleSet.NameEmpty"));
				return false;
			}
			
			return true;
		}

	}
}
