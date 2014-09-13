
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.Gui.Dialogs;
using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Nodes;

namespace NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Panels
{
	class SpanOptionPanel : NodeOptionPanel {
		
		private System.Windows.Forms.TextBox nameBox;
		private System.Windows.Forms.TextBox beginBox;
		private System.Windows.Forms.TextBox endBox;
		private System.Windows.Forms.ComboBox ruleBox;
		private System.Windows.Forms.CheckBox useBegin;
		private System.Windows.Forms.CheckBox useEnd;
		private System.Windows.Forms.Button chgBegin;
		private System.Windows.Forms.Button chgEnd;
		private System.Windows.Forms.Button chgCont;
		private System.Windows.Forms.Label samBegin;
		private System.Windows.Forms.Label samEnd;
		private System.Windows.Forms.Label samCont;
		private System.Windows.Forms.CheckBox noEscBox;
		private System.Windows.Forms.CheckBox stopEolBox;
		
		public SpanOptionPanel(SpanNode parent) : base(parent)
		{
			SetupFromXmlFile(System.IO.Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\HighlightingEditor\Span.xfrm"));
			nameBox  = (TextBox)ControlDictionary["nameBox"];
			beginBox = (TextBox)ControlDictionary["beginBox"];
			endBox   = (TextBox)ControlDictionary["endBox"];
			ruleBox  = (ComboBox)ControlDictionary["ruleBox"];

			useBegin = (CheckBox)ControlDictionary["useBegin"];
			useEnd   = (CheckBox)ControlDictionary["useEnd"];

			chgBegin = (Button)ControlDictionary["chgBegin"];
			chgEnd   = (Button)ControlDictionary["chgEnd"];
			chgCont  = (Button)ControlDictionary["chgCont"];
			
			samBegin = (Label)ControlDictionary["samBegin"];
			samEnd   = (Label)ControlDictionary["samEnd"];
			samCont  = (Label)ControlDictionary["samCont"];

			stopEolBox = (CheckBox)ControlDictionary["stopEolBox"];
			noEscBox   = (CheckBox)ControlDictionary["noEscBox"];

			this.chgBegin.Click += new EventHandler(chgBeginClick);
			this.chgCont.Click  += new EventHandler(chgContClick);
			this.chgEnd.Click   += new EventHandler(chgEndClick);
			
			this.useBegin.CheckedChanged += new EventHandler(CheckedChanged);
			this.useEnd.CheckedChanged += new EventHandler(CheckedChanged);
		}
		
		EditorHighlightColor color;
		EditorHighlightColor beginColor;
		EditorHighlightColor endColor;
		
		public override void StoreSettings()
		{
			SpanNode node = (SpanNode)parentNode;
			
			node.Name = nameBox.Text;
			node.Begin = beginBox.Text;
			node.End = endBox.Text;
			node.StopEOL = stopEolBox.Checked;
			node.NoEscapeSequences = noEscBox.Checked;
			node.Rule = ruleBox.Text;
			
			node.HighlightColor = color;
			
			if (useBegin.Checked) {
				node.BeginColor = beginColor;
			} else {
				node.BeginColor = new EditorHighlightColor(true);
			}
			
			if (useEnd.Checked) {
				node.EndColor 	= endColor;
			} else {
				node.EndColor = new EditorHighlightColor(true);
			}
		}
		
		public override void LoadSettings()
		{
			SpanNode node = (SpanNode)parentNode;
			
			try {
				ruleBox.Items.Clear();
				foreach(RuleSetNode rn in node.Parent.Parent.Parent.Nodes) { // list rule sets
					if (!rn.IsRoot) ruleBox.Items.Add(rn.Text);
				}
			} catch {}
			
			IProperties properties = ((IProperties)PropertyService.GetProperty("NetFocus.DataStructure.TextEditor.Document.DefaultDocumentProperties", new DefaultProperties()));
			samBegin.Font = samEnd.Font = samCont.Font = FontContainer.DefaultFont;

			nameBox.Text = node.Name;
			ruleBox.Text = node.Rule;
			beginBox.Text = node.Begin;
			endBox.Text = node.End;
			stopEolBox.Checked = node.StopEOL;
			noEscBox.Checked = node.NoEscapeSequences;
			
			color = node.HighlightColor;
			beginColor = node.BeginColor;
			endColor = node.EndColor;
			
			if (beginColor != null) {
				if (!beginColor.NoColor) useBegin.Checked = true;
			}
			if (endColor != null) {
				if (!endColor.NoColor) useEnd.Checked = true;
			}
			
			PreviewUpdate(samBegin, beginColor);
			PreviewUpdate(samEnd, endColor);
			PreviewUpdate(samCont, color);
			CheckedChanged(null, null);
		}
		
		public override bool ValidateSettings()
		{
			if (nameBox.Text == "") {
				ValidationMessage(ResourceService.GetString("Dialog.HighlightingEditor.Span.NameEmpty"));
				return false;
			}
			if (beginBox.Text == "") {
				ValidationMessage(ResourceService.GetString("Dialog.HighlightingEditor.Span.BeginEmpty"));
				return false;
			}
			
			return true;
		}
		
		void chgBeginClick(object sender, EventArgs e)
		{
			using (EditHighlightingColorDialog dlg = new EditHighlightingColorDialog(beginColor)) {
				if (dlg.ShowDialog(this) == DialogResult.OK) {
					beginColor = dlg.Color;
					PreviewUpdate(samBegin, beginColor);
				}
			}
		}
		
		void chgEndClick(object sender, EventArgs e)
		{
			using (EditHighlightingColorDialog dlg = new EditHighlightingColorDialog(endColor)) {
				if (dlg.ShowDialog(this) == DialogResult.OK) {
					endColor = dlg.Color;
					PreviewUpdate(samEnd, endColor);
				}
			}
		}
		
		void chgContClick(object sender, EventArgs e)
		{
			using (EditHighlightingColorDialog dlg = new EditHighlightingColorDialog(color)) {
				if (dlg.ShowDialog(this) == DialogResult.OK) {
					color = dlg.Color;
					PreviewUpdate(samCont, color);
				}
			}
		}
		
		void CheckedChanged(object sender, EventArgs e)
		{
			chgEnd.Enabled = useEnd.Checked;
			chgBegin.Enabled = useBegin.Checked;
		}
	}
}
