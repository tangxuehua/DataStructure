
using System;
using System.Collections;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using NetFocus.DataStructure.TextEditor.Actions;
using NetFocus.DataStructure.TextEditor.Document;

using DevComponents.DotNetBar;

using NetFocus.Components.AddIns;


namespace NetFocus.DataStructure.TextEditor
{
	public delegate bool KeyEventHandler(char ch);
	public delegate bool DialogKeyProcessor(Keys keyData);
	
	/// <summary>
	/// This class paints the textarea.
	/// </summary>
	// 注意:这个类对于文本编辑器来说是最重要的,集成了所有其它的类.
	[ToolboxItem(false)]
	public class TextArea : UserControl
	{
		public static bool HiddenMouseCursor = false;
		readonly static string contextMenuPath       = "/DataStructure/ViewContent/TextEditor/ContextMenu";

		Point virtualTop        = new Point(0, 0);
		TextAreaControl         motherTextAreaControl;
		TextEditorControl       motherTextEditorControl;

		DotNetBarManager dotNetBarManager1 = new DotNetBarManager();
		
		ArrayList                 bracketshemes  = new ArrayList();
		TextAreaClipboardHandler  textAreaClipboardHandler;
		bool autoClearSelection = false;
		
		ArrayList  leftMargins = new ArrayList();
		ArrayList  topMargins  = new ArrayList();
		
		TextViewMargin      textViewMargin;
		GutterMargin  gutterMargin;
		FoldMargin    foldMargin;
		IconBarMargin iconBarMargin;
		
		SelectionManager selectionManager;
		Caret            caret;
		
		public TextEditorControl MotherTextEditorControl {
			get {
				return motherTextEditorControl;
			}
		}
		
		public TextAreaControl MotherTextAreaControl {
			get {
				return motherTextAreaControl;
			}
		}
		
		public SelectionManager SelectionManager {
			get {
				return selectionManager;
			}
		}
		
		public Caret Caret {
			get {
				return caret;
			}
		}
		
		public TextViewMargin TextViewMargin {
			get {
				return textViewMargin;
			}
		}
		
		public GutterMargin GutterMargin {
			get {
				return gutterMargin;
			}
		}
		
		public FoldMargin FoldMargin {
			get {
				return foldMargin;
			}
		}
		
		public IconBarMargin IconBarMargin {
			get {
				return iconBarMargin;
			}
		}
		
		public Encoding Encoding {
			get {
				return motherTextEditorControl.Encoding;
			}
		}
		public int MaxVScrollValue {
			get {
				return (Document.GetVisibleLine(Document.TotalNumberOfLines - 1) + 1 + TextViewMargin.VisibleLineCount * 2 / 3) * Document.TextEditorProperties.Font.Height;
			}
		}
		
		public Point VirtualTop {
			get {
				return virtualTop;
			}
			set {
				Point newVirtualTop = new Point(value.X, Math.Min(MaxVScrollValue, Math.Max(0, value.Y)));
				if (virtualTop != newVirtualTop) {
					virtualTop = newVirtualTop;
					motherTextAreaControl.VScrollBar.Value = virtualTop.Y;
					
					Invalidate();
				}
			}
		}
		
		public bool AutoClearSelection {
			get {
				return autoClearSelection;
			}
			set {
				autoClearSelection = value;
			}
		}
		
		[Browsable(false)]
		public IDocument Document {
			get {
				return motherTextEditorControl.Document;
			}
		}
		
		public TextAreaClipboardHandler ClipboardHandler {
			get {
				return textAreaClipboardHandler;
			}
		}
		
		public ITextEditorProperties TextEditorProperties {
			get {
				return motherTextEditorControl.TextEditorProperties;
			}
		}
		
		//以上全部为一些简单的属性.
		
		//构造函数
		public TextArea(TextEditorControl motherTextEditorControl, TextAreaControl motherTextAreaControl)
		{
			this.motherTextAreaControl      = motherTextAreaControl;
			this.motherTextEditorControl    = motherTextEditorControl;
			
			caret            = new Caret(this);//附加插入符.
			selectionManager = new SelectionManager(Document);//附加选择管理器.
						
			ResizeRedraw = true;
			
			SetStyle(ControlStyles.DoubleBuffer, false);
			SetStyle(ControlStyles.Opaque, false);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.Selectable, true);
			
			textViewMargin = new TextViewMargin(this);//附加主要的文本区域。
			gutterMargin = new GutterMargin(this);//附加装订线区域.
			foldMargin   = new FoldMargin(this);//附加折叠区域.
			iconBarMargin = new IconBarMargin(this);//附加图标栏区域.
			leftMargins.AddRange(new AbstractMargin[] { iconBarMargin, gutterMargin, foldMargin });
			
			OptionsChanged();
			
			textAreaClipboardHandler = new TextAreaClipboardHandler(this);//附加剪切办处理程序.
			new TextAreaMouseHandler(this).Attach();
			new TextAreaDragDropHandler().Attach(this);
			
			bracketshemes.Add(new BracketHighlightingScheme('{', '}'));
			bracketshemes.Add(new BracketHighlightingScheme('(', ')'));
			bracketshemes.Add(new BracketHighlightingScheme('[', ']'));
			
			caret.PositionChanged += new EventHandler(SearchMatchingBracket);
			Document.TextContentChanged += new EventHandler(TextContentChanged);
			Document.FoldingManager.FoldingsChanged += new EventHandler(DocumentFoldingsChanged);
		}
		
		
		void SearchMatchingBracket(object sender, EventArgs e)
		{
			if (!TextEditorProperties.ShowMatchingBracket) 
			{
				textViewMargin.Highlight = null;
				return;
			}
			bool changed = false;
			if (caret.Offset == 0) 
			{
				if (textViewMargin.Highlight != null) 
				{
					int line  = textViewMargin.Highlight.OpenBrace.Y;
					int line2 = textViewMargin.Highlight.CloseBrace.Y;
					textViewMargin.Highlight = null;
					UpdateLine(line);
					UpdateLine(line2);
				}
				return;
			}
			foreach (BracketHighlightingScheme bracketsheme in bracketshemes) 
			{
				//				if (bracketsheme.IsInside(textareapainter.Document, textareapainter.Document.Caret.Offset)) {
				Highlight highlight = bracketsheme.GetHighlight(Document, Caret.Offset - 1);
				if (textViewMargin.Highlight != null && textViewMargin.Highlight.OpenBrace.Y >=0 && textViewMargin.Highlight.OpenBrace.Y < Document.TotalNumberOfLines) 
				{
					UpdateLine(textViewMargin.Highlight.OpenBrace.Y);
				}
				if (textViewMargin.Highlight != null && textViewMargin.Highlight.CloseBrace.Y >=0 && textViewMargin.Highlight.CloseBrace.Y < Document.TotalNumberOfLines) 
				{
					UpdateLine(textViewMargin.Highlight.CloseBrace.Y);
				}
				textViewMargin.Highlight = highlight;
				if (highlight != null) 
				{
					changed = true;
					break; 
				}
				//				}
			}
			if (changed || textViewMargin.Highlight != null) 
			{
				int line = textViewMargin.Highlight.OpenBrace.Y;
				int line2 = textViewMargin.Highlight.CloseBrace.Y;
				if (!changed) 
				{
					textViewMargin.Highlight = null;
				}
				UpdateLine(line);
				UpdateLine(line2);
			}
		}
		
		public void UpdateMatchingBracket()
		{
			SearchMatchingBracket(null, null);
		}
		
		void TextContentChanged(object sender, EventArgs e)
		{
			Caret.Position = new Point(0, 0);
			SelectionManager.SelectionCollection.Clear();
		}
		
		void DocumentFoldingsChanged(object sender, EventArgs e)
		{
			this.motherTextAreaControl.AdjustScrollBars(null, null);
		}
		
		
		public void SetDesiredColumn()
		{
			Caret.DesiredColumn = TextViewMargin.GetDrawingXPos(Caret.Line, Caret.Column) + (int)(VirtualTop.X * textViewMargin.GetWidth(' '));
		}
		
		public void SetCaretToDesiredColumn(int caretLine)
		{
			Caret.Position = textViewMargin.GetLogicalColumn(Caret.Line, Caret.DesiredColumn + (int)(VirtualTop.X * textViewMargin.GetWidth(' ')));
		}
		
		public void OptionsChanged()
		{
			UpdateMatchingBracket();
			textViewMargin.OptionsChanged();
			caret.RecreateCaret();
			caret.UpdateCaretPosition();
			Refresh();
		}
		
		
		AbstractMargin lastMouseInMargin;
		protected override void OnMouseLeave(System.EventArgs e)
		{
			base.OnMouseLeave(e);
			this.Cursor = Cursors.Default;
			if (lastMouseInMargin != null) {
				lastMouseInMargin.OnMouseLeave(EventArgs.Empty);
				lastMouseInMargin = null;
			}
		}
		
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if(e.Button == MouseButtons.Left)
			{
				foreach (AbstractMargin margin in leftMargins) 
				{
					if (margin.DrawingRectangle.Contains(e.X, e.Y)) 
					{
						margin.OnMouseDown(new Point(e.X, e.Y), e.Button);
					}
				}
			}
		}
		
		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseMove(e);
			foreach (AbstractMargin margin in leftMargins) {
				if (margin.DrawingRectangle.Contains(e.X, e.Y)) {
					this.Cursor = margin.Cursor;
					margin.OnMouseMove(new Point(e.X, e.Y), e.Button);
					if (lastMouseInMargin != margin) {
						if (lastMouseInMargin != null) {
							lastMouseInMargin.OnMouseLeave(EventArgs.Empty);
						}
						lastMouseInMargin = margin;
					}
					return;
				}
			}
			if (lastMouseInMargin != null) {
				lastMouseInMargin.OnMouseLeave(EventArgs.Empty);
				lastMouseInMargin = null;
			}
			if (textViewMargin.DrawingRectangle.Contains(e.X, e.Y)) {
				this.Cursor = textViewMargin.Cursor;
				return;
			}
			this.Cursor = Cursors.Default;
		}
		
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			ButtonItem[] contextMenu = null;
			if (e.Button == MouseButtons.Right) 
			{
				try
				{
					contextMenu = (ButtonItem[])(AddInTreeSingleton.AddInTree.GetTreeNode(contextMenuPath).BuildChildItems(this)).ToArray(typeof(ButtonItem));
				}
				catch
				{
					return;
				}
				ButtonItem item = new ButtonItem();
				item.SubItems.AddRange(contextMenu);
				dotNetBarManager1.RegisterPopup(item);
				Control ctrl=this as Control;
				Point p=this.PointToScreen(new Point(e.X, e.Y));
				item.PopupMenu(p);
				
			} 

		}
		
		
		AbstractMargin updateMargin = null;
		public void Refresh(AbstractMargin margin)
		{
			updateMargin = margin;
			Invalidate(updateMargin.DrawingRectangle);
			Update();
			updateMargin = null;
		}
		
		
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			//重写该方法，从而在移动文本时控件不会闪烁。
			//base.OnPaintBackground (pevent);
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			int currentXPos = 0;
			int currentYPos = 0;
			bool adjustScrollBars = false;
			Graphics  g             = e.Graphics;
			Rectangle clipRectangle = e.ClipRectangle;
			
			
			if (updateMargin != null) 
			{
				try 
				{
					updateMargin.OnPaint(g, updateMargin.DrawingRectangle);
				} 
				catch (Exception ex) 
				{
					Console.WriteLine("Got exception : " + ex);
				}
				//clipRectangle.Intersect(updateMargin.DrawingRectangle);
			}
   			
			if (clipRectangle.Width <= 0 || clipRectangle.Height <= 0) 
			{
				return;
			}
			
			if (this.TextEditorProperties.UseAntiAliasedFont) 
			{
				g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
			} 
			else 
			{
				g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
			}
			//以下开始画TextArea中的各个区域,同时制定各个区域的Rectangle大小.
			foreach (AbstractMargin margin in leftMargins) 
			{
				if (margin.IsVisible) 
				{
					Rectangle marginRectangle = new Rectangle(currentXPos , currentYPos, margin.Size.Width, Height - currentYPos);
					if (marginRectangle != margin.DrawingRectangle) 
					{
						adjustScrollBars = true;
						margin.DrawingRectangle = marginRectangle;
					}
					currentXPos += margin.DrawingRectangle.Width;//为画下一个margin提供正确的x坐标。
					if (clipRectangle.IntersectsWith(marginRectangle)) 
					{
						marginRectangle.Intersect(clipRectangle);
						if (!marginRectangle.IsEmpty) 
						{
							try 
							{
								margin.OnPaint(g, marginRectangle);
							} 
							catch (Exception ex) 
							{
								Console.WriteLine("Got exception : " + ex);
							}
						}
					}
				}
			}
			
			Rectangle textViewMarginArea = new Rectangle(currentXPos, currentYPos, Width - currentXPos, Height - currentYPos);
			if (textViewMarginArea != textViewMargin.DrawingRectangle) 
			{
				adjustScrollBars = true;
				textViewMargin.DrawingRectangle = textViewMarginArea;
			}
			if (clipRectangle.IntersectsWith(textViewMarginArea)) 
			{
				textViewMarginArea.Intersect(clipRectangle);
				if (!textViewMarginArea.IsEmpty) 
				{
					try 
					{
						textViewMargin.OnPaint(g, textViewMarginArea);
					} 
					catch (Exception ex) 
					{
						Console.WriteLine("Got exception : " + ex);
					}
				}
			}
			
			if (adjustScrollBars) 
			{
				try 
				{
					this.motherTextAreaControl.AdjustScrollBars(null, null);
				} 
				catch (Exception) {}
			}
			
			try 
			{
				Caret.UpdateCaretPosition();
			} 
			catch (Exception) {}
			
			base.OnPaint(e);
		}


		public void ScrollToCaret()
		{
			motherTextAreaControl.ScrollToCaret();
		}
		
		public void ScrollTo(int line)
		{
			motherTextAreaControl.ScrollTo(line);
		}
		
		public void BeginUpdate()
		{
			motherTextEditorControl.BeginUpdate();
		}
		
		public void EndUpdate()
		{
			motherTextEditorControl.EndUpdate();
		}
		

		string GenerateWhitespaceString(int length)
		{
			return new String(' ', length);
		}
		/// <remarks>
		/// Inserts a single character at the caret position
		/// </remarks>
		public void InsertChar(char ch)
		{
			bool updating = motherTextEditorControl.IsUpdating;
			if (!updating) {
				BeginUpdate();
			}
			
			// filter out forgein whitespace chars and replace them with standard space (ASCII 32)
			if (Char.IsWhiteSpace(ch) && ch != '\t' && ch != '\n') {
				ch = ' ';
			}
			bool removedText = false;
			if (Document.TextEditorProperties.DocumentSelectionMode == DocumentSelectionMode.Normal &&
			    SelectionManager.SelectionCollection.Count > 0) {
				Caret.Position = SelectionManager.SelectionCollection[0].StartPosition;
				SelectionManager.RemoveSelectedText();
				removedText = true;
			}
			LineSegment caretLine = Document.GetLineSegment(Caret.Line);
			int offset = Caret.Offset;
			// use desired column for generated whitespaces
			int dc=Math.Min(Caret.Column,Caret.DesiredColumn);
			if (caretLine.Length < dc && ch != '\n') {
				Document.Insert(offset, GenerateWhitespaceString(dc - caretLine.Length) + ch);
			} else {
				Document.Insert(offset, ch.ToString());
			}
			++Caret.Column;
			
			if (removedText) {
				Document.UndoStack.UndoLast(2);
			}
			
			if (!updating) {
				EndUpdate();
				UpdateLineToEnd(Caret.Line, Caret.Column);
			}
		}
		
		/// <remarks>
		/// Inserts a whole string at the caret position
		/// </remarks>
		public void InsertString(string str)
		{
			bool updating = motherTextEditorControl.IsUpdating;
			if (!updating) {
				BeginUpdate();
			}
			try {
				bool removedText = false;
				if (Document.TextEditorProperties.DocumentSelectionMode == DocumentSelectionMode.Normal &&
				    SelectionManager.SelectionCollection.Count > 0) {
					Caret.Position = SelectionManager.SelectionCollection[0].StartPosition;
					SelectionManager.RemoveSelectedText();
					removedText = true;
				}
				
				int oldOffset = Document.PositionToOffset(Caret.Position);
				int oldLine   = Caret.Line;
				LineSegment caretLine = Document.GetLineSegment(Caret.Line);
				if (caretLine.Length < Caret.Column) {
					int whiteSpaceLength = Caret.Column - caretLine.Length;
					Document.Insert(oldOffset, GenerateWhitespaceString(whiteSpaceLength) + str);
					Caret.Position = Document.OffsetToPosition(oldOffset + str.Length + whiteSpaceLength);
				} else {
					Document.Insert(oldOffset, str);
					Caret.Position = Document.OffsetToPosition(oldOffset + str.Length);
				}
				if (removedText) {
					Document.UndoStack.UndoLast(2);
				}
				if (oldLine != Caret.Line) {
					UpdateToEnd(oldLine);//从当前位置开始,更新到最后.
				} else {
					UpdateLineToEnd(Caret.Line, Caret.Column);//从当前位置开始只更新当前行.
				}
			} finally {
				if (!updating) {
					EndUpdate();
				}
			}
		}
		
		/// <remarks>
		/// Replaces a char at the caret position
		/// </remarks>
		public void ReplaceChar(char ch)
		{
			bool updating = motherTextEditorControl.IsUpdating;
			if (!updating) {
				BeginUpdate();
			}
			if (Document.TextEditorProperties.DocumentSelectionMode == DocumentSelectionMode.Normal && SelectionManager.SelectionCollection.Count > 0) {
				Caret.Position = SelectionManager.SelectionCollection[0].StartPosition;
				SelectionManager.RemoveSelectedText();
			}
			
			int lineNr   = Caret.Line;
			LineSegment  line = Document.GetLineSegment(lineNr);
			int offset = Document.PositionToOffset(Caret.Position);
			if (offset < line.Offset + line.Length) {
				Document.Replace(offset, 1, ch.ToString());
			} else {
				Document.Insert(offset, ch.ToString());
			}
			if (!updating) {
				EndUpdate();
				UpdateLineToEnd(lineNr, Caret.Column);
			}
			++Caret.Column;
//			++Caret.DesiredColumn;
		}
		
		
		public event KeyEventHandler    KeyEventHandler;
		public event DialogKeyProcessor DoProcessDialogKey;

		#region UPDATE Commands	
	
		internal void UpdateLine(int line)
		{
			UpdateLines(0, line, line);
		}
		
		internal void UpdateLines(int lineBegin, int lineEnd)
		{
			UpdateLines(0, lineBegin, lineEnd);
		}
	
		internal void UpdateToEnd(int lineBegin) 
		{
			//			if (lineBegin > FirstPhysicalLine + textView.VisibleLineCount) {
			//				return;
			//			}
			
			lineBegin     = Math.Min(lineBegin, FirstPhysicalLine);
			int y         = Math.Max(    0, (int)(lineBegin * textViewMargin.FontHeight));
			y = Math.Max(0, y - this.virtualTop.Y);
			Rectangle r = new Rectangle(0,
				y, 
				Width, 
				Height - y);
			Invalidate(r);
		}
		
		internal void UpdateLineToEnd(int lineNr, int xStart)
		{
			UpdateLines(xStart, lineNr, lineNr);
		}
		
		internal void UpdateLine(int line, int begin, int end)
		{
			UpdateLines(line, line);
		}
		int FirstPhysicalLine 
		{
			get 
			{
				return VirtualTop.Y / textViewMargin.FontHeight;
			}
		}
		internal void UpdateLines(int xPos, int lineBegin, int lineEnd)
		{
			InvalidateLines((int)(xPos * this.TextViewMargin.GetWidth(' ')), lineBegin, lineEnd);
		}
		
		void InvalidateLines(int xPos, int lineBegin, int lineEnd)
		{
			lineBegin     = Math.Max(Document.GetVisibleLine(lineBegin), FirstPhysicalLine);
			lineEnd       = Math.Min(Document.GetVisibleLine(lineEnd),   FirstPhysicalLine + textViewMargin.VisibleLineCount);
			int y         = Math.Max(    0, (int)(lineBegin  * textViewMargin.FontHeight));
			int height    = Math.Min(textViewMargin.DrawingRectangle.Height, (int)((1 + lineEnd - lineBegin) * (textViewMargin.FontHeight + 1)));
			
			Rectangle r = new Rectangle(0,
				y - 1 - this.virtualTop.Y, 
				Width, 
				height + 3);
			
			Invalidate(r);
		}
		
		
		#endregion

		#region keyboard handling methods
		
		protected virtual bool HandleKeyPress(char ch)
		{
			if (KeyEventHandler != null) 
			{
				return KeyEventHandler(ch);
			}
			return false;
		}
		
		public void SimulateKeyPress(char ch)
		{
			if (Document.ReadOnly) 
			{
				return;
			}
			
			if (ch < ' ') 
			{
				return;
			}
			
			if (!HiddenMouseCursor && TextEditorProperties.HideMouseCursor) 
			{
				HiddenMouseCursor = true;
				Cursor.Hide();
			}
			
			motherTextEditorControl.BeginUpdate();
			switch (ch) 
			{
				default: // INSERT char
					if (!HandleKeyPress(ch)) 
					{
						switch (Caret.CaretMode) 
						{
							case CaretMode.InsertMode:
								InsertChar(ch);
								break;
							case CaretMode.OverwriteMode:
								ReplaceChar(ch);
								break;
							default:
								Debug.Assert(false, "Unknown caret mode " + Caret.CaretMode);
								break;
						}
					}
					break;
			}
			
			int currentLineNr = Caret.Line;
			int delta = Document.FormattingStrategy.FormatLine(this, currentLineNr, Document.PositionToOffset(Caret.Position), ch);
			
			motherTextEditorControl.EndUpdate();
			if (delta != 0) 
			{
				//				this.motherTextEditorControl.UpdateLines(currentLineNr, currentLineNr);
			}
		}
		
		protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
			SimulateKeyPress(e.KeyChar);
		}
		
		
		public bool ExecuteDialogKey(Keys keyData)
		{
			// try, if a dialog key processor was set to use this
			if (DoProcessDialogKey != null && DoProcessDialogKey(keyData)) 
			{
				return true;
			}
			
			// if not (or the process was 'silent', use the standard edit actions
			IEditAction action =  motherTextEditorControl.GetEditAction(keyData);
			AutoClearSelection = true;
			if (action != null) 
			{
				motherTextEditorControl.BeginUpdate();
				try 
				{
					lock (Document) 
					{
						action.Execute(this);
						if (SelectionManager.HasSomethingSelected && AutoClearSelection /*&& caretchanged*/) 
						{
							if (Document.TextEditorProperties.DocumentSelectionMode == DocumentSelectionMode.Normal) 
							{
								SelectionManager.ClearSelection();
							}
						}
					}
				} 
				catch (Exception e) 
				{
					Console.WriteLine("Got Exception while executing action " + action + " : " + e.ToString());
				} 
				finally 
				{
					motherTextEditorControl.EndUpdate();
					Caret.UpdateCaretPosition();
				}
				return true;
			} 
			return false;
		}
		
		protected override bool ProcessDialogKey(Keys keyData)
		{
			return ExecuteDialogKey(keyData) || base.ProcessDialogKey(keyData);
		}
		
		
		#endregion
	}
}
