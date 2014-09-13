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

	class SpansNode : AbstractNode
	{
		public SpansNode(XmlElement el)
		{
			Text = ResNodeName("Spans");
			
			OptionPanel = new SpansOptionPanel(this);
			if (el == null) return;
			
			XmlNodeList nodes = el.GetElementsByTagName("Span");
			foreach (XmlElement el2 in nodes) 
			{
				Nodes.Add(new SpanNode(el2));
			}
			
		}

		public override void UpdateNodeText()
		{
		}
		
		public override string ToXml()
		{
			string ret = "";
			foreach (SpanNode node in Nodes) 
			{
				ret += node.ToXml();
			}
			return ret;
		}
	}
	
}
