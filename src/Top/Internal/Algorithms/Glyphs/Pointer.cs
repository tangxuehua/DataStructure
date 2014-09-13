using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace NetFocus.DataStructure.Internal.Algorithm.Glyphs
{
	/// <summary>
	/// 一个指针图园(用于链表操作时生成一个指向链表结点的指针)
	/// </summary>
	public class Pointer : IGlyph
	{
		Color color = Color.White;
		int x,y;
		GlyphAppearance appearance = GlyphAppearance.Flat;
		string text = null;

		public Pointer(int x,int y,Color backColor) : this(x,y,backColor,"")
		{
		}

		public Pointer(int x,int y,Color backColor,string text) : this(x,y,backColor,text,GlyphAppearance.Flat)
		{
		}
		public Pointer(int x,int y,Color backColor,string text,GlyphAppearance appearance)
		{
			this.x = x;
			this.y = y;
			this.color = backColor;
			this.text = text;
			this.appearance = appearance;

		}


		private void DrawPointer(Graphics g,int dx,int dy,Color color,string text)
		{
			int cx = 40;
			int cy = 40;
			Pen penWide   = new Pen(color,5);
			Point[] apt   = {new Point(12*cx/32,4*cy/8),new Point(28*cx/32,6*cy/8),new Point(22*cx/32,2*cy/8)};
			Point[] apt1  = {new Point(10*cx/32,1*cy/8),new Point(28*cx/32,6*cy/8)};
	
			penWide.LineJoin = LineJoin.Miter;
			g.TranslateTransform(dx,dy);
			g.DrawLines(penWide,apt);
			g.DrawLines(penWide,apt1);
			g.TranslateTransform(-dx,-dy);

			g.DrawString(text,new Font(FontFamily.GenericSansSerif,15),new SolidBrush(color),dx,dy);


		}


		public void SetToNewPosition(int x,int y)
		{
			this.x = x;
			this.y = y;
		}
		
		
		#region IGlyph 成员

		public void Draw(Graphics g)
		{
			DrawPointer(g,x,y,color,text);
		}

		public Rectangle Bounds
		{
			get
			{
				return new Rectangle (x,y,40,40);
			}
			set
			{
				x = value.X;
				y = value.Y;
			}
		}

		public Color BackColor
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

		public GlyphAppearance Appearance
		{
			get
			{
				return appearance;
			}
			set
			{
				appearance = value;
			}
		}

		public bool Intersects(Point p)
		{
			return false;
		}

		public IIterator CreateIterator()
		{
			return new NullIterator(this);
		}


		#endregion

	}
}
