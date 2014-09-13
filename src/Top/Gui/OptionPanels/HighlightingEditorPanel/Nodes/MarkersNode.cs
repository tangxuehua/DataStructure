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

	class MarkersNode : AbstractNode
	{
		public MarkersNode(XmlElement el, bool prev)
		{
			Text = ResNodeName(prev ? "MarkPreviousWord" : "MarkNextWord");
			
			OptionPanel = new MarkersOptionPanel(this, prev);
			if (el == null) return;
			
			XmlNodeList nodes = el.GetElementsByTagName(prev ? "MarkPrevious" : "MarkFollowing");
			
			foreach (XmlElement el2 in nodes) 
			{
				Nodes.Add(new MarkerNode(el2, prev));
			}
			
		}

		public override void UpdateNodeText()
		{
		}
		
		public override string ToXml()
		{
			string ret = "";
			foreach (MarkerNode node in Nodes) 
			{
				ret += node.ToXml();
			}
			return ret;
		}
	}
	
}
