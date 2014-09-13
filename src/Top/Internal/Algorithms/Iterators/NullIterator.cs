using System;

namespace NetFocus.DataStructure.Internal.Algorithm.Glyphs
{
	/// <summary>
	/// 一个空的迭代器,当一个图形元素是叶子节点(即没有子节点)时会用到此迭代器
	/// </summary>
	public class NullIterator : IIterator
	{
		IGlyph glyph;
		
		public NullIterator(IGlyph glyph)
		{
			this.glyph = glyph;
		}
		
		#region IIterator 成员

		public IIterator First()
		{
			return null;
		}

		public IIterator Next()
		{
			return null;
		}

		public bool IsDone()
		{
			return true;//因为是叶子节点,所以总是返回真.
		}

		public IGlyph CurrentItem
		{
			get
			{
				return this.glyph;
			}
		}

		#endregion
	}
}
