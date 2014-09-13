using System;
using System.Collections;
using System.Drawing;

namespace NetFocus.DataStructure.Internal.Algorithm.Glyphs
{
	/// <summary>
	/// 一个顺序数据结构(如数组)的迭代器(封装一个顺序的数据结构)
	/// </summary>
	public class ArrayIterator : IIterator
	{
		ArrayList glyphArray = null;
		int currentIndex = -1;

		public ArrayIterator(ArrayList glyphArray)
		{
			this.glyphArray = glyphArray;
		}
		
		#region IIterator 成员

		public IIterator First()
		{
			if(glyphArray.Count > 0)
			{
				currentIndex = 0;
			}
			else
			{
			    currentIndex = -1;
			}
			return this;
		}

		public IIterator Next()
		{
			currentIndex += 1;
			return this;
		}

		public bool IsDone()
		{
			return currentIndex == glyphArray.Count || currentIndex == -1;
		}

		public IGlyph CurrentItem
		{
			get
			{
				if(currentIndex >= 0 && currentIndex < glyphArray.Count)
				{
					return (IGlyph)glyphArray[currentIndex];
				}
				return null;
			}

		}

		#endregion

		public int Count
		{
			get
			{
				return glyphArray.Count;
			}
		}

		public IGlyph GetGlyphByIndex(int index)
		{
			if(index >= 0 && index < glyphArray.Count)
			{
				return (IGlyph)glyphArray[index];
			}
			return null;

		}

		
		public void MoveGlyphsHorizon(int distance)
		{
			for(IIterator iterator = this.First();!this.IsDone();iterator = this.Next())
			{
				int x = iterator.CurrentItem.Bounds.X + distance;
				int y = iterator.CurrentItem.Bounds.Y;
				int width = iterator.CurrentItem.Bounds.Width;
				int height = iterator.CurrentItem.Bounds.Height;
				iterator.CurrentItem.Bounds = new Rectangle(x,y,width,height);
			}
		}
		public void MoveGlyphHorizon(int glyphIndex,IGlyph insertGlyph,int direction)
		{
			if(direction == 1)//说明向右移
			{
				glyphArray.RemoveAt(glyphIndex + 1);
				glyphArray.Insert(glyphIndex + 1,insertGlyph); 					
			}
			else if(direction == 0)//说明向左移
			{
				glyphArray.RemoveAt(glyphIndex - 1);
				glyphArray.Insert(glyphIndex - 1,insertGlyph); 
			}
			
		}
		
		
		public void MoveGlyphVertical(IGlyph insertingGlyph,int yPosition)
		{
			insertingGlyph.Bounds = new Rectangle(insertingGlyph.Bounds.X,yPosition,insertingGlyph.Bounds.Width,insertingGlyph.Bounds.Height);

		}


		public void InsertGlyph(IGlyph glyph)
		{
			this.glyphArray.Add(glyph);
		}
		

		public void DeleteGlyph(int index)
		{
			if(index >= 0 && index < glyphArray.Count)
			{
				this.glyphArray.RemoveAt(index);
			}
		}
		
		
		public void SetBackColor(int index,Color backColor,Color refreshColor,bool refreshOthers)
		{
			if(refreshOthers == true)
			{
				foreach(IGlyph glyph in glyphArray)
				{
					glyph.BackColor = refreshColor;
				}
			}
			if(index >= 0 && index < glyphArray.Count)
			{
				((IGlyph)glyphArray[index]).BackColor = backColor;
			}
		}


		public void SetElementsBackColor(ArrayList indexArray,Color backColor,Color refreshColor)
		{
			for(int j = 1;j < glyphArray.Count;j++)
			{
				((IGlyph)glyphArray[j]).BackColor = refreshColor;
			}
			for(int i = 0;i < indexArray.Count;i++)
			{
				if((int)indexArray[i] < glyphArray.Count)
				{
					((IGlyph)glyphArray[(int)indexArray[i]]).BackColor = backColor;
				}
			}
		}


	}
}
