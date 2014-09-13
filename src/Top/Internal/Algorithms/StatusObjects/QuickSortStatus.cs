using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using NetFocus.DataStructure.Internal.Algorithm.Glyphs;


namespace NetFocus.DataStructure.Internal.Algorithm
{

	public class QuickSortStatus
	{
		string r;
		int i,j,low,high;
		Color commonColor;
		Color currentColor;
		Color headColor;
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

		public string R
		{
			get
			{
				return r;
			}
			set
			{
				if(canEdit == true)
				{
					r = value;
					canEdit = false;
				}
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

		public int Low
		{
			get
			{
				return low;
			}
			set
			{
				if(canEdit == true)
				{
					low = value;
					canEdit = false;
				}
			}
		}
		public int High
		{
			get
			{
				return high;
			}
			set
			{
				if(canEdit == true)
				{
					high = value;
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
				return commonColor;
			}
			set
			{
				commonColor = value;
			}
		}
		[Description("表示动画面板中正在比较元素的背景色.")]
		[Category("动画属性")]
		public Color 当前背景色
		{
			get
			{
				return currentColor;
			}
			set
			{
				currentColor = value;
			}
		}
		[Description("表示动画面板中R【0】元素的背景色.")]
		[Category("动画属性")]
		public Color 头元素背景色
		{
			get
			{
				return headColor;
			}
			set
			{
				headColor = value;
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

		
		public QuickSortStatus(string r,int low,int high)
		{
			this.r = r;
			this.low = low;
			this.high = high;
			this.i = 0;
			this.j = 0;
			headColor = Color.Gold;
			commonColor = Color.HotPink;
			currentColor = Color.LightSeaGreen;
			squareAppearance = GlyphAppearance.Popup;
			canEdit = false;
		}


	}
}
