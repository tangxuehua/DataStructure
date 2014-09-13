
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Gui.Dialogs;
using NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Nodes;


namespace NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Panels
{

	class DigitsOptionPanel : NodeOptionPanel
	{
		private Button button;
		private Label sampleLabel;
		
		EditorHighlightColor color = new EditorHighlightColor();
		
		public DigitsOptionPanel(DigitsNode parent) : base(parent)
		{
			SetupFromXmlFile(System.IO.Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\HighlightingEditor\Digits.xfrm"));
			
			button = (Button)ControlDictionary["button"];
			button.Click += new EventHandler(EditButtonClicked);
			sampleLabel  = (Label)ControlDictionary["sampleLabel"];
		}

		
		public override void StoreSettings()
		{
			DigitsNode node = (DigitsNode)parentNode;
			
			node.HighlightColor = color;
		}
		
		
		public override void LoadSettings()
		{
			DigitsNode node = (DigitsNode)parentNode;
			
			IProperties properties = ((IProperties)PropertyService.GetProperty("NetFocus.DataStructure.TextEditor.Document.DefaultDocumentProperties", new DefaultProperties()));
			sampleLabel.Font = ParseFont(properties.GetProperty("DefaultFont", new Font("Courier New", 10).ToString()));
			color = node.HighlightColor;
			PreviewUpdate(sampleLabel, color);
		}
		
		
		void EditButtonClicked(object sender, EventArgs e)
		{
			using (EditHighlightingColorDialog dlg = new EditHighlightingColorDialog(color)) {
				if (dlg.ShowDialog(this) == DialogResult.OK) {
					color = dlg.Color;
					PreviewUpdate(sampleLabel, color);
				}
			}
		}
	}
}
