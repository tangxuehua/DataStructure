using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using NetFocus.DataStructure.Internal.Algorithm.Glyphs;

namespace NetFocus.DataStructure.Internal.Algorithm
{

	public class CreateListStatus
	{
		int i;
		string l;
		string p = null;
		int n;

		Color nodeColor;
		Color headNodeColor;
		Color insertNodeColor;
		bool canEdit;

		[Browsable(false)]
		public bool CanEdit
		{
			set
			{
				canEdit = value;	
			}
		}

		[Description("P指向当前结点.")]
		[Category("算法属性")]
		public string P
		{
			get
			{
				return p;
			}
			set
			{
				if(canEdit == true)
				{
					p = value;
					canEdit = false;
				}
			}
		}

		[Description("一个用于计数的临时变量")]
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
		[Description("要创建的总共结点数")]
		[Category("算法属性")]
		public int N
		{
			get
			{
				return n;
			}
			set{}
		}
		[Description("表示当前链表.")]
		[Category("算法属性")]
		public string L
		{
			get
			{
				return l;
			}
			set{}
		}
		
		[Description("表示动画面板中每个结点的颜色.")]
		[Category("动画属性")]
		public Color 结点颜色
		{
			get
			{
				return nodeColor;
			}
			set
			{
				nodeColor = value;
			}
		}
		[Description("表示动画面板中头结点的颜色.")]
		[Category("动画属性")]
		public Color 头结点颜色
		{
			get
			{
				return headNodeColor;
			}
			set
			{
				headNodeColor = value;
			}
		}
		[Description("表示动画面板中要插入的结点的颜色.")]
		[Category("动画属性")]
		public Color 插入结点颜色
		{
			get
			{
				return insertNodeColor;
			}
			set
			{
				insertNodeColor = value;
			}
		}


		public CreateListStatus(string l)
		{
			this.l = l;
			this.p = null;
			this.n = l.Length;
			this.i = n;
			nodeColor = Color.DarkTurquoise;
			headNodeColor = Color.Red;
			insertNodeColor = Color.DarkOrange;
			canEdit = false;
		}
	}
}
