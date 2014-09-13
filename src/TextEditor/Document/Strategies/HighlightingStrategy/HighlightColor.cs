
using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;


namespace NetFocus.DataStructure.TextEditor.Document
{
	/// <summary>
	/// 一个封装高亮度显示时颜色和字体的类. 
	/// </summary>
	public class HighlightColor
	{
		bool   systemColor     = false;
		string systemColorName = null;
		
		bool   systemBgColor     = false;
		string systemBgColorName = null;
		
		Color  color;
		Color  backgroundcolor = System.Drawing.Color.WhiteSmoke;
		
		bool   bold   = false;
		bool   italic = false;
		bool   hasForgeground = false;
		bool   hasBackground = false;
		
		public bool HasForgeground {
			get {
				return hasForgeground;
			}
		}
		
		public bool HasBackground {
			get {
				return hasBackground;
			}
		}
		
		
		public bool Bold {
			get {
				return bold;
			}
		}
		

		public bool Italic {
			get {
				return italic;
			}
		}
		

		public Color BackgroundColor {
			get {
				if (!systemBgColor) {//同Color属性.
					return backgroundcolor;
				}
				return ParseColorString(systemBgColorName);
			}
		}
		

		public Color Color {
			get {
				//先判断是否指定了系统的颜色.
				//如果指定了系统颜色,则通过ParseColorString函数分析该颜色.
				//如果没有指定,则使用color提供的值.
				if (!systemColor) {
					return color;
				}
				return ParseColorString(systemColorName);
			}
		}
		

		public Font Font {
			get {
				if (Bold) {
					return Italic ? FontContainer.BoldItalicFont : FontContainer.BoldFont;
				}
				return Italic ? FontContainer.ItalicFont : FontContainer.DefaultFont;
			}
		}
		

		Color ParseColorString(string colorName)
		{
			string[] cNames = colorName.Split('*');
			PropertyInfo myPropInfo = typeof(System.Drawing.SystemColors).GetProperty(cNames[0], BindingFlags.Public | 
			                                                                                     BindingFlags.Instance | 
			                                                                                     BindingFlags.Static);
			Color c = (Color)myPropInfo.GetValue(null, null);
			
			if (cNames.Length == 2) {
				// hack : can't figure out how to parse doubles with '.' (culture info might set the '.' to ',')
				double factor = Double.Parse(cNames[1]) / 100;
				c = Color.FromArgb((int)((double)c.R * factor), (int)((double)c.G * factor), (int)((double)c.B * factor));
			}
			
			return c;
		}
		
		//分析像#66cc99之类的颜色.
		static Color ParseColor(string c)
		{
			int a = 255;
			int offset = 0;
			if (c.Length > 7) 
			{
				offset = 2;
				a = Int32.Parse(c.Substring(1,2), NumberStyles.HexNumber);
			}
			
			int r = Int32.Parse(c.Substring(1 + offset,2), NumberStyles.HexNumber);
			int g = Int32.Parse(c.Substring(3 + offset,2), NumberStyles.HexNumber);
			int b = Int32.Parse(c.Substring(5 + offset,2), NumberStyles.HexNumber);
			return Color.FromArgb(a, r, g, b);
		}
		
		//根据一个<KeyWords name = "ValueTypes" bold="true" italic="false" color="Red"></KeyWords>
		//这种节点来得到一个HighlightColor对象.
		public HighlightColor(XmlElement el)
		{
			Debug.Assert(el != null, "NetFocus.DataStructure.TextEditor.Document.SyntaxColor(XmlElement el) : el == null");
			if (el.Attributes["bold"] != null) {
				bold = Boolean.Parse(el.Attributes["bold"].InnerText);
			}
			
			if (el.Attributes["italic"] != null) {
				italic = Boolean.Parse(el.Attributes["italic"].InnerText);
			}
			
			if (el.Attributes["color"] != null) {
				string c = el.Attributes["color"].InnerText;
				if (c[0] == '#') {
					color = ParseColor(c);
				} else if (c.StartsWith("SystemColors.")) {
					systemColor     = true;
					systemColorName = c.Substring("SystemColors.".Length);
				} else {
					color = (Color)(Color.GetType()).InvokeMember(c, BindingFlags.GetProperty, null, Color, new object[0]);
				}
				hasForgeground = true;
			} else {
				color = Color.Transparent; // to set it to the default value.
			}
			
			if (el.Attributes["bgcolor"] != null) {
				string c = el.Attributes["bgcolor"].InnerText;
				if (c[0] == '#') {
					backgroundcolor = ParseColor(c);
				} else if (c.StartsWith("SystemColors.")) {
					systemBgColor     = true;
					systemBgColorName = c.Substring("SystemColors.".Length);
				} else {
					backgroundcolor = (Color)(Color.GetType()).InvokeMember(c, BindingFlags.GetProperty, null, Color, new object[0]);
				}
				hasBackground = true;
			}
		}
		
		//功能同上面这个构造函数,只不过在初始化实例时提供了一个默认的实例对象.
		public HighlightColor(XmlElement el, HighlightColor defaultColor)
		{
			Debug.Assert(el != null, "NetFocus.DataStructure.TextEditor.Document.SyntaxColor(XmlElement el) : el == null");
			if (el.Attributes["bold"] != null) {
				bold = Boolean.Parse(el.Attributes["bold"].InnerText);
			} else {
				bold = defaultColor.Bold;
			}
			
			if (el.Attributes["italic"] != null) {
				italic = Boolean.Parse(el.Attributes["italic"].InnerText);
			} else {
				italic = defaultColor.Italic;
			}
			
			if (el.Attributes["color"] != null) {
				string c = el.Attributes["color"].InnerText;
				if (c[0] == '#') {
					color = ParseColor(c);
				} else if (c.StartsWith("SystemColors.")) {
					systemColor     = true;
					systemColorName = c.Substring("SystemColors.".Length);
				} else {
					color = (Color)(Color.GetType()).InvokeMember(c, BindingFlags.GetProperty, null, Color, new object[0]);
				}
				hasForgeground = true;
			} else {
				color = defaultColor.color;
			}
			
			if (el.Attributes["bgcolor"] != null) {
				string c = el.Attributes["bgcolor"].InnerText;
				if (c[0] == '#') {
					backgroundcolor = ParseColor(c);
				} else if (c.StartsWith("SystemColors.")) {
					systemBgColor     = true;
					systemBgColorName = c.Substring("SystemColors.".Length);
				} else {
					backgroundcolor = (Color)(Color.GetType()).InvokeMember(c, BindingFlags.GetProperty, null, Color, new object[0]);
				}
				hasBackground = true;
			} else {
				backgroundcolor = defaultColor.BackgroundColor;
			}
		}
		

		public HighlightColor(Color color, bool bold, bool italic)
		{
			hasForgeground = true;
			this.color  = color;
			this.bold   = bold;
			this.italic = italic;
		}
		

		public HighlightColor(Color color, Color backgroundcolor, bool bold, bool italic)
		{
			hasForgeground = true;
			hasBackground  = true;
			this.color            = color;
			this.backgroundcolor  = backgroundcolor;
			this.bold             = bold;
			this.italic           = italic;
		}
		
		
		public HighlightColor(string systemColor, string systemBackgroundColor, bool bold, bool italic)
		{
			hasForgeground = true;
			hasBackground  = true;
			
			this.systemColor  = true;
			systemColorName   = systemColor;
		
			systemBgColor     = true;
			systemBgColorName = systemBackgroundColor;
			
			this.bold         = bold;
			this.italic       = italic;
		}
		
		
		

	}
}
