using System;
using System.Drawing;

namespace NetFocus.DataStructure.Internal.Algorithm.Glyphs
{
	/// <summary>
	/// 表示一个圆形的图元,一般在链表中使用
	/// </summary>
	public class LinkCircleNode : INode
	{
		Color backColor = Color.White;
		GlyphAppearance appearance = GlyphAppearance.Flat;
		int x,y,diameter;
		string text = null;
		INode next = null;  //指向下一个节点

		public LinkCircleNode(int x,int y,int diameter,string text) : this(x,y,diameter,Color.HotPink,text)
		{
		}

		public LinkCircleNode(int x,int y,int diameter,Color backColor,string text)
		{
			this.x = x;
			this.y = y;
			this.diameter = diameter;
			this.backColor = backColor;
			this.text = text;
		}

		public override string ToString()
		{
			return this.Bounds.X.ToString() + "," + this.Bounds.Y.ToString(); //如果两个结点的x,y坐标相等,则认为这两个结点相等
		}


		#region IGlyph 成员

		public void Draw(Graphics g)
		{
			g.FillEllipse(new SolidBrush(backColor),this.x,this.y,this.diameter,this.diameter);
			
			Font f = new Font(FontFamily.GenericSansSerif,(this.diameter * 2)/3);
			
			g.DrawString(this.text,f,SystemBrushes.ControlText,this.x + (this.diameter - f.Size)/3,this.y);

		}

		public Rectangle Bounds
		{
			get
			{
				return new Rectangle(this.x,this.y,this.diameter,this.diameter);
			}
			set
			{
				this.x = value.X;
				this.y = value.Y;
				this.diameter = value.Width;
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
