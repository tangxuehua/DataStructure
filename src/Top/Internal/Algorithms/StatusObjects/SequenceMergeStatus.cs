using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using NetFocus.DataStructure.Internal.Algorithm.Glyphs;

namespace NetFocus.DataStructure.Internal.Algorithm
{

	public class SequenceMergeStatus
	{
		int i,j,k;
		int laLength,lbLength;
		int lcLength;
		string la,lb;
		string lc;
		Color squareBackColor;
		Color currentGlyphColor;
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
		
		
		public int K
		{
			get
			{
				return k;
			}
			set
			{
				if(canEdit == true)
				{
					k = value;
					canEdit = false;
				}
			}
		}
		
		
		public int LaLength
		{
			get
			{
				return laLength;
			}
			set{}
		}
		
		
		public int LbLength
		{
			get
			{
				return lbLength;
			}
			set{}
		}
		
		
		public int LcLength
		{
			get
			{
				return lcLength;
			}
			set
			{
				if(canEdit == true)
				{
					lcLength = value;
					canEdit = false;
				}
			}
		}
		

		public string La
		{
			get
			{
				return la;
			}
			set{}
		}

		
		public string Lb
		{
			get
			{
				return lb;
			}
			set{}
		}

		
		public string Lc
		{
			get
			{
				return lc;
			}
			set
			{
				if(canEdit == true)
				{
					lc = value;
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
		[Description("表示动画面板中当前元素的背景色.")]
		[Category("动画属性")]
		public Color 当前元素背景色
		{
			get
			{
				return currentGlyphColor;
			}
			set
			{
				currentGlyphColor = value;
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

		
		public SequenceMergeStatus(string la,string lb)
		{
			this.la = la;
			this.lb = lb;
			this.lc = "";
			this.laLength = la.Length;
			this.lbLength = lb.Length;
			this.lcLength = 0;
			this.i = 0;
			this.j = 0;
			this.k = 0;

			squareBackColor = Color.DarkCyan;
			currentGlyphColor = Color.HotPink;
			squareAppearance = GlyphAppearance.Flat;
			canEdit = false;

		}
	}
}
