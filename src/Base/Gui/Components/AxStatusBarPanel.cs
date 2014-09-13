
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace NetFocus.DataStructure.Gui.Components
{
	public class AxStatusBarPanel : StatusBarPanel//StatusBar中的一个Panel
	{
		StringFormat sFormat = new StringFormat();
		
		public AxStatusBarPanel()
		{
			Style       = StatusBarPanelStyle.OwnerDraw;//自己绘制.
			BorderStyle = StatusBarPanelBorderStyle.None;//不显示边框.
		}
		
		//绘制面板的边框.
		protected virtual void DrawBorder(StatusBarDrawItemEventArgs drawEventArgs)
		{
			drawEventArgs.Graphics.DrawRectangle(SystemPens.ControlDark, 
			                                     new Rectangle(drawEventArgs.Bounds.X,
			                                                   drawEventArgs.Bounds.Y,
			                                                   drawEventArgs.Bounds.Width - 1,
			                                                   drawEventArgs.Bounds.Height - 1));
		}
				
		
		public virtual void DrawPanel(StatusBarDrawItemEventArgs drawEventArgs)
		{
			Graphics g = drawEventArgs.Graphics;
			switch (Alignment) {//判断文本的对其方式.
				case HorizontalAlignment.Left:
					sFormat.Alignment = StringAlignment.Near;
					break;
				case HorizontalAlignment.Center:
					sFormat.Alignment = StringAlignment.Center;
					break;
				case HorizontalAlignment.Right:
					sFormat.Alignment = StringAlignment.Far;
					break;
			}
			g.DrawString(Text,
			             drawEventArgs.Font,
			             SystemBrushes.ControlText, 
			             drawEventArgs.Bounds,
			             sFormat);
			DrawBorder(drawEventArgs);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				sFormat.Dispose();
			}
		}
	}
}
