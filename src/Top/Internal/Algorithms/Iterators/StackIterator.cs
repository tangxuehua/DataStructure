using System;
using System.Collections;
using System.Drawing;

namespace NetFocus.DataStructure.Internal.Algorithm.Glyphs
{
	/// <summary>
	/// 一个堆栈数据结构的迭代器(封装一个堆栈的数据结构)
	/// </summary>
	public class StackIterator : IIterator
	{
		ArrayList arrayList = null;
		int currentIndex = -1;

		public StackIterator(ArrayList arrayList)
		{
			this.arrayList = arrayList;
		}

		
		#region IIterator 成员

		public IIterator First()
		{
			if(arrayList.Count > 0)
			{
				currentIndex = 0;  //栈底元素为第一个元素
			}
			else
			{
				currentIndex = -1;
			}
			return this;
		}

		public IIterator Next()
		{
			currentIndex += 1;  //指针加一
			return this;
		}

		public bool IsDone()
		{
			return currentIndex >= arrayList.Count || currentIndex == -1;
		}

		public IGlyph CurrentItem
		{
			get
			{
				if(currentIndex >= 0 && currentIndex < arrayList.Count)
				{
					return (IGlyph)arrayList[currentIndex];
				}
				return null;
			}
		}

		#endregion

		public void RefreshItems(int newWidth,int newHeight)
		{
			if(newWidth <= 0 || newHeight <= 0)
			{
				return;
			}
			int y = 0;
			for(int i = 0;i < arrayList.Count;i++)
			{
				y = newHeight - (i + 1) * (2 + 28) - 1;
				IGlyph glyph = ((IGlyph)arrayList[i]);
				glyph.Bounds = new Rectangle(glyph.Bounds.X,y,newWidth,glyph.Bounds.Height);
			}
		}
		public void PushGlyph(IGlyph glyph)
		{
			arrayList.Add(glyph);
		}
		public void Pop()
		{
			arrayList.RemoveAt(arrayList.Count -1);
		}
		public int ItemCount
		{
			get
			{
				return arrayList.Count;
			}
		}

	}
}
