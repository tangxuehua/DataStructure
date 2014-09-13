using System;

namespace NetFocus.DataStructure.Internal.Algorithm.Glyphs
{
	/// <summary>
	/// 一个迭代器用于以一种统一的方式来顺序读取一个复杂对象集合,而不管这个对象的内部结构
	/// </summary>
	public interface IIterator
	{
		/// <summary>
		/// 将迭代器的指针指向第一个元素
		/// </summary>
		/// <returns></returns>
		IIterator First();
		/// <summary>
		/// 将迭代器的指针指向下一个元素
		/// </summary>
		/// <returns></returns>
		IIterator Next();
		/// <summary>
		/// 标识是否完成了所有元素的迭代遍历
		/// </summary>
		/// <returns></returns>
		bool IsDone();
		/// <summary>
		/// 当前迭代器指针所指向的图形对象
		/// </summary>
		IGlyph CurrentItem
		{
			get;
		}
		
	}
}
