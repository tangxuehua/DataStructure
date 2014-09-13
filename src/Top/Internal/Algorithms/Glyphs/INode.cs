using System;
using System.Drawing;

namespace NetFocus.DataStructure.Internal.Algorithm.Glyphs
{
	/// <summary>
	/// 代表一个链表中的节点对象
	/// </summary>
	public interface INode : IGlyph
	{
		INode Next
		{
			get;
			set;
		}
		string Text
		{
			get;
			set;
		}
	}
}
