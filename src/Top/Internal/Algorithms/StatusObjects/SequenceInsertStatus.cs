using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using NetFocus.DataStructure.Internal.Algorithm.Glyphs;

namespace NetFocus.DataStructure.Internal.Algorithm
{

	public class SequenceInsertStatus 
	{
		
		int i;
		char e;
		string l;
		int length;
		int j;
		Color squareBackColor;
		Color insertBackColor;
		GlyphAppearance squareAppearance;

		bool canEdit;

		[Browsable(false)]
		public bool CanEdit
		{
			set
			{
				canEdit = value;	
			}
		}

		[Description("此属性代表要插入的位置.")]
		[Category("算法属性")]
		public int I
		{
			get
			{
				return i;
			}
			set
			{
			}
		}
		[Description("一个临时变量,用来移动数组中的元素.")]
		[Category("算法属性")]
		public int J
		{
			get
			{
				return j;
			}
			set
			{
				if(canEdit == true)
				{
					j = value;
					canEdit = false;
				}
			}
		}
		[Description("当前线性表的长度.")]
		[Category("算法属性")]
		public int Length
		{
			get
			{
				return length;
			}
			set
			{
				if(canEdit == true)
				{
					length = value;
					canEdit = false;
				}
			}
		}

		[Description("代表要插入到线性表中的字符.")]
		[Category("算法属性")]
		public char E
		{
			get
			{
				return e;
			}
			set{}
		}
		
		[Description("表示当前线性表字符串.")]
		[Category("算法属性")]
		public string L
		{
			get
			{
				return l;
			}
			set
			{
				if(canEdit == true)
				{
					l = value;
					canEdit = false;
				}
			}
		}

		[Description("表示动画面板中每个元素的背景色.")]
		[Category("动画属性")]
		public Color 图形背景色
		{
			get
			{
				return squareBackColor;
			}
			set
			{
				squareBackColor = value;
			}
		}
		[Description("表示动画面板中要插入元素的背景色.")]
		[Category("动画属性")]
		public Color 插入元素背景色
		{
			get
			{
				return insertBackColor;
			}
			set
			{
				insertBackColor = value;
			}
		}

		[Description("表示当前显示的动画的显示方式.")]
		[Category("动画属性")]
		public GlyphAppearance 图形外观
		{
			get
			{
				return squareAppearance;
			}
			set
			{
				squareAppearance = value;
			}
		}


		public SequenceInsertStatus(string l, int i, char e) 
		{
			canEdit = false;
			this.l = l + " ";
			this.i = i;
			this.e = e;
			this.length = this.l.Length - 1;
			j = this.length - 1;

			squareBackColor = Color.HotPink;
			insertBackColor = Color.GreenYellow;
			squareAppearance = GlyphAppearance.Flat;
		}


	}
		
}
