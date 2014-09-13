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

	class MarkerNode : AbstractNode
	{
		bool        previous;
		string      what;
		EditorHighlightColor color;
		bool        markMarker = false;
		
		public MarkerNode(XmlElement el, bool prev)
		{
			Text = "Marker";
			previous = prev;
			OptionPanel = new MarkerOptionPanel(this, prev);
			
			if (el == null) return;
			
			color = new EditorHighlightColor(el);
			what  = el.InnerText;
			if (el.Attributes["markmarker"] != null) 
			{
				markMarker = Boolean.Parse(el.Attributes["markmarker"].InnerText);
			}
			
			UpdateNodeText();
			
		}
		
		public MarkerNode(string What, bool prev)
		{
			what = What;
			previous = prev;
			color = new EditorHighlightColor();
			UpdateNodeText();
			
			OptionPanel = new MarkerOptionPanel(this, prev);
		}
		
		public override void UpdateNodeText()
		{
			Text = what;
		}
		
		public override string ToXml()
		{
			string ret = "\t\t\t<Mark" + (previous ? "Previous" : "Following") + " ";
			ret += color.ToXml();
			if (markMarker) ret += " markmarker=\"true\"";
			ret += ">" + ReplaceXmlChars(what) + "</Mark" + (previous ? "Previous" : "Following") + ">\n\n";
			return ret;
		}
		
		public string What 
		{
			get 
			{
				return what;
			}
			set 
			{
				what = value;
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
		
		public bool MarkMarker 
		{
			get 
			{
				return markMarker;
			}
			set 
			{
				markMarker = value;
			}
		}
		
	}
	
}
