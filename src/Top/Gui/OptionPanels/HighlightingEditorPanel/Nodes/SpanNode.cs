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

	class SpanNode : AbstractNode
	{
		bool        stopEOL;
		EditorHighlightColor color;
		EditorHighlightColor beginColor = null;
		EditorHighlightColor endColor = null;
		string      begin = "";
		string      end   = "";
		string      name  = "";
		string      rule  = "";
		bool        noEscapeSequences = false;
		
		public SpanNode(XmlElement el)
		{
			Text = ResNodeName("Span");
			
			OptionPanel = new SpanOptionPanel(this);

			if (el == null) return;
			
			color   = new EditorHighlightColor(el);
			
			if (el.Attributes["rule"] != null) 
			{
				rule = el.Attributes["rule"].InnerText;
			}
			
			if (el.Attributes["noescapesequences"] != null) 
			{
				noEscapeSequences = Boolean.Parse(el.Attributes["noescapesequences"].InnerText);
			}
			
			name    = el.Attributes["name"].InnerText;
			stopEOL = Boolean.Parse(el.Attributes["stopateol"].InnerText);
			begin   = el["Begin"].InnerText;
			beginColor = new EditorHighlightColor(el["Begin"]);
			
			if (el["End"] != null) 
			{
				end  = el["End"].InnerText;
				endColor = new EditorHighlightColor(el["End"]);
			}
			
			UpdateNodeText();
			
		}
		
		public override string ToXml()
		{
			string ret = "";
			ret = "\t\t\t<Span name=\"" + ReplaceXmlChars(name) + "\" ";
			if (noEscapeSequences) ret += "noescapesequences=\"true\" ";
			if (rule != "") ret += "rule=\"" + ReplaceXmlChars(rule) + "\" ";
			ret += "stopateol=\"" + stopEOL.ToString().ToLower() + "\" ";
			ret += color.ToXml();
			ret += ">\n";
			
			ret += "\t\t\t\t<Begin ";
			if (beginColor != null && !beginColor.NoColor) ret += beginColor.ToXml();
			ret += ">" + ReplaceXmlChars(begin) + "</Begin>\n";
			
			if (end != "") 
			{
				ret += "\t\t\t\t<End ";
				if (endColor != null && !endColor.NoColor) ret += endColor.ToXml();
				ret += ">" + ReplaceXmlChars(end) + "</End>\n";
			}
			ret += "\t\t\t</Span>\n\n";
			return ret;
		}
		
		public SpanNode(string Name)
		{
			name = Name;
			color = new EditorHighlightColor();
			UpdateNodeText();
			
			OptionPanel = new SpanOptionPanel(this);
		}
		
		public override void UpdateNodeText()
		{
			if (name != "") { Text = name; return; }
			
			if (end == "" && stopEOL) 
			{
				Text = begin + " to EOL";
			} 
			else 
			{
				Text = begin + " to " + end;
			}
		}
		
		public bool StopEOL 
		{
			get 
			{
				return stopEOL;
			}
			set 
			{
				stopEOL = value;
			}
		}
		
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
		
		public EditorHighlightColor BeginColor 
		{
			get 
			{		
				return beginColor;
			}
			set 
			{
				beginColor = value;
			}
		}
		
		public EditorHighlightColor EndColor 
		{
			get 
			{
				return endColor;
			}
			set 
			{
				endColor = value;
			}
		}
		
		public string Begin 
		{
			get 
			{
				return begin;
			}
			set 
			{
				begin = value;
			}
		}
		
		public string End 
		{
			get 
			{
				return end;
			}
			set 
			{
				end = value;
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
		
		public string Rule 
		{
			get 
			{
				return rule;
			}
			set 
			{
				rule = value;
			}
		}
		
		public bool NoEscapeSequences 
		{
			get 
			{
				return noEscapeSequences;
			}
			set 
			{
				noEscapeSequences = value;
			}
		}

	}
	
}
