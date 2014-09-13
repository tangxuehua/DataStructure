
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Text;
using System.Xml;

using NetFocus.DataStructure.TextEditor.Util;


namespace NetFocus.DataStructure.TextEditor.Document
{
	public class HighlightRuleSet
	{
		LookupTable keyWords;
		LookupTable prevMarkers;
		LookupTable nextMarkers;
		ArrayList   spans = new ArrayList();
		IHighlightingStrategy highlighter = null;
		bool noEscapeSequences = false;
		string      reference  = null;
		bool ignoreCase = false;
		string name     = null;
		bool[] delimiters = new bool[256];
		
		public ArrayList Spans {
			get {
				return spans;
			}
		}
		
		internal IHighlightingStrategy Highlighter {
			get {
				return highlighter;
			}
			set {
				highlighter = value;
			}
		}
		
		public LookupTable KeyWords {
			get {
				return keyWords;
			}
		}
		
		public LookupTable PrevMarkers {
			get {
				return prevMarkers;
			}
		}
		
		public LookupTable NextMarkers {
			get {
				return nextMarkers;
			}
		}
		
		public bool[] Delimiters {
			get {
				return delimiters;
			}
		}
		
		public bool NoEscapeSequences {
			get {
				return noEscapeSequences;
			}
		}
		
		public bool IgnoreCase {
			get {
				return ignoreCase;
			}
		}
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public string Reference {
			get {
				return reference;
			}
		}
		
		
		public HighlightRuleSet()
		{
			keyWords    = new LookupTable(false);
			prevMarkers = new LookupTable(false);
			nextMarkers = new LookupTable(false);
		}
		
		//每个RuleSet节点的样子:<RuleSet ignorecase = "false">......</RuleSet>
		public HighlightRuleSet(XmlElement el)
		{
			
			if (el.Attributes["name"] != null) {
				Name = el.Attributes["name"].InnerText;
			}
			
			if (el.Attributes["noescapesequences"] != null) {
				noEscapeSequences = Boolean.Parse(el.Attributes["noescapesequences"].InnerText);
			}
			
			if (el.Attributes["reference"] != null) {
				reference = el.Attributes["reference"].InnerText;
			}
			
			if (el.Attributes["ignorecase"] != null) {
				ignoreCase  = Boolean.Parse(el.Attributes["ignorecase"].InnerText);
			}
			
			for (int i  = 0; i < Delimiters.Length; ++i) {
				Delimiters[i] = false;
			}
			
			if (el["Delimiters"] != null) {
				string delimiterString = el["Delimiters"].InnerText;
				foreach (char ch in delimiterString) {
					Delimiters[(int)ch] = true;//将当前字符置为分隔符.
				}
			}

			XmlNodeList nodes = null;

			//以下是初始化Span.
			nodes = el.GetElementsByTagName("Span");
			foreach (XmlElement el2 in nodes) 
			{
				Spans.Add(new Span(el2));
			}

			keyWords    = new LookupTable(!IgnoreCase);
			prevMarkers = new LookupTable(!IgnoreCase);
			nextMarkers = new LookupTable(!IgnoreCase);
			
			//以下是初始化KeyWords.
			nodes = el.GetElementsByTagName("KeyWords");
			foreach (XmlElement el2 in nodes) 
			{
				HighlightColor color = new HighlightColor(el2);
				
				XmlNodeList keys = el2.GetElementsByTagName("Key");
				foreach (XmlElement node in keys) 
				{
					keyWords[node.Attributes["word"].InnerText] = color;//为每个关键字都赋予一个HighLightColor对象.
				}
			}
			
			//每个MarkPrevious节点的样子:
			//<MarkPrevious bold = "true" italic = "false" color = "MidnightBlue">(</MarkPrevious>
			nodes = el.GetElementsByTagName("MarkPrevious");
			foreach (XmlElement el2 in nodes) {
				PrevMarker prev = new PrevMarker(el2);
				prevMarkers[prev.What] = prev;
			}
			//每个MarkFollowing节点的样子和MarkFollowing的样子完全一样.
			nodes = el.GetElementsByTagName("MarkFollowing");
			foreach (XmlElement el2 in nodes) {
				NextMarker next = new NextMarker(el2);
				nextMarkers[next.What] = next;
			}
		}
	}
}
