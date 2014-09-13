
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.InteropServices;
using System.Xml;
using System.Text;

using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.TextEditor.Actions;

 
namespace NetFocus.DataStructure.TextEditor
{
	/// <summary>
	/// This class represents a basic text editor control
	/// </summary>
	[ToolboxItem(true)]
	public class TextEditorControl : TextEditorControlBase
	{
		TextAreaControl textAreaControl;//声明一个文本区域控件变量.
		PrintDocument   printDocument = null;
		
		public PrintDocument PrintDocument {
			get {
				if (printDocument == null) {
					printDocument = new PrintDocument();
					printDocument.BeginPrint += new PrintEventHandler(this.BeginPrint);
					printDocument.PrintPage  += new PrintPageEventHandler(this.PrintPage);
				}
				return printDocument;
			}
		}
		
		
		public override TextAreaControl ActiveTextAreaControl {
			get {
				return textAreaControl;
			}
		}
		
		
		public TextEditorControl()
		{
			this.SetStyle(ControlStyles.ContainerControl, true);
			this.SetStyle(ControlStyles.Selectable, true);
			this.ResizeRedraw = true;

			Document = (new DocumentFactory()).CreateDefaultDocument();
			Document.HighlightingStrategy = HighlightingManager.Manager.HighlightingDefinitions["Default"] as DefaultHighlightingStrategy;
			textAreaControl  = new TextAreaControl(this);
			textAreaControl.Dock = DockStyle.Fill;
			InitializeTextAreaControl(textAreaControl);
			this.Controls.Add(textAreaControl);
			//注意:下面为Document.UpdateCommited事件添加了处理程序，
			//所以当Document.OnUpdateCommited()方法被调用到时，会调用这里注册的处理程序.
			//而Document.OnUpdateCommited()方法应该由这个类中的EndUpdate()方法来调用到。
			Document.UpdateCommited += new EventHandler(CommitUpdateRequested);

			TextEditorProperties = new DefaultTextEditorProperties();
			OptionsChanged();
		}
		
		//这个方法以后可以被继承自这个类的文本编辑器重写.
		protected virtual void InitializeTextAreaControl(TextAreaControl newControl)
		{
		}
		
		
		public override void OptionsChanged()
		{
			textAreaControl.OptionsChanged();
		}
		

		public override void EndUpdate()
		{
			base.EndUpdate();
			Document.OnUpdateCommited();
		}
		
		
		void CommitUpdateRequested(object sender, EventArgs e)
		{
			if (IsUpdating) 
			{
				return;
			}
			foreach (TextAreaUpdate update in Document.UpdateQueue) 
			{
				switch (update.TextAreaUpdateType) 
				{
					case TextAreaUpdateType.PositionToEnd:
						this.textAreaControl.TextArea.UpdateToEnd(update.Position.Y);
						break;
					case TextAreaUpdateType.PositionToLineEnd:
					case TextAreaUpdateType.SingleLine:
						this.textAreaControl.TextArea.UpdateLine(update.Position.Y);
						break;
					case TextAreaUpdateType.SinglePosition:
						this.textAreaControl.TextArea.UpdateLine(update.Position.Y, update.Position.X, update.Position.X);
						break;
					case TextAreaUpdateType.LinesBetween:
						this.textAreaControl.TextArea.UpdateLines(update.Position.X, update.Position.Y);
						break;
					case TextAreaUpdateType.WholeTextArea:
						this.textAreaControl.TextArea.Invalidate();
						break;
				}
			}
			Document.UpdateQueue.Clear();
			this.textAreaControl.TextArea.Update();
		}
		
		
		public bool EnableUndo {
			get {
				return Document.UndoStack.CanUndo;
			}
		}
		
		
		public bool EnableRedo {
			get {
				return Document.UndoStack.CanRedo;
			}
		}

		
		public void Undo()
		{
			if (Document.ReadOnly) {
				return;
			}
			if (Document.UndoStack.CanUndo) {
				BeginUpdate();
				Document.UndoStack.Undo();
				
				Document.UpdateQueue.Clear();
				Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
				
				this.textAreaControl.TextArea.UpdateMatchingBracket();
				
				EndUpdate();
			}
		}
		
		
		public void Redo()
		{
			if (Document.ReadOnly) {
				return;
			}
			if (Document.UndoStack.CanRedo) {
				BeginUpdate();
				Document.UndoStack.Redo();
				
				Document.UpdateQueue.Clear();
				Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
				
				this.textAreaControl.TextArea.UpdateMatchingBracket();
				
				EndUpdate();
			}
		}
		
		
		public void SetHighlighting(string name)
		{
			//Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyFromName(name);
			Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighterByName(name);

		}
		
		

        #region Printing routines

		int          curLineNr = 0;
		float        curTabIndent = 0;
		StringFormat printingStringFormat;
		
		void BeginPrint(object sender, PrintEventArgs ev)
		{
			curLineNr = 0;
			printingStringFormat = (StringFormat)System.Drawing.StringFormat.GenericTypographic.Clone();
			
			// 100 should be enough for everyone ...err ?
			float[] tabStops = new float[100];
			for (int i = 0; i < tabStops.Length; ++i) {
				tabStops[i] = TabIndent * textAreaControl.TextArea.TextViewMargin.GetWidth(' '); 
			}
			
			printingStringFormat.SetTabStops(0, tabStops);
		}
		
		void Advance(ref float x, ref float y, float maxWidth, float size, float fontHeight)
		{
			if (x + size < maxWidth) {
				x += size;
			} else {
				x  = curTabIndent;
				y += fontHeight;
			}
		}
		
		float MeasurePrintingHeight(Graphics g, LineSegment line, float maxWidth)
		{
			float xPos = 0;
			float yPos = 0;
			float fontHeight = Font.GetHeight(g);
//			bool  gotNonWhitespace = false;
			curTabIndent = 0;
			foreach (TextWord word in line.Words) {
				switch (word.Type) {
					case TextWordType.Space:
						Advance(ref xPos, ref yPos, maxWidth, textAreaControl.TextArea.TextViewMargin.GetWidth(' '), fontHeight);
//						if (!gotNonWhitespace) {
//							curTabIndent = xPos;
//						}
						break;
					case TextWordType.Tab:
						Advance(ref xPos, ref yPos, maxWidth, TabIndent * textAreaControl.TextArea.TextViewMargin.GetWidth(' '), fontHeight);
//						if (!gotNonWhitespace) {
//							curTabIndent = xPos;
//						}
						break;
					case TextWordType.Word:
//						if (!gotNonWhitespace) {
//							gotNonWhitespace = true;
//							curTabIndent    += TabIndent * textAreaControl.TextArea.TextView.GetWidth(' ');
//						}
						SizeF drawingSize = g.MeasureString(word.Word, word.Font, new SizeF(maxWidth, fontHeight * 100), printingStringFormat);
						Advance(ref xPos, ref yPos, maxWidth, drawingSize.Width, fontHeight);
						break;
				}
			}
			return yPos + fontHeight;
		}
		
		void DrawLine(Graphics g, LineSegment line, float yPos, RectangleF margin)
		{
			float xPos = 0;
			float fontHeight = Font.GetHeight(g);
//			bool  gotNonWhitespace = false;
			curTabIndent = 0 ;
			
			foreach (TextWord word in line.Words) {
				switch (word.Type) {
					case TextWordType.Space:
						Advance(ref xPos, ref yPos, margin.Width, textAreaControl.TextArea.TextViewMargin.GetWidth(' '), fontHeight);
//						if (!gotNonWhitespace) {
//							curTabIndent = xPos;
//						}
						break;
					case TextWordType.Tab:
						Advance(ref xPos, ref yPos, margin.Width, TabIndent * textAreaControl.TextArea.TextViewMargin.GetWidth(' '), fontHeight);
//						if (!gotNonWhitespace) {
//							curTabIndent = xPos;
//						}
						break;
					case TextWordType.Word:
//						if (!gotNonWhitespace) {
//							gotNonWhitespace = true;
//							curTabIndent    += TabIndent * textAreaControl.TextArea.TextView.GetWidth(' ');
//						}
						g.DrawString(word.Word, word.Font, BrushRegistry.GetBrush(word.Color), xPos + margin.X, yPos);
						SizeF drawingSize = g.MeasureString(word.Word, word.Font, new SizeF(margin.Width, fontHeight * 100), printingStringFormat);
						Advance(ref xPos, ref yPos, margin.Width, drawingSize.Width, fontHeight);
						break;
				}
			}
		}
		
		void PrintPage(object sender, PrintPageEventArgs ev)
		{
			Graphics g = ev.Graphics;
			float yPos = ev.MarginBounds.Top;
			
			while (curLineNr < Document.TotalNumberOfLines) {
				LineSegment curLine  = Document.GetLineSegment(curLineNr);
				if (curLine.Words != null) {
					float drawingHeight = MeasurePrintingHeight(g, curLine, ev.MarginBounds.Width);
					if (drawingHeight + yPos > ev.MarginBounds.Bottom) {
						break;
					}
					
					DrawLine(g, curLine, yPos, ev.MarginBounds);
					yPos += drawingHeight;
				}
				++curLineNr;
			}
			
			// If more lines exist, print another page.
			ev.HasMorePages = curLineNr < Document.TotalNumberOfLines;
		}


		#endregion

	}
}
