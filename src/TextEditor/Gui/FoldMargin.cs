
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
	public class FoldMargin : AbstractMargin
	{
		int selectedFoldLine = -1;
		
		public override Size Size {
			get {
				return new Size((int)(textArea.TextViewMargin.FontHeight),
				                -1);
			}
		}
		
		public override bool IsVisible {
			get {
				return textArea.TextEditorProperties.EnableFolding;
			}
		}
		
		
		public FoldMargin(TextArea textArea) : base(textArea)
		{
		}
		
		
		public override void OnPaint(Graphics g, Rectangle rect)
		{
			if (rect.Width <= 0 || rect.Height <= 0) {
				return;
			}
			HighlightColor lineNumberPainterColor = textArea.Document.HighlightingStrategy.GetEnvironmentColorForName("LineNumbers");
			HighlightColor foldLineColor          = textArea.Document.HighlightingStrategy.GetEnvironmentColorForName("FoldLine");
			
			
			for (int y = 0; y < (DrawingRectangle.Height + textArea.TextViewMargin.VisibleLineDrawingRemainder) / textArea.TextViewMargin.FontHeight + 1; ++y) {
				Rectangle markerRectangle = new Rectangle(DrawingRectangle.X,
				                                          DrawingRectangle.Top + y * textArea.TextViewMargin.FontHeight - textArea.TextViewMargin.VisibleLineDrawingRemainder,
				                                          DrawingRectangle.Width,
				                                          textArea.TextViewMargin.FontHeight);
				
				if (rect.IntersectsWith(markerRectangle)) {
					// draw dotted separator line
					if (textArea.Document.TextEditorProperties.ShowLineNumbers) {
						g.FillRectangle(BrushRegistry.GetBrush(textArea.Enabled ? lineNumberPainterColor.BackgroundColor : SystemColors.InactiveBorder),
						                new Rectangle(markerRectangle.X + 1, markerRectangle.Y, markerRectangle.Width - 1, markerRectangle.Height));
						
						g.DrawLine(BrushRegistry.GetDotPen(lineNumberPainterColor.Color, lineNumberPainterColor.BackgroundColor),
						           base.DrawingRectangle.X,
						           markerRectangle.Y,
						           base.DrawingRectangle.X,
						           markerRectangle.Bottom);
					} else {
						g.FillRectangle(BrushRegistry.GetBrush(textArea.Enabled ? lineNumberPainterColor.BackgroundColor : SystemColors.InactiveBorder), markerRectangle);
					}
					
					int currentLine = textArea.Document.GetFirstLogicalLine(textArea.Document.GetVisibleLine(textArea.TextViewMargin.FirstVisibleLine) + y);
					PaintFoldMarker(g, currentLine, markerRectangle);
				}
			}
		}
		
		
		bool SelectedFoldingFrom(ArrayList list)
		{
			if (list != null) {
				for (int i = 0; i < list.Count; ++i) {
					if (this.selectedFoldLine == ((FoldMarker)list[i]).StartLine) {
						return true;
					}
				}
			}
			return false;
		}
		
		void PaintFoldMarker(Graphics g, int lineNumber, Rectangle drawingRectangle)
		{
			HighlightColor foldLineColor    = textArea.Document.HighlightingStrategy.GetEnvironmentColorForName("FoldLine");
			HighlightColor selectedFoldLine = textArea.Document.HighlightingStrategy.GetEnvironmentColorForName("SelectedFoldLine");
			
			ArrayList foldingsWithStart = textArea.Document.FoldingManager.GetFoldingsWithStart(lineNumber);
			ArrayList foldingsBetween   = textArea.Document.FoldingManager.GetFoldingsContainsLineNumber(lineNumber);
			ArrayList foldingsWithEnd   = textArea.Document.FoldingManager.GetFoldingsWithEnd(lineNumber);
			
			bool isFoldStart = foldingsWithStart.Count > 0; 
			bool isBetween   = foldingsBetween.Count > 0;
			bool isFoldEnd   = foldingsWithEnd.Count > 0;
			
			bool isStartSelected   = SelectedFoldingFrom(foldingsWithStart);
			bool isBetweenSelected = SelectedFoldingFrom(foldingsBetween);
			bool isEndSelected     = SelectedFoldingFrom(foldingsWithEnd);
			
			int foldMarkerSize = (int)Math.Round(textArea.TextViewMargin.FontHeight * 0.57f);
			foldMarkerSize -= (foldMarkerSize) % 2;
			int foldMarkerYPos = drawingRectangle.Y + (int)((drawingRectangle.Height - foldMarkerSize) / 2);
			int xPos = drawingRectangle.X + (drawingRectangle.Width - foldMarkerSize) / 2 + foldMarkerSize / 2;
			
			
			if (isFoldStart) {
				bool isVisible         = true;
				bool moreLinedOpenFold = false;
				foreach (FoldMarker foldMarker in foldingsWithStart) {
					if (foldMarker.IsFolded) {
						isVisible = false;
					} else {
						moreLinedOpenFold = foldMarker.EndLine > foldMarker.StartLine;
					}
				}
				
				bool isFoldEndFromUpperFold = false;
				foreach (FoldMarker foldMarker in foldingsWithEnd) {
					if (foldMarker.EndLine > foldMarker.StartLine && !foldMarker.IsFolded) {
						isFoldEndFromUpperFold = true;
					} 
				}
				
				DrawFoldMarker(g, new RectangleF(drawingRectangle.X + (drawingRectangle.Width - foldMarkerSize) / 2,
				                                 foldMarkerYPos,
				                                 foldMarkerSize,
				                                 foldMarkerSize),
				                  isVisible,
				                  isStartSelected
				                  );
				
				// draw line above fold marker
				if (isBetween || isFoldEndFromUpperFold) {
					g.DrawLine(BrushRegistry.GetPen(isBetweenSelected ? selectedFoldLine.Color : foldLineColor.Color),
					           xPos,
					           drawingRectangle.Top,
					           xPos,
					           foldMarkerYPos - 1);
				}
				
				// draw line below fold marker
				if (isBetween || moreLinedOpenFold) {
					g.DrawLine(BrushRegistry.GetPen(isEndSelected || (isStartSelected && isVisible) || isBetweenSelected ? selectedFoldLine.Color : foldLineColor.Color),
					           xPos,
					           foldMarkerYPos + foldMarkerSize + 1,
					           xPos,
					           drawingRectangle.Bottom);
				}
			} else {
				if (isFoldEnd) {
					int midy = drawingRectangle.Top + drawingRectangle.Height / 2;
					// draw line above fold end marker
					g.DrawLine(BrushRegistry.GetPen(isBetweenSelected || isEndSelected ? selectedFoldLine.Color : foldLineColor.Color),
					           xPos,
					           drawingRectangle.Top,
					           xPos,
					           midy);
					
					// draw fold end marker
					g.DrawLine(BrushRegistry.GetPen(isBetweenSelected || isEndSelected ? selectedFoldLine.Color : foldLineColor.Color),
					           xPos,
					           midy,
					           xPos + foldMarkerSize / 2,
					           midy);
					// draw line below fold end marker
					if (isBetween) {
						g.DrawLine(BrushRegistry.GetPen(isBetweenSelected ? selectedFoldLine.Color : foldLineColor.Color),
						           xPos,
						           midy + 1,
						           xPos,
						           drawingRectangle.Bottom);
					}
				} else if (isBetween) {
					// just draw the line :)
					g.DrawLine(BrushRegistry.GetPen(isBetweenSelected ? selectedFoldLine.Color : foldLineColor.Color),
					           xPos,
					           drawingRectangle.Top,
					           xPos,
					           drawingRectangle.Bottom);
				}
			}
		}
		
		public override void OnMouseMove(Point mousepos, MouseButtons mouseButtons)
		{
			bool  showFolding  = textArea.Document.TextEditorProperties.EnableFolding;
			int   physicalLine = + (int)((mousepos.Y + textArea.VirtualTop.Y) / textArea.TextViewMargin.FontHeight);
			int   realline     = textArea.Document.GetFirstLogicalLine(physicalLine);
			
			textArea.Focus();
			
			if (!showFolding || realline < 0 || realline + 1 >= textArea.Document.TotalNumberOfLines) {
				return;
			}
			
			ArrayList foldMarkers = textArea.Document.FoldingManager.GetFoldingsWithStart(realline);
			int oldSelection = selectedFoldLine;
			if (foldMarkers.Count > 0) {
				selectedFoldLine = realline;
			} else {
				selectedFoldLine = -1;
			}
			if (oldSelection != selectedFoldLine) {
				textArea.Refresh(this);
			}
		}
		
		public override void OnMouseDown(Point mousepos, MouseButtons mouseButtons)
		{
			bool  showFolding  = textArea.Document.TextEditorProperties.EnableFolding;
			int   physicalLine = + (int)((mousepos.Y + textArea.VirtualTop.Y) / textArea.TextViewMargin.FontHeight);
			int   realline     = textArea.Document.GetFirstLogicalLine(physicalLine);
			
			textArea.Focus();
			
			if (!showFolding || realline < 0 || realline + 1 >= textArea.Document.TotalNumberOfLines) {
				return;
			}
			
			ArrayList foldMarkers = textArea.Document.FoldingManager.GetFoldingsWithStart(realline);
			foreach (FoldMarker fm in foldMarkers) {
				fm.IsFolded = !fm.IsFolded;
			}
			textArea.MotherTextAreaControl.AdjustScrollBars(null, null);
			textArea.Refresh();
		}
		
		public override void OnMouseLeave(EventArgs e)
		{
			if (selectedFoldLine != -1) {
				selectedFoldLine = -1;
				textArea.Refresh(this);
			}
		}
		

		void DrawFoldMarker(Graphics g, RectangleF rectangle, bool isOpened, bool isSelected)
		{
			HighlightColor foldMarkerColor = textArea.Document.HighlightingStrategy.GetEnvironmentColorForName("FoldMarker");
			HighlightColor foldLineColor   = textArea.Document.HighlightingStrategy.GetEnvironmentColorForName("FoldLine");
			HighlightColor selectedFoldLine = textArea.Document.HighlightingStrategy.GetEnvironmentColorForName("SelectedFoldLine");
			
			Rectangle intRect = new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
			g.FillRectangle(BrushRegistry.GetBrush(foldMarkerColor.BackgroundColor), intRect);
			g.DrawRectangle(BrushRegistry.GetPen(isSelected ? selectedFoldLine.Color : foldMarkerColor.Color), intRect);
			
			int space  = (int)Math.Round(((double)rectangle.Height) / 8d) + 1;
			int mid    = intRect.Height / 2 + intRect.Height % 2;			
			
			g.DrawLine(BrushRegistry.GetPen(foldLineColor.BackgroundColor), 
			           rectangle.X + space, 
			           rectangle.Y + mid, 
			           rectangle.X + rectangle.Width - space, 
			           rectangle.Y + mid);
			
			if (!isOpened) {
				g.DrawLine(BrushRegistry.GetPen(foldLineColor.BackgroundColor), 
				           rectangle.X + mid, 
				           rectangle.Y + space, 
				           rectangle.X + mid, 
				           rectangle.Y + rectangle.Height - space);
			}
		}

	}
}
