using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;

using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Gui.Dialogs;
using NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Panels;

namespace NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Nodes
{

	class DigitsNode : AbstractNode
	{
		EditorHighlightColor highlightColor;
		
		public EditorHighlightColor HighlightColor 
		{
			get 
			{
				return highlightColor;
			}
			set 
			{
				highlightColor = value;
			}
		}

		
		public DigitsNode(XmlElement el)
		{
			if (el != null) 
			{
				highlightColor = new EditorHighlightColor(el);
			} 
			else 
			{
				highlightColor = new EditorHighlightColor();
			}
			
			Text = ResNodeName("DigitsColor");
			
			OptionPanel = new DigitsOptionPanel(this);
		}

		
		public override void UpdateNodeText()
		{
		}
		
		
		public override string ToXml()
		{
			return "\t<Digits name=\"Digits\" " + highlightColor.ToXml() + "/>\n\n";
		}
	}
}
