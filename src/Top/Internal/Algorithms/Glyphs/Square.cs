using System;
using System.Drawing;

namespace NetFocus.DataStructure.Internal.Algorithm.Glyphs
{
	/// <summary>
	/// 一个正方形的图元对象
	/// </summary>
	public class Square : IGlyph
	{
		Color backColor = Color.White;
		int x,y,size;
		GlyphAppearance appearance = GlyphAppearance.Flat;
		string text = null;

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


		public Square(int x,int y,int size,string text) : this(x,y,size,Color.HotPink,text)
		{
		}

		public Square(int x,int y,int size,Color backColor,string text) : this(x,y,size,backColor,GlyphAppearance.Flat,text)
		{
		}
		public Square(int x,int y,int size,Color backColor,GlyphAppearance appearance,string text)
		{
			this.x = x;
			this.y = y;
			this.size = size;
			this.backColor = backColor;
			this.appearance = appearance;
			this.text = text;
		}

		
		#region IGlyph 成员

		public void Draw(Graphics g)
		{
			g.FillRectangle(new SolidBrush(backColor),this.x,this.y,this.size ,this.size);
			Font f = new Font(FontFamily.GenericSansSerif,(this.size * 2)/3);
			
			g.DrawString(this.text,f,SystemBrushes.ControlText,this.x + (this.size - f.Size)/3,this.y);
		
			switch(appearance)
			{
				case GlyphAppearance.Flat:
				case GlyphAppearance.Solid:
					break;
				case GlyphAppearance.Popup:
					g.DrawLine(new Pen(Color.White,1),this.x,this.y,this.x,this.y + this.size);//左边框;
					g.DrawLine(new Pen(Color.White,1),this.x,this.y,this.x + this.size,this.y);//上边框;
					g.DrawLine(new Pen(Color.Black,1),this.x ,this.y + this.size,this.x + this.size,this.y + this.size);//下边框;
					g.DrawLine(new Pen(Color.Black,1),this.x + this.size,this.y,this.x + this.size,this.y + this.size);//右边框;
					break;
			}
		}

		public Rectangle Bounds
		{
			get
			{
				return new Rectangle(this.x,this.y,this.size,this.size);
			}
			set
			{
				this.x = value.X;
				this.y = value.Y;
				this.size = value.Width;
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

		public bool Intersects(System.Drawing.Point p)
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
