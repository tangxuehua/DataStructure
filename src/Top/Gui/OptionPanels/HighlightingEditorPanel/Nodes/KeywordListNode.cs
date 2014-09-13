using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using System.Collections.Specialized;
using System.Text;

using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Gui.Dialogs;
using NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Panels;

namespace NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Nodes
{

	class KeywordListNode : AbstractNode
	{
		EditorHighlightColor color;
		StringCollection words = new StringCollection();
		string name;
		
		public EditorHighlightColor HighlightColor 
		{
			get 
			{
				return color;
			}
			set 
			{
				color = value;
			}
		}
		
		public StringCollection Words 
		{
			get 
			{
				return words;
			}
			set 
			{
				if (words != null) 
				{
					words.Clear();
				}
				words = value;
			}
		}
		
		public string Name 
		{
			get 
			{
				return name;
			}
			set 
			{
				name = value;
			}
		}
		
		public KeywordListNode(XmlElement el)
		{
			Text = ResNodeName("KeywordList");
			OptionPanel = new KeywordListOptionPanel(this);
				
			if (el == null) return;

			color = new EditorHighlightColor(el);
			
			XmlNodeList keys = el.GetElementsByTagName("Key");
			foreach (XmlElement node in keys) 
			{
				if (node.Attributes["word"] != null) words.Add(node.Attributes["word"].InnerText);
			}
			
			if (el.Attributes["name"] != null) 
			{
				name = el.Attributes["name"].InnerText;
			}
			UpdateNodeText();
			
		}
		
		public KeywordListNode(string Name)
		{
			name = Name;
			color = new EditorHighlightColor();
			UpdateNodeText();

			OptionPanel = new KeywordListOptionPanel(this);
		}
		
		public override void UpdateNodeText()
		{
			if (name != "") Text = name;
		}
		
		public override string ToXml()
		{
			StringBuilder ret = new StringBuilder("\t\t\t<KeyWords name=\"");
			ret.Append(ReplaceXmlChars(name));
			ret.Append("\" ");
			////ret += color.ToXml() + ">\n";
			ret.Append(color.ToXml());
			ret.Append(">\n");
			foreach(string str in words) 
			{
				////ret += "\t\t\t\t<Key word=\"" + ReplaceXmlChars(str) + "\"/>\n";
				ret.Append("\t\t\t\t<Key word=\"");
				ret.Append(ReplaceXmlChars(str));
				ret.Append("\"/>\n");
			}
			////ret += "\t\t\t</KeyWords>\n\n";
			ret.Append("\t\t\t</KeyWords>\n\n");
			return ret.ToString();
		}
	}
	
}
