using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using NetFocus.DataStructure.Internal.Algorithm.Glyphs;

namespace NetFocus.DataStructure.Internal.Algorithm
{

	public class ListInsertStatus
	{
		int i;
		char e;
		string l;
		int length;
		int j;
		string p = null;
		Color nodeColor;
		Color currentNodeColor;
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

		[Description("P指向当前节点的前驱结点.")]
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

		[Description("此属性代表要删除的位置.")]
		[Category("算法属性")]
		public int I
		{
			get
			{
				return i;
			}
			set{}
		}
		[Description("一个临时变量,用来查找被删节点的前驱.")]
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
		[Description("当前链表的长度.")]
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

		[Description("用来保存被删除的节点.")]
		[Category("算法属性")]
		public char E
		{
			get
			{
				return e;
			}
			set
			{
			}
		}
		
		[Description("表示当前链表.")]
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
		[Description("表示动画面板中当前P指针所指结点的颜色.")]
		[Category("动画属性")]
		public Color 当前结点颜色
		{
			get
			{
				return currentNodeColor;
			}
			set
			{
				currentNodeColor = value;
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

		public ListInsertStatus(string l,int i,char e)
		{
			this.l = l;
			this.i = i;
			this.e = e;
			this.length = l.Length;
			this.j = 0;
			this.p = null;
			nodeColor = Color.DarkTurquoise;
			currentNodeColor = Color.Pink;
			headNodeColor = Color.Red;
			insertNodeColor = Color.DarkOrange;
			canEdit = false;
		}
	}
}
