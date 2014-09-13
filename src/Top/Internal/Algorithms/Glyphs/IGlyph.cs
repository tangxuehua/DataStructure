using System;
using System.Drawing;


namespace NetFocus.DataStructure.Internal.Algorithm.Glyphs
{
	public enum GlyphAppearance
	{
		Flat = 0,
		Popup = 1,
		Solid = 2
	}
	
	
	public interface IGlyph
	{
		/// <summary>
		/// 一个用来绘制自己的方法
		/// </summary>
		/// <param name="g"></param>
		void Draw(Graphics g);
		/// <summary>
		/// 当前图元对象的边框
		/// </summary>
		System.Drawing.Rectangle Bounds
		{
			get;
			set;
		}

		/// <summary>
		/// 当前图元对象的背景色
		/// </summary>
		Color BackColor
		{
			get;
			set;
		}

		/// <summary>
		/// 当前图元对象的外观,有Flat,Popup,Solid三种方式
		/// </summary>
		GlyphAppearance Appearance
		{
			get;
			set;
		}

		/// <summary>
		/// 标识某个点是否与当前图元相交
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		bool Intersects(Point p);
		/// <summary>
		/// 为当前图元对象的子图形对象创建一个迭代器
		/// </summary>
		/// <returns></returns>
		IIterator CreateIterator();

	}
}
