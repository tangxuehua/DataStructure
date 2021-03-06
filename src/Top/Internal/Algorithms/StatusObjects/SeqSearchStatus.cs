using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using NetFocus.DataStructure.Internal.Algorithm.Glyphs;

namespace NetFocus.DataStructure.Internal.Algorithm
{
	public class SeqSearchStatus
	{
		int i;
		int n;
		char key;
		string r;
		Color stringRColor;
		Color overElementColor;
		Color currentElementColor;
		Color headElementColor;
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

		public int I
		{
			get
			{
				return i;
			}
			set
			{
				if(canEdit == true)
				{
					i = value;
					canEdit = false;
				}
			}
		}
		public int N
		{
			get
			{
				return n;
			}
			set
			{
			}
		}
		public char Key
		{
			get
			{
				return key;
			}
			set
			{
			}
		}
		public string R
		{
			get
			{
				return r;
			}
			set
			{
			}
		}

		[Description("表示动画面板中线性表的颜色.")]
		[Category("动画属性")]
		public Color 线性表颜色
		{
			get
			{
				return stringRColor;
			}
			set
			{
				stringRColor = value;
			}
		}
		[Description("表示动画面板中R[0]元素的颜色.")]
		[Category("动画属性")]
		public Color 头元素颜色
		{
			get
			{
				return headElementColor;
			}
			set
			{
				headElementColor = value;
			}
		}
		[Description("表示动画面板中比较过元素的颜色.")]
		[Category("动画属性")]
		public Color 比较过元素颜色
		{
			get
			{
				return overElementColor;
			}
			set
			{
				overElementColor = value;
			}
		}
		[Description("表示动画面板中当前被比较元素的颜色.")]
		[Category("动画属性")]
		public Color 当前元素颜色
		{
			get
			{
				return currentElementColor;
			}
			set
			{
				currentElementColor = value;
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

		public SeqSearchStatus(string r,char key)
		{
			this.r = r;
			this.key = key;
			this.n = r.Length;
			this.i = -1;
			squareAppearance = GlyphAppearance.Popup;
			headElementColor = Color.HotPink;
			overElementColor = Color.LightGray;
			stringRColor = Color.Teal;
			currentElementColor = Color.Red;
			canEdit = false;

		}
	}
}
