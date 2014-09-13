using System;
using System.Collections;
using System.Drawing;

namespace NetFocus.DataStructure.Internal.Algorithm.Glyphs
{
	/// <summary>
	/// 一个组合图园,里面有很多的Square对象
	/// </summary>
	public class SquareLine : IGlyph
	{
		Color backColor = Color.Transparent;
		int x,y,width;
		GlyphAppearance appearance = GlyphAppearance.Flat;
		ArrayList squareArray;

		
		public SquareLine(int x,int y,int width,ArrayList squareArray) : this(x,y,width,Color.HotPink,squareArray)
		{
		}

		public SquareLine(int x,int y,int width,Color backColor,ArrayList squareArray) : this(x,y,width,backColor,GlyphAppearance.Flat,squareArray)
		{
		}
		public SquareLine(int x,int y,int width,Color backColor,GlyphAppearance appearance,ArrayList squareArray)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.backColor = backColor;
			this.appearance = appearance;
			this.squareArray = squareArray;
		}

		
		#region IGlyph 成员

		public void Draw(System.Drawing.Graphics g)
		{
			// TODO:  添加 SquareLine.Draw 实现
			
		}

		
		public Rectangle Bounds
		{
			get
			{
				return new Rectangle(this.x,this.y,this.width,100);
			}
			set
			{
				this.x = value.X;
				this.y = value.Y;
				this.width = value.Width;
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
			// TODO:  添加 SquareLine.Intersects 实现
			return false;
		}

		
		public IIterator CreateIterator()
		{
			return new ArrayIterator(squareArray);
		}

		
		#endregion

	}
}
