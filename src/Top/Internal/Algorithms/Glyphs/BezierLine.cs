using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace NetFocus.DataStructure.Internal.Algorithm.Glyphs
{
	/// <summary>
	/// 表示一条贝赛尔曲线的图元,在创建链表时使用
	/// </summary>
	public class BezierLine : IGlyph
	{
		Rectangle rectangle;
		Point[] points;
		int width = 2;
		Color color = Color.HotPink;
		GlyphAppearance appearance = GlyphAppearance.Flat;

		public BezierLine(Rectangle rectangle,Point[] points) : this(rectangle,points,2)
		{

		}
		public BezierLine(Rectangle rectangle,Point[] points,int width) : this(rectangle,points,width,Color.HotPink)
		{

		}
		public BezierLine(Rectangle rectangle,Point[] points,int width,Color color)
		{
			this.rectangle = rectangle;
			this.points = points;
			this.width = width;
			this.color = color;
		}

		private void DrawArrow(Graphics g,float dx,float dy,Color color)
		{
			int cx = 20;
			int cy = 20;
			Pen penWide   = new Pen(color,2);
			Point[] apt = {new Point(5*cx/32,3*cy/8),new Point(20*cx/32,0),new Point(20*cx/32,5*cy/8)};
			penWide.LineJoin = LineJoin.Miter;
			g.TranslateTransform(dx,dy);
			g.DrawLines(penWide,apt);
			g.TranslateTransform(-dx,-dy);

		}
		
		#region IGlyph 成员

		public void Draw(Graphics g)
		{
			Pen pen   = new Pen(color,width);
			g.DrawBezier(pen,points[0],points[1],points[2],points[3]);
			DrawArrow(g,points[3].X-13.0f,points[3].Y,this.color);
		}

		public Rectangle Bounds
		{
			get
			{
				return rectangle;
			}
			set
			{
				rectangle = new Rectangle(value.X,value.Y,value.Width,value.Height);
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
