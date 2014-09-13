
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Xml;
using System.Xml.Schema;
using NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Nodes;

namespace NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Panels
{
	class SchemeOptionPanel : NodeOptionPanel
	{
		private System.Windows.Forms.TextBox extBox;
		private System.Windows.Forms.TextBox nameBox;
		
		public SchemeOptionPanel(SchemeNode parent) : base(parent)
		{
			SetupFromXmlFile(System.IO.Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\HighlightingEditor\Scheme.xfrm"));
			nameBox = (TextBox)ControlDictionary["nameBox"];
			extBox  = (TextBox)ControlDictionary["extBox"];
		}
		
		public override void StoreSettings()
		{
			SchemeNode node = (SchemeNode)parentNode;
			node.Name = nameBox.Text;
			node.Extensions = extBox.Text.Split(';');
		}
		
		public override void LoadSettings()
		{
			SchemeNode node = (SchemeNode)parentNode;
			nameBox.Text = node.Name;
			extBox.Text = String.Join(";", node.Extensions);
		}
		
		public override bool ValidateSettings()
		{
			if (nameBox.Text == "") {
				ValidationMessage("${res:Dialog.HighlightingEditor.Scheme.NameEmpty}");
				return false;
			}
			
			return true;
		}
	}
}
