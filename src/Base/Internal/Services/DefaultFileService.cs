
using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Windows.Forms;

using NetFocus.DataStructure.Gui;


namespace NetFocus.DataStructure.Services
{
	public delegate void FileEventHandler(object sender, FileEventArgs e);
	
	public class FileEventArgs : EventArgs
	{
		string fileName   = null;
		string sourceFile = null;
		string targetFile = null;
		
		bool   isDirectory;
		
		public string FileName 
		{
			get 
			{
				return fileName;
			}
		}
		
		public string SourceFile 
		{
			get 
			{
				return sourceFile;
			}
		}
		
		public string TargetFile 
		{
			get 
			{
				return targetFile;
			}
		}
		
		
		public bool IsDirectory 
		{
			get 
			{
				return isDirectory;
			}
		}
		
		public FileEventArgs(string fileName, bool isDirectory)
		{
			this.fileName = fileName;
			this.isDirectory = isDirectory;
		}
		
		public FileEventArgs(string sourceFile, string targetFile, bool isDirectory)
		{
			this.sourceFile = sourceFile;
			this.targetFile = targetFile;
			this.isDirectory = isDirectory;
		}
	}
	
	public class DefaultFileService : AbstractService, IFileService
	{
		string currentFile;
		RecentOpenMemeto       recentOpen = null;
		
		public RecentOpenMemeto RecentOpenMemeto {
			get {
				if (recentOpen == null) {
					PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
					recentOpen = (RecentOpenMemeto)propertyService.GetProperty("NetFocus.DataStructure.Gui.MainWindow.RecentOpenMemeto", new RecentOpenMemeto());
				}
				return recentOpen;
			}
		}
		
		public string CurrentFile {
			get {
				return currentFile;
			}
			set {
				currentFile = value;
			}
		}
		
		
		class LoadFileWrapper
		{
			IViewType viewType;
			
			public LoadFileWrapper(IViewType viewType)
			{
				this.viewType = viewType;
			}
			
			public void Invoke(string fileName)
			{
				//利用当前的displayBinding并根据文件名来最终创建一个视图.
				IViewContent viewContent = viewType.CreateContentForFile(fileName);
				WorkbenchSingleton.Workbench.ShowView(viewContent);
			}
		}
		
		
		public void OpenFile(string fileName)
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));

			Debug.Assert(fileUtilityService.IsValidFileName(fileName));
				
			// test, if file fileName exists
			if (!fileName.StartsWith("http://")) {
				// test, if an untitled file should be opened
				if (!Path.IsPathRooted(fileName)) { 
					foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
						if (content.IsUntitled && content.UntitledName == fileName) {
							content.SelectView();
							return;
						}
					}
				} else if (!fileUtilityService.TestFileExists(fileName)) {
					return;
				}
			}
			//检查当前文件有没有在视图中打开过.
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				// WINDOWS DEPENDENCY : ToUpper()
				if (content.ContentName != null && 
				    content.ContentName.ToUpper() == fileName.ToUpper()) {
					//content.SelectView();
					MessageBox.Show("该文件已经打开！","提示",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
					return;
				}
			}
			
			ViewTypeService viewTypeService = (ViewTypeService)ServiceManager.Services.GetService(typeof(ViewTypeService));
			
			IFileService fileService = (IFileService)ServiceManager.Services.GetService(typeof(IFileService));
			IViewType viewType = viewTypeService.GetViewTypePerFileName(fileName);
			
			if (viewType != null) {
				LoadFileWrapper fileWrapper = new LoadFileWrapper(viewType);
				NamedFileOperationDelegate nameFileOperationDelegate = new NamedFileOperationDelegate(fileWrapper.Invoke);
				FileOperationResult result = fileUtilityService.ObservedLoad(nameFileOperationDelegate,fileName);
				if (result == FileOperationResult.OK) {//如果当前文档打开成功,则把当前打开的文档添加到最近打开的列表中.
					fileService.RecentOpenMemeto.AddLastFile(fileName);
				}
			} else {
				throw new ApplicationException("Can't open " + fileName + ", no display codon found.");
			}
		}
		
		public void NewFile(string defaultName, string language, string content)
		{
			ViewTypeService viewTypeService = (ViewTypeService)ServiceManager.Services.GetService(typeof(ViewTypeService));
			
			IViewType viewType = viewTypeService.GetViewTypePerLanguageName(language);
			
			if (viewType != null) 
			{
				IViewContent newContent = viewType.CreateContentForLanguage(language, content);
				newContent.UntitledName = defaultName;
				newContent.IsDirty      = false;
				WorkbenchSingleton.Workbench.ShowView(newContent);
			} 
			else 
			{
				throw new ApplicationException("Can't create display binding for language " + language);				
			}
		}
		
		public IViewContent GetOpenFile(string fileName)
		{
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				// WINDOWS DEPENDENCY : ToUpper()
				if (content.ContentName != null &&
				    content.ContentName.ToUpper() == fileName.ToUpper()) {
					return content;
				}
			}
			return null;
		}
		

		
		public void RemoveFile(string fileName)
		{
			if (Directory.Exists(fileName)) {
				try {
					Directory.Delete(fileName);
				} catch (Exception e) {
					MessageBox.Show("Can't remove directory " + fileName + " reason:\n" + e.ToString(), "Error");
					return;
				}
				OnFileRemoved(new FileEventArgs(fileName, true));
			} else {
				try {
					File.Delete(fileName);
				} catch (Exception e) {
					MessageBox.Show("Can't remove file " + fileName + " reason:\n" + e.ToString(), "Error");
					return;
				}
				OnFileRemoved(new FileEventArgs(fileName, false));
			}
		}
		
		public void RenameFile(string oldName, string newName)
		{
			if (oldName != newName) {
				if (Directory.Exists(oldName)) {
					try {
						Directory.Move(oldName, newName);
					} catch (Exception e) {
						MessageBox.Show("Can't rename directory " + oldName + " reason:\n" + e.ToString(), "Error");
						return;
					}
					OnFileRenamed(new FileEventArgs(oldName, newName, true));
				} else {
					try {
						File.Move(oldName, newName);
					} catch (Exception e) {
						MessageBox.Show("Can't rename file " + oldName + " reason:\n" + e.ToString(), "Error");
						return;
					}
					OnFileRenamed(new FileEventArgs(oldName, newName, false));
				}
			}
		}
		
		protected virtual void OnFileRemoved(FileEventArgs e)
		{
			if (FileRemoved != null) {
				FileRemoved(this, e);
			}
		}

		protected virtual void OnFileRenamed(FileEventArgs e)
		{
			if (FileRenamed != null) {
				FileRenamed(this, e);
			}
		}

		public event FileEventHandler FileRenamed;
		public event FileEventHandler FileRemoved;
	}


}
