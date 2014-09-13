
using System;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using NetFocus.DataStructure.TextEditor.Document;

namespace NetFocus.DataStructure.TextEditor
{
	/// <summary>
	/// This class views the line numbers and folding markers.
	/// </summary>
	public class IconBarMargin : AbstractMargin
	{
		public override Size Size {
			get {
				return new Size((int)(textArea.TextViewMargin.FontHeight * 1.2f),
				                -1);
			}
		}
		
		
		public override bool IsVisible {
			get {
				return textArea.TextEditorProperties.IsIconBarVisible;
			}
		}
		
		
		public IconBarMargin(TextArea textArea) : base(textArea)
		{
		}
		
		
		public override void OnPaint(Graphics g, Rectangle rect)
		{
			if (rect.Width <= 0 || rect.Height <= 0) {
				return;
			}
			// paint background
			g.FillRectangle(SystemBrushes.Control, new Rectangle(DrawingRectangle.X, rect.Top, DrawingRectangle.Width - 1, rect.Height));
			g.DrawLine(SystemPens.ControlDark, base.DrawingRectangle.Right - 1, rect.Top, base.DrawingRectangle.Right - 1, rect.Bottom);
			
			// paint mark icons
			foreach (int mark in textArea.Document.BookmarkManager.Marks) {
				int lineNumber = textArea.Document.GetVisibleLine(mark);
				int yPos = (int)(lineNumber * textArea.TextViewMargin.FontHeight) - textArea.VirtualTop.Y;
				if (yPos >= rect.Y && yPos <= rect.Bottom) {
					DrawBookmark(g, yPos);
				}
			}
			base.OnPaint(g, rect);
		}
		
	
		public override void OnMouseDown(Point mousepos, MouseButtons mouseButtons)
		{
			int lineNumber = textArea.TextViewMargin.GetLogicalLine(mousepos);
			int yPos = (int)(lineNumber * textArea.TextViewMargin.FontHeight) - textArea.VirtualTop.Y;

			textArea.Document.BookmarkManager.ToggleMarkAt(lineNumber);
			textArea.Invalidate(drawingRectangle);
			
			base.OnMouseDown (mousepos, mouseButtons);
		}


	    void DrawBookmark(Graphics g, int y)
		{
			int delta = textArea.TextViewMargin.FontHeight / 8;
			Rectangle rect = new Rectangle(1, y + delta, textArea.TextViewMargin.FontHeight - delta * 2, textArea.TextViewMargin.FontHeight - delta * 2);
			FillRoundRect(g, Brushes.Red, rect);
			DrawRoundRect(g, Pens.Black, rect);
		}

		GraphicsPath CreateRoundRectGraphicsPath(Rectangle r)
		{
			GraphicsPath gp = new GraphicsPath();
			/*int radius = r.Width / 2;
			gp.AddLine(r.X + radius, r.Y, r.Right - radius, r.Y);
			gp.AddArc(r.Right - radius, r.Y, radius, radius, 270, 90);
			
			gp.AddLine(r.Right, r.Y + radius, r.Right, r.Bottom - radius);
			gp.AddArc(r.Right - radius, r.Bottom - radius, radius, radius, 0, 90);
			gp.AddLine(r.Right - radius, r.Bottom, r.X + radius, r.Bottom);
			gp.AddArc(r.X, r.Bottom - radius, radius, radius, 90, 90);
			
			gp.AddLine(r.X, r.Bottom - radius, r.X, r.Y + radius);
			gp.AddArc(r.X, r.Y, radius, radius, 180, 90);*/
			gp.AddEllipse(r.X,r.Y,r.Width-1 ,r.Height-1 );//Ìí¼ÓÒ»¸öÔ²È¦.
			
			gp.CloseFigure();
			return gp;
		}
		
		void DrawRoundRect(Graphics g, Pen p , Rectangle r)
		{
			using (GraphicsPath gp = CreateRoundRectGraphicsPath(r)) {
				g.DrawPath(p, gp);
				
			}
		}
		
		void FillRoundRect(Graphics g, Brush b , Rectangle r)
		{
			using (GraphicsPath gp = CreateRoundRectGraphicsPath(r)) {
				g.FillPath(b, gp);
				
			}
		}

		
	}
}
