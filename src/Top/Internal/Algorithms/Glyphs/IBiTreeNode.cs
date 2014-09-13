using System;
using System.Drawing;


namespace NetFocus.DataStructure.Internal.Algorithm.Glyphs
{
	/// <summary>
	/// 代表一棵二叉树中的一个结点
	/// </summary>
	public interface IBiTreeNode : IGlyph
	{
		/// <summary>
		/// 结点的文本
		/// </summary>
		string Text
		{
			get;
			set;
		}
		/// <summary>
		/// the left child of the current biTree node
		/// </summary>
		IBiTreeNode LeftChild
		{
			get;
			set;
		}

		/// <summary>
		/// the right child of the current biTree node
		/// </summary>
		IBiTreeNode RightChild
		{
			get;
			set;
		}
	}
}
