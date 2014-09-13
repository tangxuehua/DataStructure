
using System;
using System.Drawing;
using System.Diagnostics;

namespace NetFocus.DataStructure.TextEditor.Document
{
	public enum TextWordType {
		Word,
		Space,
		Tab
	}
	
	/// <summary>
	/// This class represents single words with color information, two special versions of a word are 
	/// spaces and tabs.
	/// </summary>
	public class TextWord
	{
		LineSegment     line;
		IDocument       document;
		int          offset;
		int          length;
		HighlightColor  highlightColor;
		public bool hasDefaultColor;
		static TextWord spaceWord = new SpaceTextWord();
		static TextWord tabWord   = new TabTextWord();
		
		static public TextWord Space {
			get {
				return spaceWord;
			}
		}
		
		static public TextWord Tab {
			get {
				return tabWord;
			}
		}
		
		public int Offset {
			get {
				return offset;
			}
		}
		
		public virtual int Length {
			get {
				return length;
			}
		}
		
		public bool HasDefaultColor {
			get {
				return hasDefaultColor;
			}
		}
		
		public virtual TextWordType Type {
			get {
				return TextWordType.Word;
			}
		}
		
		public string Word {
			get {
				if (document == null) {
					return String.Empty;
				}
				return document.GetText(line.Offset + offset, length);
			}
		}
		
		public Font Font {
			get {
				return highlightColor.Font;
			}
		}
		
		public Color Color {
			get {
				return highlightColor.Color;
			}
		}
		
		public HighlightColor HighlightColor {
			get {
				return highlightColor;
			}
			set {
				highlightColor = value;
			}
		}
		
		public virtual bool IsWhiteSpace {
			get {
				return false;
			}
		}
		
		
		protected TextWord()
		{
		}
		
		
		public TextWord(IDocument document, LineSegment line, int offset, int length, HighlightColor highlightColor, bool hasDefaultColor)
		{
			Debug.Assert(document != null);
			Debug.Assert(line != null);
			Debug.Assert(highlightColor != null);
			
			this.document = document;
			this.line  = line;
			this.offset = offset;
			this.length = length;
			this.highlightColor = highlightColor;
			this.hasDefaultColor = hasDefaultColor;
		}
		
	
	}
	public class SpaceTextWord : TextWord
	{
		public SpaceTextWord()
		{
			
		}
			
		public SpaceTextWord(HighlightColor  highlightColor)
		{
			
			base.HighlightColor  = highlightColor;
		}
		public override TextWordType Type 
		{
			get 
			{
				return TextWordType.Space;
			}
		}
		public override int Length
		{
			get
			{
				return 1;
			}
		}

		public override bool IsWhiteSpace 
		{
			get 
			{
				return true;
			}
		}
	}
		
	
	public class TabTextWord : TextWord
	{
		public TabTextWord()
		{
		}
		public TabTextWord(HighlightColor  highlightColor)
		{

			base.HighlightColor  = highlightColor;
		}
			
		public override int Length
		{
			get
			{
				return 1;
			}
		}	
		public override TextWordType Type 
		{
			get 
			{
				return TextWordType.Tab;
			}
		}
		public override bool IsWhiteSpace 
		{
			get 
			{
				return true;
			}
		}
	}
		
}
