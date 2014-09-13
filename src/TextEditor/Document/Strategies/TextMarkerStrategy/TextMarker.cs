
using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections;

namespace NetFocus.DataStructure.TextEditor.Document
{
	public enum TextMarkerType {
		SolidBlock, 
		Underlined,
		WaveLine
	}
	
	/// <summary>
	/// Description of TextMarker.	
	/// </summary>
	public class TextMarker : AbstractSegment
	{
		TextMarkerType textMarkerType;
		Color          color;
		string         toolTip = null;
		
		public TextMarkerType TextMarkerType {
			get {
				return textMarkerType;
			}
		}
		
		public Color Color {
			get {
				return color;
			}
		}
		
		public string ToolTip {
			get {
				return toolTip;
			}
			set {
				toolTip = value;
			}
		}
		
		public TextMarker(int offset, int length, TextMarkerType textMarkerType) : this(offset, length, textMarkerType, Color.Red)
		{
		}
		
		public TextMarker(int offset, int length, TextMarkerType textMarkerType, Color color)
		{
			this.offset          = offset;
			this.length          = length;
			this.textMarkerType  = textMarkerType;
			this.color           = color;
		}
	}
}
