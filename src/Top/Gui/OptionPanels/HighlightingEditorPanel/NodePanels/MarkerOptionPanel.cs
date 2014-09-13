
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
	class MarkerOptionPanel : NodeOptionPanel
	{
		private System.Windows.Forms.Button chgBtn;
		private System.Windows.Forms.CheckBox checkBox;
		private System.Windows.Forms.TextBox nameBox;
		private System.Windows.Forms.Label sampleLabel;
		
		bool previous;
		
		public MarkerOptionPanel(MarkerNode parent, bool prev) : base(parent)
		{
			SetupFromXmlFile(System.IO.Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\HighlightingEditor\Marker.xfrm"));
			
			chgBtn = (Button)ControlDictionary["chgBtn"];
			chgBtn.Click += new EventHandler(chgBtnClick);
			
			checkBox  = (CheckBox)ControlDictionary["checkBox"];
			nameBox   = (TextBox)ControlDictionary["nameBox"];
			sampleLabel = (Label)ControlDictionary["sampleLabel"];

			previous = prev;
			ControlDictionary["explLabel"].Text = ResourceService.GetString(previous ? "Dialog.HighlightingEditor.Marker.ExplanationPrev" : "Dialog.HighlightingEditor.Marker.ExplanationNext");
		}
		
		EditorHighlightColor color;
		
		public override void StoreSettings()
		{
			MarkerNode node = (MarkerNode)parentNode;
			
			node.What = nameBox.Text;
			node.HighlightColor = color;
			node.MarkMarker = checkBox.Checked;
		}
		
		public override void LoadSettings()
		{
			MarkerNode node = (MarkerNode)parentNode;
			
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			IProperties properties = ((IProperties)propertyService.GetProperty("NetFocus.DataStructure.TextEditor.Document.DefaultDocumentProperties", new DefaultProperties()));
			sampleLabel.Font = ParseFont(properties.GetProperty("DefaultFont", new Font("Courier New", 10).ToString()));

			color = node.HighlightColor;
			nameBox.Text = node.What;
			checkBox.Checked = node.MarkMarker;
			PreviewUpdate(sampleLabel, color);
		}
		
		public override bool ValidateSettings()
		{
			if (nameBox.Text == "") {
				ValidationMessage(ResourceService.GetString("Dialog.HighlightingEditor.Marker.NameEmpty"));
				return false;
			}
			
			return true;
		}
		
		void chgBtnClick(object sender, EventArgs e)
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
