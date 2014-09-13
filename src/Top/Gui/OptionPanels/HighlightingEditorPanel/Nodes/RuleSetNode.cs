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

	class RuleSetNode : AbstractNode
	{
		bool noEscapeSequences = false;
		bool ignoreCase   = false;
		bool isRoot       = false;
		string name       = String.Empty;
		string delimiters = String.Empty;
		string reference  = String.Empty;
		
		KeywordListsNode keywordNode;
		SpansNode spansNode;
		MarkersNode prevMarkerNode;
		MarkersNode nextMarkerNode;
		
		public RuleSetNode(XmlElement el)
		{
			Text = ResNodeName("RuleSet");
			
			OptionPanel = new RuleSetOptionPanel(this);
			
			if (el == null) return;
			
			if (el.Attributes["name"] != null) 
			{
				name = el.Attributes["name"].InnerText;
				Text = name;
				isRoot = false;
			}
			
			if (name == "") 
			{
				Text = ResNodeName("RootRuleSet");
				isRoot = true;
			}
			
			if (el.Attributes["noescapesequences"] != null) 
			{
				noEscapeSequences = Boolean.Parse(el.Attributes["noescapesequences"].InnerText);
			}
			
			if (el.Attributes["reference"] != null) 
			{
				reference = el.Attributes["reference"].InnerText;
			}
			
			if (el.Attributes["ignorecase"] != null) 
			{
				ignoreCase  = Boolean.Parse(el.Attributes["ignorecase"].InnerText);
			}
			
			if (el["Delimiters"] != null) 
			{
				delimiters = el["Delimiters"].InnerText;
			}
			
			keywordNode = new KeywordListsNode(el);
			spansNode   = new SpansNode(el);
			prevMarkerNode = new MarkersNode(el, true);  // Previous
			nextMarkerNode = new MarkersNode(el, false); // Next
			Nodes.Add(keywordNode);
			Nodes.Add(spansNode);
			Nodes.Add(prevMarkerNode);
			Nodes.Add(nextMarkerNode);			
	
		}
		
		public RuleSetNode(string Name, string Delim, string Ref, bool noEsc, bool noCase)
		{
			name = Name;
			Text = Name;
			delimiters = Delim;
			reference = Ref;
			noEscapeSequences = noEsc;
			ignoreCase = noCase;
			
			keywordNode = new KeywordListsNode(null);
			spansNode   = new SpansNode(null);
			prevMarkerNode = new MarkersNode(null, true);  // Previous
			nextMarkerNode = new MarkersNode(null, false); // Next
			Nodes.Add(keywordNode);
			Nodes.Add(spansNode);
			Nodes.Add(prevMarkerNode);
			Nodes.Add(nextMarkerNode);	
			
			OptionPanel = new RuleSetOptionPanel(this);
		}
		
		public override void UpdateNodeText()
		{
			if (name != "" && !isRoot) 
			{
				Text = name;
			}
		}
		
		public override string ToXml()
		{
			if (reference != "")   return "\t\t<RuleSet name=\"" + ReplaceXmlChars(name) + "\" reference=\"" + ReplaceXmlChars(reference) + "\"></RuleSet>\n\n";
			
			string ret = "\t\t<RuleSet ignorecase=\"" + ignoreCase.ToString().ToLower() + "\" ";
			if (noEscapeSequences) ret += "noescapesequences=\"true\" ";
			if (!isRoot)           ret += "name=\"" + ReplaceXmlChars(name) + "\" ";
			ret += ">\n";
			
			ret += "\t\t\t<Delimiters>" + ReplaceXmlChars(delimiters) + "</Delimiters>\n\n";
			
			ret += spansNode.ToXml();
			ret += prevMarkerNode.ToXml();
			ret += nextMarkerNode.ToXml();
			ret += keywordNode.ToXml();
			
			ret += "\t\t</RuleSet>\n\n";
			
			return ret;
		}
		
		public string Delimiters 
		{
			get 
			{
				return delimiters;
			}
			set 
			{
				delimiters = value;
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
		
		public bool IgnoreCase 
		{
			get 
			{
				return ignoreCase;
			}
			set 
			{
				ignoreCase = value;
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
		
		public string Reference 
		{
			get 
			{
				return reference;
			}
			set 
			{
				reference = value;
			}
		}
		
		public bool IsRoot 
		{
			get 
			{
				return isRoot;
			}
		}
		
	}
	
}
