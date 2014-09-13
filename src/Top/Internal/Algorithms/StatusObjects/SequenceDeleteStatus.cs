using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using NetFocus.DataStructure.Internal.Algorithm.Glyphs;

namespace NetFocus.DataStructure.Internal.Algorithm
{

	public class SequenceDeleteStatus
	{
		int i;
		string e;
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

		[Description("要删除的元素的位置.")]
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

		[Description("保存被删除元素的值.")]
		[Category("算法属性")]
		public string E
		{
			get
			{
				return e;
			}
			set
			{
				if(canEdit == true)
				{
					e = value;
					canEdit = false;
				}
			}
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
		[Description("表示动画面板中要删除元素的背景色.")]
		[Category("动画属性")]
		public Color 删除元素背景色
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

		
		public SequenceDeleteStatus(string l, int i)
		{
			this.l = l;
			this.i = i;
			this.length = l.Length;
			this.j = i;
			this.e = null;
			squareBackColor = Color.DarkCyan;
			insertBackColor = Color.Red;
			squareAppearance = GlyphAppearance.Flat;
			canEdit = false;

		}
	}
}
