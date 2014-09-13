
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
	public delegate void MarginMouseEventHandler(AbstractMargin sender, Point mousepos, MouseButtons mouseButtons);
	public delegate void MarginPaintEventHandler(AbstractMargin sender, Graphics g, Rectangle rect);
	
	//这个类代表一个区域,可以是一个文本区域,也可以是一个装订线区域,或者是一个折叠区域,etc.
	public abstract class AbstractMargin
	{
		protected Rectangle drawingRectangle = new Rectangle(0, 0, 0, 0);
		protected TextArea  textArea;
		
		public Rectangle DrawingRectangle {
			get {
				return drawingRectangle;
			}
			set {
				drawingRectangle = value;
			}
		}
		
		public TextArea TextArea {
			get {
				return textArea;
			}
		}
		
		public IDocument Document {
			get {
				return textArea.Document;
			}
		}
		
		public ITextEditorProperties TextEditorProperties {
			get {
				return textArea.Document.TextEditorProperties;
			}
		}
		
		public virtual Cursor Cursor {
			get {
				return Cursors.Default;//默认是正常的光标,但比如说是GutterMargin则可以重写此默认值.
			}
		}
		
		public virtual Size Size {
			get {
				return new Size(-1, -1);
			}
		}
		
		public virtual bool IsVisible {
			get {
				return true;
			}
		}
		
		
		protected AbstractMargin(TextArea textArea)
		{
			this.textArea = textArea;
		}
		
		
		//以下定义了一些关于鼠标的事件和方法,以及定义了一个如何绘制自己的事件和方法.
		public virtual void OnMouseDown(Point mousepos, MouseButtons mouseButtons)
		{
			if (MouseDown != null) {
				MouseDown(this, mousepos, mouseButtons);
			}
		}
		public virtual void OnMouseMove(Point mousepos, MouseButtons mouseButtons)
		{
			if (MouseMove != null) {
				MouseMove(this, mousepos, mouseButtons);
			}
		}
		public virtual void OnMouseLeave(EventArgs e)
		{
			if (MouseLeave != null) {
				MouseLeave(this, e);
			}
		}
		
		public virtual void OnPaint(Graphics g, Rectangle rect)
		{
			if (Paint != null) 
			{
				Paint(this, g, rect);
			}
		}
		
		public event MarginMouseEventHandler MouseDown;
		public event MarginMouseEventHandler MouseMove;
		public event EventHandler            MouseLeave;
		public event MarginPaintEventHandler Paint;
	}
}

