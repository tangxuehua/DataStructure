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

	class KeywordListsNode : AbstractNode
	{
		public KeywordListsNode(XmlElement el)
		{
			Text = ResNodeName("KeywordLists");
			OptionPanel = new KeywordListsOptionPanel(this);
			
			if (el == null) return;
			
			XmlNodeList nodes = el.GetElementsByTagName("KeyWords");
			if (nodes == null) return;
			
			foreach (XmlElement el2 in nodes) 
			{
				Nodes.Add(new KeywordListNode(el2));
			}
			
		}

		public override void UpdateNodeText()
		{
		}
		
		public override string ToXml()
		{
			string ret = "";
			foreach (KeywordListNode node in Nodes) 
			{
				ret += node.ToXml();
			}
			return ret;
		}
	}
	
}
