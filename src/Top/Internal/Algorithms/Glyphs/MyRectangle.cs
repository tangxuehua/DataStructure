using System;
using System.Drawing;

namespace NetFocus.DataStructure.Internal.Algorithm.Glyphs
{
	/// <summary>
	/// 一个矩形的图元(用于排序的算法)
	/// </summary>
	public class MyRectangle : IGlyph
	{
		int x,y,width,height;
		GlyphAppearance appearance;
		string text;
		Color backColor;

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


		public MyRectangle(int x,int y,int width,int height,string text) : this(x,y,width,height,Color.HotPink,text)
		{

		}
		public MyRectangle(int x,int y,int width,int height,Color backColor,string text) : this(x,y,width,height,Color.HotPink,GlyphAppearance.Popup,text)
		{

		}
		public MyRectangle(int x,int y,int width,int height,Color backColor,GlyphAppearance appearance,string text)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
			this.backColor = backColor;
			this.appearance = appearance;
			this.text = text;
		}
		
		
		#region IGlyph 成员

		public void Draw(Graphics g)
		{
			g.FillRectangle(new SolidBrush(backColor),this.x,this.y,this.width ,this.height);
			Font f = new Font(FontFamily.GenericSansSerif,15f);
			
			g.DrawString(this.text,f,SystemBrushes.ControlText,this.x + this.width / 4,this.y - 20);
		
			switch(appearance)
			{
				case GlyphAppearance.Flat:
				case GlyphAppearance.Solid:
					break;
				case GlyphAppearance.Popup:
					g.DrawLine(new Pen(Color.White,1),this.x,this.y,this.x,this.y + this.height);//左边框;
					g.DrawLine(new Pen(Color.White,1),this.x,this.y,this.x + this.width,this.y);//上边框;
					g.DrawLine(new Pen(Color.Black,1),this.x ,this.y + this.height,this.x + this.width,this.y + this.height);//下边框;
					g.DrawLine(new Pen(Color.Black,1),this.x + this.width,this.y,this.x + this.width,this.y + this.height);//右边框;
					break;
			}
		}

		public Rectangle Bounds
		{
			get
			{
				return new Rectangle(this.x,this.y,this.width,this.height);
			}
			set
			{
				this.x = value.X;
				this.y = value.Y;
				this.width = value.Width;
				this.height = value.Height;
			}
		}

		public Color BackColor
		{
			get
			{
				return backColor;
			}
			set
			{
				backColor = value;
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
