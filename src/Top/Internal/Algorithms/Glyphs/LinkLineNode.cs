using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace NetFocus.DataStructure.Internal.Algorithm.Glyphs
{
	/// <summary>
	/// 代表一个带箭头的连接线的图元
	/// </summary>
	public class LinkLineNode : INode
	{
		Color color = Color.White;
		GlyphAppearance appearance = GlyphAppearance.Flat;
		int x1,y1,x2,y2;
		int width = 2;
		INode next = null;
		string text = null;


		public LinkLineNode(int x1,int y1,int x2,int y2) : this(x1,y1,x2,y2,2)
		{
		}

		public LinkLineNode(int x1,int y1,int x2,int y2,int width) : this(x1,y1,x2,y2,width,Color.HotPink)
		{
		}

		public LinkLineNode(int x1,int y1,int x2,int y2,int width,Color color)
		{
			this.x1 = x1;
			this.y1 = y1;
			this.x2 = x2;
			this.y2 = y2;
			this.width = width;
			this.color = color; 
		}

		
		private void DrawArrow(Graphics g,float dx,float dy,Color color)
		{
			int cx = 20;
			int cy = 20;
			Pen penWide   = new Pen(color,2);
			Point[] apt   = {new Point(0,6*cy/8),new Point(28*cx/32,4*cy/8),new Point(0,2*cy/8)};
	
			penWide.LineJoin = LineJoin.Miter;
			g.TranslateTransform(dx,dy);
			g.DrawLines(penWide,apt);
			g.TranslateTransform(-dx,-dy);

		}

		
		#region IGlyph 成员

		public void Draw(Graphics g)
		{
			g.DrawLine(new Pen(this.color,this.width),x1,y1,x2,y2);
			DrawArrow(g,x2-18.0f,y2-10.5f,this.color);
		}

		public Rectangle Bounds
		{
			get
			{
				return new Rectangle(x1,y2,x2-x1,y2);  //这里不用去关心,因为对于线条来说,它的Bounds不重要;
			}
			set
			{
				this.x1 = value.X;
				this.y1 = value.Y;
				this.x2 = value.X + value.Width;
				this.y2 = value.Height;
			}
		}

		public Color BackColor
		{
			get
			{
				return this.color;
			}
			set
			{
				this.color = value;
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
			return new NodeListIterator(this);
		}

		#endregion

		#region INode 成员

		public INode Next
		{
			get
			{
				return next;
			}
			set
			{
				next = value;
			}
		}

		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}


		#endregion
	}
}
