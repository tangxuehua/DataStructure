using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;

using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.Gui.Pads;
using NetFocus.DataStructure.TextEditor;
using NetFocus.DataStructure.TextEditor.Undo;
using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.Properties;
using NetFocus.Components.AddIns;
using NetFocus.DataStructure.Services;
using NetFocus.Components.AddIns.Codons;


namespace NetFocus.DataStructure.Gui.Views
{
	public class TextEditorView : AbstractViewContent, IClipboardHandler, IMementoCapable, IPrintable, IEditable, IPositionable
	{
		delegate void VoidDelegate(AbstractMargin margin);
		TextEditorControl textEditorControl = null;
				
		public TextEditorView()
		{
			textEditorControl = new TextEditorControl();

			textEditorControl.Document.DocumentChanged += new DocumentEventHandler(TextAreaChangedEvent);
			textEditorControl.ActiveTextAreaControl.Caret.CaretModeChanged += new EventHandler(CaretModeChanged);
			textEditorControl.ActiveTextAreaControl.Enter += new EventHandler(CaretUpdate);
			textEditorControl.ActiveTextAreaControl.Caret.PositionChanged +=new EventHandler(CaretChanged);
			((Form)WorkbenchSingleton.Workbench).Activated += new EventHandler(GotFocusEvent);

		}
		
		
		public override string UntitledName 
		{
			get 
			{
				return base.UntitledName;
			}
			set 
			{
				base.UntitledName = value;
				textEditorControl.FileName = value;
			}
		}
		
		public override string ContentName 
		{
			set 
			{
				if (Path.GetExtension(ContentName) != Path.GetExtension(value)) 
				{
					if (textEditorControl.Document.HighlightingStrategy != null) 
					{
						textEditorControl.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighterForFile(value);
						textEditorControl.Refresh();
					}
				}
				base.ContentName  = value;
			}
		}

		public override void RedrawContent()
		{
			textEditorControl.OptionsChanged();
			textEditorControl.Refresh();
		}
		
		public override void Dispose()
		{
			((Form)NetFocus.DataStructure.Gui.WorkbenchSingleton.Workbench).Activated -= new EventHandler(GotFocusEvent);
			textEditorControl.Dispose();
		}
		
		public override bool IsReadOnly 
		{
			get 
			{
				return textEditorControl.IsReadOnly;
			}
		}
		
		public override void SelectView()
		{
			textEditorControl.Focus();
			OnViewSelected(null);
		}
		
		void SaveFileAs()
		{
			if (IsViewOnly) 
			{
				return;
			}
			using (SaveFileDialog fdiag = new SaveFileDialog()) 
			{
				fdiag.OverwritePrompt = true;
				fdiag.AddExtension    = true;
					
				string[] fileFilters  = (string[])(AddInTreeSingleton.AddInTree.GetTreeNode("/DataStructure/Workbench/FileFilter").BuildChildItems(this)).ToArray(typeof(string));
				fdiag.Filter          = String.Join("|", fileFilters);
				for (int i = 0; i < fileFilters.Length; ++i) 
				{
					if (fileFilters[i].IndexOf(Path.GetExtension(ContentName == null ? UntitledName : ContentName)) >= 0) 
					{
						fdiag.FilterIndex = i + 1;
						break;
					}
				}
					
				if (fdiag.ShowDialog() == DialogResult.OK) 
				{
					string fileName = fdiag.FileName;
						
					IFileService fileService = (IFileService)NetFocus.DataStructure.Services.ServiceManager.Services.GetService(typeof(IFileService));
					FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
					if (!fileUtilityService.IsValidFileName(fileName)) 
					{
						MessageBox.Show("File name " + fileName +" is invalid");
						return;
					}
					if (fileUtilityService.ObservedSave(new NamedFileOperationDelegate(SaveFile), fileName) == FileOperationResult.OK) 
					{
						fileService.RecentOpenMemeto.AddLastFile(fileName);
						//MessageBox.Show(fileName, "文件已保存", MessageBoxButtons.OK);
					}
				}
			}
			
		}

		public override void CloseView(bool force)
		{
			if (!force && IsDirty) 
			{
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
				DialogResult dr = MessageBox.Show(resourceService.GetString("MainWindow.SaveChangesMessage"),resourceService.GetString("MainWindow.SaveChangesMessageHeader") + " ?",MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				switch (dr) 
				{
					case DialogResult.Yes:
						if (ContentName == null) 
						{
							SaveFileAs();
							if (IsDirty) 
							{
								return;  //如果用户在保存新建的文件时选择了取消按钮,则直接返回,并且不关闭当前文档.
							} 
						}
						else 
						{
							FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
							fileUtilityService.ObservedSave(new FileOperationDelegate(SaveFile), ContentName , FileErrorPolicy.ProvideAlternative);
						}
						break;
					case DialogResult.No:
						break;
					case DialogResult.Cancel:
						return;
				}
			}
			OnCloseEvent(null);
		}

		public override void SaveFile()
		{
			if(ContentName != null)
			{
				if (watcher != null) 
				{
					this.watcher.EnableRaisingEvents = false;
				}

				textEditorControl.SaveFile(ContentName);
				IsDirty   = false;
			
				SetWatcher();
			}
		}

		public override void SaveFile(string fileName)
		{
			if (watcher != null) 
			{
				this.watcher.EnableRaisingEvents = false;
			}

			textEditorControl.SaveFile(fileName);
			ContentName  = fileName;
			IsDirty   = false;
			
			SetWatcher();

		}

		public override void LoadFile(string fileName)
		{
			textEditorControl.IsReadOnly = (File.GetAttributes(fileName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
			textEditorControl.LoadFile(fileName);
			ContentName  = fileName;
			IsDirty     = false;
			SetWatcher();
		}
		
		public override Control Control 
		{
			get 
			{
				return textEditorControl;
			}
		}
		
		public override void OnContentNameChanged(System.EventArgs e)
		{
			base.OnContentNameChanged(e);
			textEditorControl.FileName = base.ContentName;
		}
		
	
		void TextAreaChangedEvent(object sender, DocumentEventArgs e)
		{
			IsDirty = true;//如果当前视图中的内容有所改变,马上使IsDirty属性为true.
		}
		
		void CaretUpdate(object sender, EventArgs e)
		{
			CaretChanged(null, null);
			CaretModeChanged(null, null);
		}
		
		void CaretChanged(object sender, EventArgs e)
		{
			Point    pos       = textEditorControl.Document.OffsetToPosition(textEditorControl.ActiveTextAreaControl.Caret.Offset);
			LineSegment line   = textEditorControl.Document.GetLineSegment(pos.Y);
			IStatusBarService statusBarService = (IStatusBarService)NetFocus.DataStructure.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			statusBarService.SetCaretPosition(pos.X + 1, pos.Y + 1, textEditorControl.ActiveTextAreaControl.Caret.Offset - line.Offset + 1);
		}
		
		void CaretModeChanged(object sender, EventArgs e)
		{
			IStatusBarService statusBarService = (IStatusBarService)NetFocus.DataStructure.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			statusBarService.SetInsertMode(((TextEditorControl)this.Control).ActiveTextAreaControl.Caret.CaretMode == CaretMode.InsertMode);
		}
		
		
		#region implement the IMementoCapable interface.
		
		public IXmlConvertable CreateMemento()
		{
			DefaultProperties properties = new DefaultProperties();
			string[] bookMarks = new string[((TextEditorControl)this.Control).Document.BookmarkManager.Marks.Count];
			for (int i = 0; i < bookMarks.Length; ++i) 
			{
				bookMarks[i] = ((TextEditorControl)this.Control).Document.BookmarkManager.Marks[i].ToString();
			}
			properties.SetProperty("Bookmarks",   String.Join(",", bookMarks));
			properties.SetProperty("CaretOffset", ((TextEditorControl)this.Control).ActiveTextAreaControl.Caret.Offset);
			properties.SetProperty("VisibleLine", ((TextEditorControl)this.Control).ActiveTextAreaControl.TextArea.TextViewMargin.FirstVisibleLine);
			properties.SetProperty("HighlightingLanguage", ((TextEditorControl)this.Control).Document.HighlightingStrategy.Name);
			properties.SetProperty("Foldings", ((TextEditorControl)this.Control).Document.FoldingManager.SerializeToString());
			return properties;
		}
		
		public void SetMemento(IXmlConvertable memento)
		{
			IProperties properties = (IProperties)memento;
			string[] bookMarks = properties.GetProperty("Bookmarks").ToString().Split(',');
			foreach (string mark in bookMarks) 
			{
				if (mark != null && mark.Length > 0) 
				{
					textEditorControl.Document.BookmarkManager.Marks.Add(Int32.Parse(mark));
				}
			}
			
			textEditorControl.ActiveTextAreaControl.Caret.Position =  ((TextEditorControl)this.Control).Document.OffsetToPosition(Math.Min(((TextEditorControl)this.Control).Document.TextLength, Math.Max(0, properties.GetProperty("CaretOffset", ((TextEditorControl)this.Control).ActiveTextAreaControl.Caret.Offset))));

			if (((TextEditorControl)this.Control).Document.HighlightingStrategy.Name != properties.GetProperty("HighlightingLanguage", ((TextEditorControl)this.Control).Document.HighlightingStrategy.Name)) 
			{
				IHighlightingStrategy highlightingStrategy = HighlightingManager.Manager.FindHighlighterByName(properties.GetProperty("HighlightingLanguage", ((TextEditorControl)this.Control).Document.HighlightingStrategy.Name));
				if (highlightingStrategy != null) 
				{
					textEditorControl.Document.HighlightingStrategy = highlightingStrategy;
				}
			}
			textEditorControl.ActiveTextAreaControl.TextArea.TextViewMargin.FirstVisibleLine = properties.GetProperty("VisibleLine", 0);
			
			textEditorControl.Document.FoldingManager.DeserializeFromString(properties.GetProperty("Foldings", ""));

		}
		
		
		#endregion
		
		#region implement the IPrintable interface

		public PrintDocument PrintDocument 
		{
			get 
			{ 
				return textEditorControl.PrintDocument;
			}
		}

		#endregion

		#region FileSystemWatcher
		// KSL Start, New lines
		FileSystemWatcher watcher;
		bool wasChangedExternally = false;
		void SetWatcher()
		{
			try 
			{
				if (this.watcher == null) 
				{
					this.watcher = new FileSystemWatcher();
					this.watcher.Changed += new FileSystemEventHandler(this.OnFileChangedEvent);
				} 
				else 
				{
					this.watcher.EnableRaisingEvents = false;
				}
				this.watcher.Path = Path.GetDirectoryName(((TextEditorControl)this.Control).FileName);
				this.watcher.Filter = Path.GetFileName(((TextEditorControl)this.Control).FileName);
				this.watcher.NotifyFilter = NotifyFilters.LastWrite;
				this.watcher.EnableRaisingEvents = true;
			} 
			catch (Exception) 
			{
				watcher = null;
			}
		}
		void GotFocusEvent(object sender, EventArgs e)
		{
			lock (this) 
			{
				if (wasChangedExternally) 
				{
					wasChangedExternally = false;
					StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
					string message = stringParserService.Parse("${res:NetFocus.DataStructure.DefaultEditor.Gui.Editor.TextEditorDisplayBinding.FileAlteredMessage}", new string[,] {{"File", Path.GetFullPath(((TextEditorControl)this.Control).FileName)}});
					if (MessageBox.Show(message,
						stringParserService.Parse("${res:MainWindow.DialogName}"),
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Question) == DialogResult.Yes) 
					{
						LoadFile(((TextEditorControl)this.Control).FileName);
					} 
					else 
					{
						IsDirty = true;
					}
				}
			}
		}
		
		void OnFileChangedEvent(object sender, FileSystemEventArgs e)
		{
			lock (this) 
			{
				if(e.ChangeType != WatcherChangeTypes.Deleted) 
				{
					wasChangedExternally = true;
					if (((Form)NetFocus.DataStructure.Gui.WorkbenchSingleton.Workbench).Focused) 
					{
						GotFocusEvent(this, EventArgs.Empty);
					}
				}
			}
		}

		// KSL End
		#endregion

		#region implement the IEditor interface
		public string Text 
		{
			get 
			{
				return textEditorControl.Document.TextContent;
			}
			set 
			{
				textEditorControl.Document.TextContent = value;
			}
		}
		
		public void Undo()
		{
			this.textEditorControl.Undo();
		}
		
		public void Redo()
		{
			this.textEditorControl.Redo();
		}
		
		#endregion

		#region implement the IPositionable interface.
		public void JumpTo(int line, int column)
		{
			textEditorControl.ActiveTextAreaControl.JumpTo(line, column);
		}
		
		#endregion

		#region NetFocus.DataStructure.Gui.IClipboardHandler interface implementation

		public bool EnableUndo 
		{
			get 
			{
				return textEditorControl.EnableUndo;
			}
		}
		public bool EnableRedo 
		{
			get 
			{
				return textEditorControl.EnableRedo;
			}
		}


		
		public IClipboardHandler ClipboardHandler 
		{
			get 
			{
				return this;
			}
		}

		public bool EnableCut 
		{
			get 
			{
				return textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCut;
			}
		}
		
		public bool EnableCopy 
		{
			get 
			{
				return textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCopy;
			}
		}
		
		public bool EnablePaste 
		{
			get 
			{
				return textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnablePaste;
			}
		}
		
		public bool EnableDelete 
		{
			get 
			{
				return textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableDelete;
			}
		}
		
		public bool EnableSelectAll 
		{
			get 
			{
				return textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableSelectAll;
			}
		}
		
		public void SelectAll(object sender, System.EventArgs e)
		{
			textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.SelectAll(sender, e);
		}
		
		public void Delete(object sender, System.EventArgs e)
		{
			textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Delete(sender, e);
		}
		
		public void Paste(object sender, System.EventArgs e)
		{
			textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Paste(sender, e);
		}
		
		public void Copy(object sender, System.EventArgs e)
		{
			textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Copy(sender, e);
		}
		
		public void Cut(object sender, System.EventArgs e)
		{
			textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Cut(sender, e);
		}
		#endregion

	}

}
