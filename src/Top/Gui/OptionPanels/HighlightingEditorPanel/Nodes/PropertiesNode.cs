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

	class PropertiesNode : AbstractNode
	{
		public Hashtable Properties = new Hashtable();
		
		public PropertiesNode(XmlElement el)
		{
			Text = ResNodeName("Properties");
			OptionPanel = new PropertiesOptionPanel(this);

			if (el == null) return;
			
			foreach (XmlElement el2 in el.ChildNodes) 
			{
				if (el2.Attributes["name"] == null || el2.Attributes["value"] == null) continue;
				Properties.Add(el2.Attributes["name"].InnerText, el2.Attributes["value"].InnerText);
			}
			
		}
	
		public override void UpdateNodeText()
		{
		}
		
		public override string ToXml()
		{
			string ret = "\t<Properties>\n";
			foreach (DictionaryEntry de in Properties) 
			{
				ret += "\t\t<Property name=\"" + ReplaceXmlChars((string)de.Key) 
					+ "\" value=\"" + ReplaceXmlChars((string)de.Value) + "\"/>\n";
			}
			ret += "\t</Properties>\n\n";
			return ret;
		}
	}
	
}
