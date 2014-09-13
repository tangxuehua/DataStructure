using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml;

using DevComponents.DotNetBar;

using NetFocus.Components.AddIns;
using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Services;


namespace NetFocus.DataStructure.Gui
{
	/// <summary>
	/// This is a Workspace with a multiple document interface.
	/// </summary>
	public class DefaultWorkbench : Form, IWorkbench, IMementoCapable
	{
		#region some variants
		
		const string workbenchMemento        = "DataStructure.Workbench.WorkbenchMemento";
		readonly string mainMenuPath    = "/DataStructure/Workbench/MainMenu";
		readonly string viewContentPath = "/DataStructure/Workbench/Pads";
		PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));

		private DevComponents.DotNetBar.DotNetBarManager dotNetBarManager1;
		private DevComponents.DotNetBar.DockSite barLeftDockSite;
		private DevComponents.DotNetBar.DockSite barRightDockSite;
		private DevComponents.DotNetBar.DockSite barTopDockSite;
		private DevComponents.DotNetBar.DockSite barBottomDockSite;
		public Bar TopMenu = new Bar();
		public Bar[]   ToolBars;

		bool            fullscreen;
		IWorkbenchLayout layout = null;
		FormWindowState defaultWindowState = FormWindowState.Normal;
		Rectangle       normalBounds       = new Rectangle(0, 0, 640, 480);
		PadContentCollection padContentCollection       = new PadContentCollection();
		ViewContentCollection viewContentCollection = new ViewContentCollection();
		

		#endregion

		#region the implemention of interface IWorkbench

		public string Title 
		{
			get 
			{
				return Text;
			}
			set 
			{
				Text = value;
			}
		}
		

		public PadContentCollection PadContentCollection 
		{
			get 
			{
				return padContentCollection;
			}
		}
		
		
		public ViewContentCollection ViewContentCollection 
		{
			get 
			{
				return viewContentCollection;
			}
		}
		

		public IViewContent ActiveViewContent 
		{
			get 
			{
				if (layout == null) 
				{
					return null;
				}
				return layout.ActiveViewContent;
			}
		}


		public IWorkbenchLayout WorkbenchLayout 
		{
			get 
			{
				return layout;
			}
			set 
			{
				layout = value;
			}
		}
		

		public virtual void ShowView(IViewContent content)
		{
			if (layout == null) 
			{
				return;
			}
			layout.ShowView(content);
		}
		

		public void CloseView(IViewContent content)
		{
			if (layout == null) 
			{
				return;
			}
			layout.CloseView(content);
		}

		
		public void CloseViews()
		{
			if (layout == null) 
			{
				return;
			}
			layout.CloseViews();
			
		}

		
		public virtual void ShowPad(IPadContent content)
		{
			if (layout != null) 
			{
				layout.ShowPad(content);
			}
		}
		

		public IPadContent GetPad(Type type)
		{
			if (layout == null) 
			{
				return null;
			}
			return layout.GetPad(type);
		}


		#endregion

		#region implements the IMementoCapable interface
		
		public IXmlConvertable CreateMemento()
		{
			WorkbenchMemento memento   = new WorkbenchMemento();
			memento.Bounds             = normalBounds;
			memento.DefaultWindowState = fullscreen ? defaultWindowState : WindowState;
			memento.WindowState        = WindowState;
			memento.FullScreen         = fullscreen;
			return memento;
		}
		
		public void SetMemento(IXmlConvertable xmlMemento)
		{
			if (xmlMemento != null) 
			{
				WorkbenchMemento memento = (WorkbenchMemento)xmlMemento;
				
				Bounds      = normalBounds = memento.Bounds;
				WindowState = memento.WindowState;
				defaultWindowState = memento.DefaultWindowState;
				FullScreen  = memento.FullScreen;
			}
		}
		public IXmlConvertable GetStoredMemento(IViewContent content)
		{
			if (content != null && content.ContentName != null) 
			{
				PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
				
				string directory = propertyService.ConfigDirectory + "temp";
				if (!Directory.Exists(directory)) 
				{
					Directory.CreateDirectory(directory);
				}
				string fileName = content.ContentName.Substring(3).Replace('/', '.').Replace('\\', '.').Replace(Path.DirectorySeparatorChar, '.');
				string fullFileName = directory + Path.DirectorySeparatorChar + fileName;
				// check the file name length because it could be more than the maximum length of a file name
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				if (fileUtilityService.IsValidFileName(fullFileName) && File.Exists(fullFileName)) 
				{
					IXmlConvertable prototype = ((IMementoCapable)content).CreateMemento();
					XmlDocument doc = new XmlDocument();
					doc.Load(fullFileName);
					
					return (IXmlConvertable)prototype.FromXmlElement((XmlElement)doc.DocumentElement.ChildNodes[0]);
				}
			}
			return null;
		}
		
		public void StoreMemento(IViewContent content)
		{
			if (content.ContentName == null) 
			{
				return;
			}
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			string directory = propertyService.ConfigDirectory + "temp";
			if (!Directory.Exists(directory)) 
			{
				Directory.CreateDirectory(directory);
			}
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<?xml version=\"1.0\"?>\n<Mementoable/>");
			
			XmlAttribute fileAttribute = doc.CreateAttribute("file");
			fileAttribute.InnerText = content.ContentName;
			doc.DocumentElement.Attributes.Append(fileAttribute);
			
			
			IXmlConvertable memento = ((IMementoCapable)content).CreateMemento();
			
			doc.DocumentElement.AppendChild(memento.ToXmlElement(doc));
			
			string fileName = content.ContentName.Substring(3).Replace('/', '.').Replace('\\', '.').Replace(Path.DirectorySeparatorChar, '.');
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			// check the file name length because it could be more than the maximum length of a file name
			string fullFileName = directory + Path.DirectorySeparatorChar + fileName;
			if (fileUtilityService.IsValidFileName(fullFileName)) 
			{
				fileUtilityService.ObservedSave(new NamedFileOperationDelegate(doc.Save), fullFileName, FileErrorPolicy.ProvideAlternative);
			}
		}
		

		#endregion

		public bool FullScreen 
		{
			get {
				return fullscreen;
			}
			set {
				fullscreen = value;
				if (fullscreen) {
					FormBorderStyle    = FormBorderStyle.None;
					defaultWindowState = WindowState;
					WindowState        = FormWindowState.Maximized;
				} else {
					FormBorderStyle = FormBorderStyle.Sizable;
					Bounds          = normalBounds;
					WindowState     = defaultWindowState;
				}
			}
		}
		
		
		public void AddMenuAndToolbarControls()
		{
			this.SuspendLayout();

			this.Controls.Add(this.barLeftDockSite);
			this.Controls.Add(this.barRightDockSite);
			this.Controls.Add(this.barTopDockSite);
			this.Controls.Add(this.barBottomDockSite);

			this.ResumeLayout(false);
		}

		
		public void InitializeComponent()
		{
			this.dotNetBarManager1 = new DotNetBarManager();
			this.barBottomDockSite = new DevComponents.DotNetBar.DockSite();
			this.barLeftDockSite = new DevComponents.DotNetBar.DockSite();
			this.barRightDockSite = new DevComponents.DotNetBar.DockSite();
			this.barTopDockSite = new DevComponents.DotNetBar.DockSite();

			// 
			// dotNetBarManager1
			// 
			this.dotNetBarManager1.BottomDockSite = this.barBottomDockSite;
			this.dotNetBarManager1.DefinitionName = "";
			this.dotNetBarManager1.LeftDockSite = this.barLeftDockSite;
			this.dotNetBarManager1.ParentForm = this;
			this.dotNetBarManager1.RightDockSite = this.barRightDockSite;
			this.dotNetBarManager1.TopDockSite = this.barTopDockSite;
			this.dotNetBarManager1.Style = eDotNetBarStyle.OfficeXP;
			// 
			// barBottomDockSite
			// 
			this.barBottomDockSite.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			this.barBottomDockSite.BackgroundImageAlpha = ((System.Byte)(255));
			this.barBottomDockSite.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.barBottomDockSite.Location = new System.Drawing.Point(0, 318);
			this.barBottomDockSite.Name = "barBottomDockSite";
			this.barBottomDockSite.Size = new System.Drawing.Size(664, 0);
			this.barBottomDockSite.TabIndex = 3;
			this.barBottomDockSite.TabStop = false;
			// 
			// barLeftDockSite
			// 
			this.barLeftDockSite.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			this.barLeftDockSite.BackgroundImageAlpha = ((System.Byte)(255));
			this.barLeftDockSite.Dock = System.Windows.Forms.DockStyle.Left;
			this.barLeftDockSite.Location = new System.Drawing.Point(0, 0);
			this.barLeftDockSite.Name = "barLeftDockSite";
			this.barLeftDockSite.Size = new System.Drawing.Size(0, 318);
			this.barLeftDockSite.TabIndex = 0;
			this.barLeftDockSite.TabStop = false;
			// 
			// barRightDockSite
			// 
			this.barRightDockSite.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			this.barRightDockSite.BackgroundImageAlpha = ((System.Byte)(255));
			this.barRightDockSite.Dock = System.Windows.Forms.DockStyle.Right;
			this.barRightDockSite.Location = new System.Drawing.Point(664, 0);
			this.barRightDockSite.Name = "barRightDockSite";
			this.barRightDockSite.Size = new System.Drawing.Size(0, 318);
			this.barRightDockSite.TabIndex = 1;
			this.barRightDockSite.TabStop = false;
			// 
			// barTopDockSite
			// 
			this.barTopDockSite.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			this.barTopDockSite.BackgroundImageAlpha = ((System.Byte)(255));
			this.barTopDockSite.Dock = System.Windows.Forms.DockStyle.Top;
			this.barTopDockSite.Location = new System.Drawing.Point(0, 0);
			this.barTopDockSite.Name = "barTopDockSite";
			this.barTopDockSite.Size = new System.Drawing.Size(664, 0);
			this.barTopDockSite.TabIndex = 2;
			this.barTopDockSite.TabStop = false;

		
		}

		
		public DefaultWorkbench()
		{
			//为窗体做一些简单的初始化操作
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			this.Text = resourceService.GetString("MainWindow.DialogName");
			this.Icon = resourceService.GetIcon("Icons.DataStructureIcon");
			this.MinimumSize = new Size(510,410);
			this.StartPosition = FormStartPosition.CenterScreen;
			this.AllowDrop      = true;
			this.BackColor = SystemColors.AppWorkspace;
			InitializeComponent();

		}
		
		
		public void CreateMenu(object sender, EventArgs e)
		{
			TopMenu.MenuBar=true;
			TopMenu.Stretch=true;
			IAddInTreeNode node = AddInTreeSingleton.AddInTree.GetTreeNode(mainMenuPath);
			ArrayList objectList = node.BuildChildItems(this);
			ButtonItem[] items = (ButtonItem[])objectList.ToArray(typeof(ButtonItem));
			TopMenu.Items.Clear();
			TopMenu.Items.AddRange(items);
			if(dotNetBarManager1.Bars.Contains(TopMenu) == false)
			{
				dotNetBarManager1.Bars.Add(TopMenu);
			}
			TopMenu.DockSide=eDockSide.Top;
		}

		
		
		void CreatePads(object sender, EventArgs e)
		{
			IPadContent[] contents = (IPadContent[])(AddInTreeSingleton.AddInTree.GetTreeNode(viewContentPath).BuildChildItems(this)).ToArray(typeof(IPadContent));
			foreach (IPadContent content in contents) 
			{
				PadContentCollection.Add(content);
			}
		}

		void CreateToolBars()
		{
			ToolbarService toolBarService = (ToolbarService)ServiceManager.Services.GetService(typeof(ToolbarService));
			
			ToolBars = null;
			
			ToolBars = toolBarService.CreateToolbars();
				
			foreach (Bar toolBar in ToolBars) 
			{
				if(dotNetBarManager1.Bars.Contains(toolBar) == false)
				{
					dotNetBarManager1.Bars.Add(toolBar);
				}
				toolBar.DockSide=eDockSide.Top;
			}
		}
		void SetStandardStatusBar(object sender, EventArgs e)
		{
			IStatusBarService statusBarService = (IStatusBarService)NetFocus.DataStructure.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			statusBarService.SetMessage("就绪");
		}

		void CheckRemovedFile(object sender, FileEventArgs e)
		{
			if (e.IsDirectory) 
			{
				foreach (IViewContent content in ViewContentCollection) 
				{
					if (content.ContentName.StartsWith(e.FileName)) 
					{
						content.CloseView(true);
					}
				}
			} 
			else 
			{
				foreach (IViewContent content in ViewContentCollection) 
				{
					// WINDOWS DEPENDENCY : ToUpper
					if (content.ContentName != null &&
						content.ContentName.ToUpper() == e.FileName.ToUpper()) 
					{
						content.CloseView(true);
						return;
					}
				}
			}
		}
		
		void CheckRenamedFile(object sender, FileEventArgs e)
		{
			if (e.IsDirectory) 
			{
				foreach (IViewContent content in ViewContentCollection) 
				{
					if (content.ContentName.StartsWith(e.SourceFile)) 
					{
						content.ContentName = e.TargetFile + content.ContentName.Substring(e.SourceFile.Length);
					}
				}
			} 
			else 
			{
				foreach (IViewContent content in ViewContentCollection) 
				{
					// WINDOWS DEPENDENCY : ToUpper
					if (content.ContentName != null &&
						content.ContentName.ToUpper() == e.SourceFile.ToUpper()) 
					{
						content.ContentName = e.TargetFile;
						return;
					}
				}
			}
		}


		public void InitializeWorkspace()
		{
			CreatePads(this, null);
			CreateToolBars();
			CreateMenu(this,null);
			
			layout = new SdiWorkbenchLayout();
			layout.Attach(this);

			layout.ShowPads(padContentCollection);

			SetMemento((IXmlConvertable)propertyService.GetProperty(workbenchMemento, new WorkbenchMemento()));

			this.MenuComplete += new EventHandler(SetStandardStatusBar);
			SetStandardStatusBar(null, null);
			
			IFileService fileService = (IFileService)ServiceManager.Services.GetService(typeof(IFileService));
			
			fileService.FileRemoved += new FileEventHandler(CheckRemovedFile);
			fileService.FileRenamed += new FileEventHandler(CheckRenamedFile);
			fileService.FileRemoved += new FileEventHandler(fileService.RecentOpenMemeto.FileRemoved);
			fileService.FileRenamed += new FileEventHandler(fileService.RecentOpenMemeto.FileRenamed);

			fileService.RecentOpenMemeto.RecentFileChanged    += new EventHandler(CreateMenu);

			layout.ActiveViewContentChanged += new EventHandler(CreateMenu);

		}
		

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (WindowState == FormWindowState.Normal) {
				normalBounds = Bounds;
			}
			
		}
		
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			
			while (viewContentCollection.Count > 0) {
				IViewContent content = viewContentCollection[0];
				content.CloseView(false);
				if (viewContentCollection.IndexOf(content) >= 0) {
					e.Cancel = true;
					return;
				}
			}
		}
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			layout.Detach();
			foreach (IPadContent content in PadContentCollection) 
			{
				content.Dispose();
			}

		}
		protected override void OnDragDrop(DragEventArgs e)
		{
			base.OnDragDrop(e);
			if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop)) 
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				IFileService fileService = (IFileService)NetFocus.DataStructure.Services.ServiceManager.Services.GetService(typeof(IFileService));
				foreach (string file in files) 
				{
					if (File.Exists(file)) 
					{
						fileService.OpenFile(file);
					}
				}
			}
		}
		protected override void OnDragEnter(DragEventArgs e)
		{
			base.OnDragEnter(e);
			if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop)) 
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				foreach (string file in files) 
				{
					if (File.Exists(file)) 
					{
						e.Effect = DragDropEffects.Copy;
						return;
					}
				}
			}
			e.Effect = DragDropEffects.None;
		}
		

		
	}
}
