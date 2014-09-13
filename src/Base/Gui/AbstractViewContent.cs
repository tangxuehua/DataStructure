using System;
using System.Windows.Forms;
using System.IO;


namespace NetFocus.DataStructure.Gui
{
	public abstract class AbstractViewContent : IViewContent
	{
		string untitledName = "";
		string contentName  = null;
		bool   isDirty  = false;
		bool   isViewOnly = false;
		Type algorithmType = null;
		
		
		public virtual string UntitledName {
			get {
				return untitledName;
			}
			set {
				untitledName = value;
			}
		}
		
		public virtual string ContentName {
			get {
				return contentName;
			}
			set {
				contentName = value;
				OnContentNameChanged(null);
			}
		}
		
		public bool IsUntitled {
			get {
				return contentName == null;
			}
		}
		public Type AlgorithmType
		{
			get
			{
				return algorithmType;
			}
			set
			{
				algorithmType = value;
			}
		}
		public virtual bool IsDirty {
			get {
				return isDirty;
			}
			set {
				isDirty = value;
				OnDirtyChanged(null);
			}
		}
		
		public virtual bool IsReadOnly {
			get {
				return false;
			}
		}		
		
		public virtual bool IsViewOnly {
			get {
				return isViewOnly;
			}
			set {
				isViewOnly = value;
			}
		}
		
		public abstract Control Control {
			get;
		}
		
		public virtual void RedrawContent()
		{
		}
		
		public virtual void Dispose()
		{
		}
		public abstract void SelectView();
		
		public abstract void CloseView(bool force);
		
		public virtual void SaveFile()
		{
			throw new System.NotImplementedException();//要求子类必须重写这个方法.
		}
		
		public virtual void SaveFile(string fileName)
		{
			throw new System.NotImplementedException();//要求子类必须重写这个方法.
		}
		
		public abstract void LoadFile(string fileName);
				
		public virtual void OnDirtyChanged(EventArgs e)
		{
			if (DirtyChanged != null) {
				DirtyChanged(this, e);
			}
		}
				
		public virtual void OnContentNameChanged(EventArgs e)
		{
			if (ContentNameChanged != null) {
				ContentNameChanged(this, e);
			}
		}
		public virtual void OnViewSelected(EventArgs e)
		{
			if (ViewSelected != null) 
			{
				ViewSelected(this, e);
			}
		}
				
		public virtual void OnCloseEvent(EventArgs e)
		{
			if (CloseEvent != null) 
			{
				CloseEvent(this, e);
			}
		}
		

		public event EventHandler ContentNameChanged;
		public event EventHandler DirtyChanged;
		public event EventHandler ViewSelected;
		public event EventHandler CloseEvent;

	}
}
