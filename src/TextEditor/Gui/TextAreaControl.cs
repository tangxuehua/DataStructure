
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


namespace NetFocus.DataStructure.TextEditor
{
	[ToolboxItem(false)]
	public class TextAreaControl : Panel
	{
		TextEditorControl motherTextEditorControl;//所属的文本编辑器控件.
		TextArea   textArea;

		HRuler     hRuler     = null;
		VScrollBar vScrollBar = new VScrollBar();
		HScrollBar hScrollBar = new HScrollBar();
		bool       doHandleMousewheel = true;//是否处理鼠标滚轮.
		
		public TextArea TextArea {
			get {
				return textArea;
			}
		}
		
		public VScrollBar VScrollBar 
		{
			get 
			{
				return vScrollBar;
			}
		}
		
		public HScrollBar HScrollBar 
		{
			get 
			{
				return hScrollBar;
			}
		}
		
		public bool DoHandleMousewheel 
		{
			get 
			{
				return doHandleMousewheel;
			}
			set 
			{
				doHandleMousewheel = value;
			}
		}
		

		public SelectionManager SelectionManager 
		{
			get {
				return textArea.SelectionManager;
			}
		}
		
		
		public Caret Caret {
			get {
				return textArea.Caret;
			}
		}
		
		
		[Browsable(false)]
		public IDocument Document {
			get {
				return motherTextEditorControl.Document;
			}
		}
		
		
		public ITextEditorProperties TextEditorProperties {
			get {
				return motherTextEditorControl.TextEditorProperties;
			}
		}
		

		void VScrollBarValueChanged(object sender, EventArgs e)
		{
			textArea.VirtualTop = new Point(textArea.VirtualTop.X, vScrollBar.Value);
			textArea.Invalidate();
		}
		
		void HScrollBarValueChanged(object sender, EventArgs e)
		{
			textArea.VirtualTop = new Point(hScrollBar.Value, textArea.VirtualTop.Y);
			textArea.Invalidate();
		}
		
		public void AdjustScrollBars(object sender, DocumentEventArgs e)
		{
			vScrollBar.Minimum = 0;
			vScrollBar.Maximum = textArea.MaxVScrollValue;
			int max = 0;

			foreach (ISegment lineSegment in Document.LineSegmentCollection) 
			{
				if(Document.FoldingManager.IsLineVisible(Document.GetLineNumberForOffset(lineSegment.Offset))) 
				{
					max = Math.Max(lineSegment.Length, max);
				}
			}
			hScrollBar.Minimum = 0;
			hScrollBar.Maximum = (Math.Max(0, max + textArea.TextViewMargin.VisibleColumnCount - 1));
			
			vScrollBar.LargeChange = Math.Max(0, textArea.TextViewMargin.DrawingRectangle.Height);
			vScrollBar.SmallChange = Math.Max(0, textArea.TextViewMargin.FontHeight);
			
			hScrollBar.LargeChange = Math.Max(0, textArea.TextViewMargin.VisibleColumnCount - 1);
			hScrollBar.SmallChange = Math.Max(0, (int)textArea.TextViewMargin.GetWidth(' '));
		}
		
		
		public TextAreaControl(TextEditorControl motherTextEditorControl)
		{
			this.motherTextEditorControl = motherTextEditorControl;
			this.textArea                = new TextArea(motherTextEditorControl, this);
			
			this.Controls.Add(textArea);//将TextArea用户控件加到Panel中.
			
			vScrollBar.ValueChanged += new EventHandler(VScrollBarValueChanged);
			this.Controls.Add(this.vScrollBar);
			hScrollBar.ValueChanged += new EventHandler(HScrollBarValueChanged);
			this.Controls.Add(this.hScrollBar);

			ResizeRedraw = true;//调整大小时重会自己.
			
			Document.DocumentChanged += new DocumentEventHandler(AdjustScrollBars);
			
			SetStyle(ControlStyles.Selectable, true);//可以获得焦点.
		}
		
		
		void SetScrollBarBounds()
		{
			vScrollBar.Bounds = new Rectangle(textArea.Bounds.Right, 0, SystemInformation.HorizontalScrollBarArrowWidth, Height - SystemInformation.VerticalScrollBarArrowHeight);
			hScrollBar.Bounds = new Rectangle(0, textArea.Bounds.Bottom, Width - SystemInformation.HorizontalScrollBarArrowWidth, SystemInformation.VerticalScrollBarArrowHeight);
		}
		
		void ResizeTextArea()
		{
			int y = 0;
			int h = 0;
			if (hRuler != null) 
			{
				hRuler.Bounds = new Rectangle(0, 
					0, 
					Width - SystemInformation.HorizontalScrollBarArrowWidth,
					textArea.TextViewMargin.FontHeight);
							
				y = hRuler.Bounds.Bottom;
				h = hRuler.Bounds.Height;
			}
			
			textArea.Bounds = new Rectangle(0, y,
				Width - SystemInformation.HorizontalScrollBarArrowWidth,
				Height - SystemInformation.VerticalScrollBarArrowHeight - h);
			SetScrollBarBounds();//设置滚动条的边界.
		}
		
		protected override void OnResize(System.EventArgs e)
		{
			base.OnResize(e);
			ResizeTextArea();
		}
		
		
		public void OptionsChanged()
		{
			textArea.OptionsChanged();
			
			if (textArea.TextEditorProperties.ShowHorizontalRuler) 
			{
				if (hRuler == null) 
				{
					hRuler = new HRuler(textArea);
					Controls.Add(hRuler);
					ResizeTextArea();
				}
			} 
			else 
			{
				if (hRuler != null) 
				{
					Controls.Remove(hRuler);
					hRuler.Dispose();
					hRuler = null;
					ResizeTextArea();
				}
			}
			
			AdjustScrollBars(null, null);
		}
		
		
		public void HandleMouseWheel(MouseEventArgs e)
		{
			if ((Control.ModifierKeys & Keys.Control) != 0 && TextEditorProperties.MouseWheelTextZoom) {
				if (e.Delta > 0) {
					motherTextEditorControl.Font = new Font(motherTextEditorControl.Font.Name,
					                                        motherTextEditorControl.Font.Size + 1);
					
				} else {
					motherTextEditorControl.Font = new Font(motherTextEditorControl.Font.Name,
					                                        Math.Max(6, motherTextEditorControl.Font.Size - 1));
					
					
				}
			} else {
				int MAX_DELTA  = 120; // basically it's constant now, but could be changed later by MS
				int multiplier = Math.Abs(e.Delta) / MAX_DELTA;
				
				int newValue;
				if (System.Windows.Forms.SystemInformation.MouseWheelScrollLines > 0) {
					newValue = this.vScrollBar.Value - (TextEditorProperties.MouseWheelScrollDown ? 1 : -1) * Math.Sign(e.Delta) * System.Windows.Forms.SystemInformation.MouseWheelScrollLines * vScrollBar.SmallChange * multiplier;
				} else {
					newValue = this.vScrollBar.Value - (TextEditorProperties.MouseWheelScrollDown ? 1 : -1) * Math.Sign(e.Delta) * vScrollBar.LargeChange;
				}
				vScrollBar.Value = Math.Max(vScrollBar.Minimum, Math.Min(vScrollBar.Maximum, newValue));
			}
		}
		
		
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if (DoHandleMousewheel) {
				HandleMouseWheel(e);
			}
		}
		
		
		public void ScrollToCaret()
		{
			int curCharMin  = (int)(this.hScrollBar.Value - this.hScrollBar.Minimum);
			int curCharMax  = curCharMin + textArea.TextViewMargin.VisibleColumnCount;
			
			int pos         = textArea.TextViewMargin.GetVisualColumn(textArea.Caret.Line, textArea.Caret.Column);
			
			if (textArea.TextViewMargin.VisibleColumnCount < 0) {
				hScrollBar.Value = 0;
			} else {
				if (pos < curCharMin) {
					hScrollBar.Value = (int)(Math.Max(0, pos - scrollMarginHeight));
				} else {
					if (pos > curCharMax) {
						hScrollBar.Value = (int)Math.Max(0, Math.Min(hScrollBar.Maximum, (pos - textArea.TextViewMargin.VisibleColumnCount + scrollMarginHeight)));
					}
				}
			}
			ScrollTo(textArea.Caret.Line);
		}
		
		
		int scrollMarginHeight  = 3;
		
		public void ScrollTo(int line)
		{
			line = Math.Max(0, Math.Min(Document.TotalNumberOfLines - 1, line));
			line = Document.GetVisibleLine(line);
			int curLineMin = textArea.TextViewMargin.FirstPhysicalLine;
			if (textArea.TextViewMargin.LineHeightRemainder > 0) {
				curLineMin ++;
			}
			
			if (line - scrollMarginHeight + 3 < curLineMin) {
				this.vScrollBar.Value =  Math.Max(0, Math.Min(this.vScrollBar.Maximum, (line - scrollMarginHeight + 3) * textArea.TextViewMargin.FontHeight)) ;
				VScrollBarValueChanged(this, EventArgs.Empty);
			} else {
				int curLineMax = curLineMin + this.textArea.TextViewMargin.VisibleLineCount;
				if (line + scrollMarginHeight - 1 > curLineMax) {
					if (this.textArea.TextViewMargin.VisibleLineCount == 1) {
						this.vScrollBar.Value =  Math.Max(0, Math.Min(this.vScrollBar.Maximum, (line - scrollMarginHeight - 1) * textArea.TextViewMargin.FontHeight)) ;
					} else {
						this.vScrollBar.Value = Math.Min(this.vScrollBar.Maximum,
						                                 (line - this.textArea.TextViewMargin.VisibleLineCount + scrollMarginHeight - 1)* textArea.TextViewMargin.FontHeight) ;
					}
					VScrollBarValueChanged(this, EventArgs.Empty);
				}
			}
		}
		
		
		public void JumpTo(int line, int column)
		{
			textArea.Focus();
			textArea.SelectionManager.ClearSelection();
			textArea.Caret.Position = new Point(column, line);
			textArea.SetDesiredColumn();
			ScrollToCaret();
		}
	}
}
