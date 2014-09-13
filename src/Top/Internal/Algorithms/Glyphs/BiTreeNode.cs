using System;
using System.Drawing;


namespace NetFocus.DataStructure.Internal.Algorithm.Glyphs
{
	public class BiTreeNode : IBiTreeNode
	{
		Color backColor;
		GlyphAppearance appearance = GlyphAppearance.Flat;
		int x,y,diameter;
		string text = null;
		IBiTreeNode leftChild = null;  //左子结点
		IBiTreeNode rightChild = null;  //右子结点

		public BiTreeNode(int x,int y,int diameter,string text) : this(x,y,diameter,Color.HotPink,text)
		{
		}

		public BiTreeNode(int x,int y,int diameter,Color backColor,string text)
		{
			this.x = x;
			this.y = y;
			this.diameter = diameter;
			this.backColor = backColor;
			this.text = text;
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
			return new NullIterator(this);
		}

		
		#endregion

		#region IBiTreeNode 成员

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

		public IBiTreeNode LeftChild
		{
			get
			{
				return leftChild;
			}
			set
			{
				leftChild = value;
			}

		}

		public IBiTreeNode RightChild
		{
			get
			{
				return rightChild;
			}
			set
			{
				rightChild = value;
			}
		}

		
		#endregion

	}
}
