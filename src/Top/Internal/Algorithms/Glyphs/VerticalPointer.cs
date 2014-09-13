using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace NetFocus.DataStructure.Internal.Algorithm.Glyphs
{
	/// <summary>
	/// 表示一个垂直向上的指针图元(一般在排序的时候用到)
	/// </summary>
	public class VerticalPointer : IGlyph
	{
		Color color;
		int x,y;
		GlyphAppearance appearance = GlyphAppearance.Flat;
		string text = null;

		public VerticalPointer(int x,int y,Color backColor) : this(x,y,backColor,"")
		{
		}

		public VerticalPointer(int x,int y,Color backColor,string text) : this(x,y,backColor,text,GlyphAppearance.Flat)
		{
		}
		public VerticalPointer(int x,int y,Color backColor,string text,GlyphAppearance appearance)
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
			Pen penWide   = new Pen(color,2);
			Point[] apt   = {new Point(16*cx/32,4*cy/8),new Point(20*cx/32,2*cy/8),new Point(24*cx/32,4*cy/8)};
			Point[] apt1  = {new Point(20*cx/32,7*cy/8),new Point(20*cx/32,2*cy/8)};
	
			penWide.LineJoin = LineJoin.Miter;
			g.TranslateTransform(dx,dy);
			g.DrawLines(penWide,apt);
			g.DrawLines(penWide,apt1);
			g.TranslateTransform(-dx,-dy);

			g.DrawString(text,new Font(FontFamily.GenericSansSerif,15,FontStyle.Bold),new SolidBrush(color),dx + 10,dy);


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
