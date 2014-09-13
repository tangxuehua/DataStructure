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

	class RuleSetsNode : AbstractNode
	{
		public RuleSetsNode(XmlElement el)
		{
			Text = ResNodeName("RuleSets");
			
			OptionPanel = new RuleSetsOptionPanel(this);
			if (el == null) return;

			XmlNodeList nodes = el.GetElementsByTagName("RuleSet");
			
			foreach (XmlElement element in nodes) 
			{
				Nodes.Add(new RuleSetNode(element));
			}
			
		}

		public override void UpdateNodeText()
		{
		}
		
		public override string ToXml()
		{
			string ret = "\t<RuleSets>\n";
			foreach (RuleSetNode node in Nodes) 
			{
				ret += node.ToXml();
			}
			ret += "\t</RuleSets>\n\n";
			return ret;
		}
	}
	
}
