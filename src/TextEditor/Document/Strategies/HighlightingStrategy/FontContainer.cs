
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
	/// 一个封装字体的容器。
	/// </summary>
	public class FontContainer
	{
		static Font defaultfont    = null;
		static Font boldfont       = null;
		static Font italicfont     = null;
		static Font bolditalicfont = null;
		
		public static Font BoldFont {
			get {
				return boldfont;
			}
		}
		

		public static Font ItalicFont {
			get {
				return italicfont;
			}
		}
		

		public static Font BoldItalicFont {
			get {
				return bolditalicfont;
			}
		}
		

		public static Font DefaultFont {
			get {
				return defaultfont;
			}
			set {
				defaultfont    = value;
				boldfont       = new Font(defaultfont, FontStyle.Bold);
				italicfont     = new Font(defaultfont, FontStyle.Italic);
				bolditalicfont = new Font(defaultfont, FontStyle.Bold | FontStyle.Italic);
			}
		}
		

		//根据一个描述字体的字符串来得到一个字体对象。
		public static Font ParseFont(string font)
		{
			string[] descr = font.Split(new char[]{',', '='});
			return new Font(descr[1], Single.Parse(descr[3]));
		}
		
		
		static FontContainer()
		{
			DefaultFont = new Font("Courier New", 10);//初始化一个默认字体.
		}
	}
}
