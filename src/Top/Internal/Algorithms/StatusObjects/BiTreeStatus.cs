using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml;

using NetFocus.DataStructure.Internal.Algorithm.Glyphs;


namespace NetFocus.DataStructure.Internal.Algorithm
{
	public class BiTreeStatus
	{
		string p;

		Color traversedColor;
		Color commonColor;
		Color currentColor;
		Color visitingColor;
		bool canEdit;

		[Browsable(false)]
		public bool CanEdit
		{
			set
			{
				canEdit = value;	
			}
		}

		[Description("描述P当前指向的结点的值.")]
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
		[Description("表示动画面板中二叉树被遍历过结点的颜色.")]
		[Category("动画属性")]
		public Color 遍历过结点颜色
		{
			get
			{
				return traversedColor;
			}
			set
			{
				traversedColor = value;
			}
		}
		[Description("表示动画面板中二叉树结点的颜色.")]
		[Category("动画属性")]
		public Color 结点颜色
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
		[Description("表示动画面板中二叉树当前遍历结点的颜色.")]
		[Category("动画属性")]
		public Color 当前结点颜色
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
		[Description("表示动画面板中二叉树当前被输出结点的颜色.")]
		[Category("动画属性")]
		public Color 输出结点颜色
		{
			get
			{
				return visitingColor;
			}
			set
			{
				visitingColor = value;
			}
		}

		public BiTreeStatus()
		{
			this.p = null;
			traversedColor = Color.LightSkyBlue;
			commonColor = Color.HotPink;
			currentColor = Color.LightSeaGreen;
			visitingColor = Color.Gold;
			canEdit = false;
		}
	}
}
