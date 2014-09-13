using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using NetFocus.DataStructure.Internal.Algorithm.Glyphs;

namespace NetFocus.DataStructure.Internal.Algorithm
{
	public class IndexBFStatus
	{
		string s,t;
		int pos,sLength,tLength;
	    int i,j;
		int findPosition;
		Color stringTColor;
		Color stringSColor;
		Color currentElementColor;
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
		
		[Description("串S中被比较元素的位置.")]
		[Category("算法属性")]
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
		

		[Description("子串T中被比较元素的位置.")]
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
		
		[Description("串S.")]
		[Category("算法属性")]
		public string S
		{
			get
			{
				return s;
			}
			set
			{
				
			}
		}
		
		[Description("串T.")]
		[Category("算法属性")]
		public string T
		{
			get
			{
				return t;
			}
			set
			{
				
			}
		}
		
		[Description("查找的起始位置.")]
		[Category("算法属性")]
		public int Pos
		{
			get
			{
				return pos;
			}
			set
			{
				
			}
		}
		
		[Description("串S的长度.")]
		[Category("算法属性")]
		public int SLength
		{
			get
			{
				return sLength;
			}
			set
			{
				
			}
		}
		
		[Description("串T的长度.")]
		[Category("算法属性")]
		public int TLength
		{
			get
			{
				return tLength;
			}
			set
			{
				
			}
		}


		[Description("表示动画面板中串S的颜色.")]
		[Category("动画属性")]
		public Color 串S元素颜色
		{
			get
			{
				return stringSColor;
			}
			set
			{
				stringSColor = value;
			}
		}
		[Description("表示动画面板中串T的颜色.")]
		[Category("动画属性")]
		public Color 串T元素颜色
		{
			get
			{
				return stringTColor;
			}
			set
			{
				stringTColor = value;
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
		[Description("找到的位置.")]
		[Category("算法属性")]
		public int 找到位置
		{
			get
			{
				return findPosition;
			}
			set
			{
				if(canEdit == true)
				{
					findPosition = value;
					canEdit = false;
				}
			}
		}
		


		public IndexBFStatus(string s,string t,int pos)
		{
			this.s = s;
			this.t = t;
			this.pos = pos;
			this.sLength = s.Length;
			this.tLength = t.Length;
			this.i = 0;
			this.j = 0;
			this.findPosition = -1;
			squareAppearance = GlyphAppearance.Popup;
			stringTColor = SystemColors.InactiveBorder;
			stringSColor = SystemColors.InactiveBorder;
			currentElementColor = Color.Red;
			canEdit = false;

		}


	}
}
