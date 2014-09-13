
using System;
using System.Drawing;
using System.Windows.Forms;

using NetFocus.DataStructure.Services;

namespace NetFocus.DataStructure.Gui
{
	public abstract class AbstractPadContent : IPadContent
	{
		string title;
		Bitmap icon;
		string[] shortcut = null;
		public abstract Control Control {
			get;
		}
		
		public virtual string Title {
			get {
				return title;
			}
		}
		
		public virtual Bitmap Icon {
			get {
				return icon;
			}
		}

		public string[] Shortcut 
		{
			get 
			{
				return shortcut;
			}
			set 
			{
				shortcut = value;
			}
		}
		
		public virtual void RedrawContent()
		{
			
		}

		public AbstractPadContent(string title, string iconResoureName)
		{
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			this.title = stringParserService.Parse(title);
			if (iconResoureName != null) 
			{
				ResourceService ResourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
				this.icon  = ResourceService.GetBitmap(iconResoureName);
			}
		}

		public void BringPadToFront()
		{
			if (!WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible(this)) 
			{
				WorkbenchSingleton.Workbench.WorkbenchLayout.ShowPad(this);
			}
			WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(this);
		}
		
		public virtual void Dispose()
		{
			if (icon != null) {
				icon.Dispose();
			}
		}
		
		protected virtual void OnTitleChanged(EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}
		
		protected virtual void OnIconChanged(EventArgs e)
		{
			if (IconChanged != null) {
				IconChanged(this, e);
			}
		}
		
		public event EventHandler TitleChanged;
		public event EventHandler IconChanged;

	}
}
