
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using NetFocus.DataStructure.TextEditor.Document;


namespace NetFocus.DataStructure.TextEditor
{
	public class GutterMargin : AbstractMargin
	{
		StringFormat numberStringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
		Point selectionStartPos;
		bool selectionComeFromGutter = false;
		public static Cursor RightLeftCursor;
		
		static GutterMargin()
		{
			Stream cursorStream = Assembly.GetCallingAssembly().GetManifestResourceStream("RightArrow.cur");
			RightLeftCursor = new Cursor(cursorStream);
			cursorStream.Close();
		}
		
		
		public override Cursor Cursor {
			get {
				return RightLeftCursor;
			}
		}
		
		public override Size Size {
			get {
				return new Size((int)(textArea.TextViewMargin.GetWidth('8') * Math.Max(3, (int)Math.Log10(textArea.Document.TotalNumberOfLines) + 1)),
				                -1);
			}
		}
		
		public override bool IsVisible {
			get {
				return textArea.TextEditorProperties.ShowLineNumbers;
			}
		}
		
		
		public GutterMargin(TextArea textArea) : base(textArea)
		{
			numberStringFormat.LineAlignment = StringAlignment.Far;
			numberStringFormat.FormatFlags   = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.FitBlackBox |
			                                    StringFormatFlags.NoWrap | StringFormatFlags.NoClip;
		}
		
		
		public override void OnPaint(Graphics g, Rectangle rect)
		{
			if (rect.Width <= 0 || rect.Height <= 0) {
				return;
			}
			HighlightColor lineNumberPainterColor = textArea.Document.HighlightingStrategy.GetEnvironmentColorForName("LineNumbers");
			int fontHeight = lineNumberPainterColor.Font.Height;
			Brush fillBrush = textArea.Enabled ? BrushRegistry.GetBrush(lineNumberPainterColor.BackgroundColor) : SystemBrushes.InactiveBorder;
			Brush drawBrush = BrushRegistry.GetBrush(lineNumberPainterColor.Color);
			
			for (int y = 0; y < (DrawingRectangle.Height + textArea.TextViewMargin.VisibleLineDrawingRemainder) / fontHeight + 1; ++y) {
				int ypos = DrawingRectangle.Y + fontHeight * y  - textArea.TextViewMargin.VisibleLineDrawingRemainder;
				Rectangle backgroundRectangle = new Rectangle(DrawingRectangle.X, ypos, DrawingRectangle.Width, fontHeight);
				if (rect.IntersectsWith(backgroundRectangle)) {
					g.FillRectangle(fillBrush, backgroundRectangle);
					int curLine = textArea.Document.GetFirstLogicalLine(textArea.Document.GetVisibleLine(textArea.TextViewMargin.FirstVisibleLine) + y);
					
					if (curLine < textArea.Document.TotalNumberOfLines) {
						g.DrawString((curLine + 1).ToString(), lineNumberPainterColor.Font, drawBrush, backgroundRectangle, numberStringFormat);
					}
				}
			}
		}
		
		
		//当鼠标点击某个行号时,调用这个方法.
		public override void OnMouseDown(Point mousepos, MouseButtons mouseButtons)
		{
			selectionComeFromGutter = true;
			int realline = textArea.TextViewMargin.GetLogicalLine(mousepos);
			if (realline >= 0 && realline < textArea.Document.TotalNumberOfLines) {
				selectionStartPos = new Point(0, realline);
				Point selectionEndPos = new Point(textArea.Document.GetLineSegment(realline).Length + 1, realline);
				textArea.SelectionManager.ClearSelection();
				textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, selectionStartPos, selectionEndPos));

				textArea.Caret.Position = selectionStartPos;
			}
		}
		
		public override void OnMouseLeave(EventArgs e)
		{
			selectionComeFromGutter = false;
		}
		
		//当鼠标在装订线上拖动时,调用这个方法(选中某些行)
		public override void OnMouseMove(Point mousepos, MouseButtons mouseButtons)
		{
			if (mouseButtons == MouseButtons.Left) {
				if (selectionComeFromGutter) {
					int realline       = textArea.TextViewMargin.GetLogicalLine(mousepos);
					Point realmousepos = new Point(0, realline);
					if (realmousepos.Y < textArea.Document.TotalNumberOfLines) {
						if (selectionStartPos.Y == realmousepos.Y) {
							textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, realmousepos, new Point(textArea.Document.GetLineSegment(realmousepos.Y).Length + 1, realmousepos.Y)));
						} else  if (selectionStartPos.Y < realmousepos.Y && textArea.SelectionManager.HasSomethingSelected) {
							textArea.SelectionManager.ExtendSelection(textArea.SelectionManager.SelectionCollection[0].EndPosition, realmousepos);
						} else {
							textArea.SelectionManager.ExtendSelection(textArea.Caret.Position, realmousepos);
						}
						textArea.Caret.Position = realmousepos;
					}
				} else {
					if (textArea.SelectionManager.HasSomethingSelected) {
						selectionStartPos  = textArea.Document.OffsetToPosition(textArea.SelectionManager.SelectionCollection[0].Offset);
						int realline       = textArea.TextViewMargin.GetLogicalLine(mousepos);
						Point realmousepos = new Point(0, realline);
						if (realmousepos.Y < textArea.Document.TotalNumberOfLines) {
							textArea.SelectionManager.ExtendSelection(textArea.Caret.Position, realmousepos);
						}
						textArea.Caret.Position = realmousepos;
					}
				}
			}
		}
	}
}
