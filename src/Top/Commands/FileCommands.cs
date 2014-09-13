using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using NetFocus.Components.AddIns;

using NetFocus.DataStructure.Properties;
using NetFocus.Components.AddIns.Codons;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Gui.Dialogs;
using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.Internal.Algorithm;

namespace NetFocus.DataStructure.Commands
{

	public class CreateNewFile : AbstractMenuCommand
	{
		public override void Run()
		{
			using (NewFileDialog nfd = new NewFileDialog()) 
			{
				nfd.Owner = (Form)WorkbenchSingleton.Workbench;
				nfd.ShowDialog();
			}
			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			if(content != null)
			{
				content.ViewSelected += AlgorithmManager.Algorithms.ClearPadsHandler;
				content.SelectView();
				AlgorithmManager.Algorithms.Timer.Enabled = false;
			}
		}
	}
	public class OpenFile : AbstractMenuCommand
	{
		public override void Run()
		{
			using (OpenFileDialog fdiag  = new OpenFileDialog()) 
			{
				fdiag.AddExtension    = true;
				
				string[] fileFilters  = (string[])(AddInTreeSingleton.AddInTree.GetTreeNode("/DataStructure/Workbench/FileFilter").BuildChildItems(this)).ToArray(typeof(string));
				fdiag.Filter          = String.Join("|", fileFilters);
				fdiag.FilterIndex = fileFilters.GetLength(0);
				bool foundFilter      = false;

				if (!foundFilter) 
				{
					IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
					if (content != null) 
					{
						for (int i = 0; i < fileFilters.Length; ++i) 
						{
							if (fileFilters[i].IndexOf(Path.GetExtension(content.ContentName == null ? content.UntitledName : content.ContentName)) >= 0) 
							{
								fdiag.FilterIndex = i + 1;
								break;
							}
						}
					}
				}
				
				fdiag.Multiselect     = true;
				fdiag.CheckFileExists = true;
				
				if (fdiag.ShowDialog() == DialogResult.OK) 
				{
					IFileService fileService = (IFileService)NetFocus.DataStructure.Services.ServiceManager.Services.GetService(typeof(IFileService));
					foreach (string name in fdiag.FileNames) 
					{
						fileService.OpenFile(name);
						IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
						if(content != null)
						{
							content.ViewSelected -= AlgorithmManager.Algorithms.ClearPadsHandler;
							content.ViewSelected += AlgorithmManager.Algorithms.ClearPadsHandler;
						}
					}
					WorkbenchSingleton.Workbench.ActiveViewContent.SelectView();	
					AlgorithmManager.Algorithms.Timer.Enabled = false;
				}
			}
		}
	}

	public class CloseFile : AbstractMenuCommand
	{
		public override void Run()
		{
			if (WorkbenchSingleton.Workbench.ActiveViewContent != null) 
			{
				WorkbenchSingleton.Workbench.ActiveViewContent.CloseView(false);
				//WorkbenchSingleton.Workbench.CloseView(WorkbenchSingleton.Workbench.ActiveViewContent);
			}
		}
	}

	public class SaveFile : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			if (content != null) 
			{
				if (content.IsViewOnly) 
				{
					return;
				}
				if (content.ContentName == null) 
				{
					SaveFileAs sfa = new SaveFileAs();
					sfa.Run();
				} 
				else 
				{
					FileAttributes attr = FileAttributes.ReadOnly | FileAttributes.Directory | FileAttributes.Offline | FileAttributes.System;
					if ((File.GetAttributes(content.ContentName) & attr) != 0) 
					{
						SaveFileAs sfa = new SaveFileAs();
						sfa.Run();
					} 
					else 
					{
						FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
						fileUtilityService.ObservedSave(new FileOperationDelegate(content.SaveFile), content.ContentName);
					}
				}

			}
		}
	} 

	public class ReloadFile : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (content != null && content.ContentName != null) 
			{
				IXmlConvertable memento = null;
				if (content is IMementoCapable) 
				{
					memento = ((IMementoCapable)content).CreateMemento();
				}
				content.LoadFile(content.ContentName);
				
				if (memento != null) 
				{
					((IMementoCapable)content).SetMemento(memento);
				}
			}
		}
	}
	
	public class SaveFileAs : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (content != null) 
			{
				if (content.IsViewOnly) 
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
						if (fileFilters[i].IndexOf(Path.GetExtension(content.ContentName == null ? content.UntitledName : content.ContentName)) >= 0) 
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
						if (fileUtilityService.ObservedSave(new NamedFileOperationDelegate(content.SaveFile), fileName) == FileOperationResult.OK) 
						{
							fileService.RecentOpenMemeto.AddLastFile(fileName);
							//MessageBox.Show(fileName, "文件已保存", MessageBoxButtons.OK);
						}
					}
				}
			}
		}
	}
	
	public class SaveAllFiles : AbstractMenuCommand
	{
		public override void Run()
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) 
			{
				if (content.IsViewOnly) 
				{
					continue;
				}
				
				if (content.ContentName == null) 
				{
					using (SaveFileDialog fdiag = new SaveFileDialog()) 
					{
						fdiag.OverwritePrompt = true;
						fdiag.AddExtension    = true;
						
						fdiag.Filter          = String.Join("|", (string[])(AddInTreeSingleton.AddInTree.GetTreeNode("/DataStructure/Workbench/FileFilter").BuildChildItems(this)).ToArray(typeof(string)));
						
						if (fdiag.ShowDialog() == DialogResult.OK) 
						{
							string fileName = fdiag.FileName;
							if (Path.GetExtension(fileName).StartsWith("?") || Path.GetExtension(fileName) == "*") 
							{
								fileName = Path.ChangeExtension(fileName, "");
							}
							if (fileUtilityService.ObservedSave(new NamedFileOperationDelegate(content.SaveFile), fileName) == FileOperationResult.OK) 
							{
								//MessageBox.Show(fileName, "文件已保存", MessageBoxButtons.OK);
							}
						}
					}
				} 
				else 
				{
					fileUtilityService.ObservedSave(new FileOperationDelegate(content.SaveFile), content.ContentName);
				}
			}
		}
	}
	
	
	public class ExitWorkbenchCommand : AbstractMenuCommand
	{
		public override void Run()
		{			
			((Form)WorkbenchSingleton.Workbench).Close();
		}
	}
	
	public class Print : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (content != null) 
			{
				if (content is IPrintable) 
				{
					PrintDocument pdoc = ((IPrintable)content).PrintDocument;
					if (pdoc != null) 
					{
						using (PrintDialog ppd = new PrintDialog()) 
						{
							ppd.Document  = pdoc;
							ppd.AllowSomePages = true;
							if (ppd.ShowDialog() == DialogResult.OK) 
							{ // fixed by Roger Rubin
								pdoc.Print();
							}
						}
					} 
					else 
					{
						MessageBox.Show("Couldn't create PrintDocument", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				} 
				else 
				{
					MessageBox.Show("Can't print this content content", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
	}
	
	public class PrintPreview : AbstractMenuCommand
	{
		public override void Run()
		{
			try 
			{
				IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
				
				if (content != null) 
				{
					if (content is IPrintable) 
					{
						using (PrintDocument pdoc = ((IPrintable)content).PrintDocument) 
						{
							if (pdoc != null) 
							{
								PrintPreviewDialog ppd = new PrintPreviewDialog();
								ppd.Owner     = (Form)WorkbenchSingleton.Workbench;
								ppd.TopMost   = true;
								ppd.Document  = pdoc;
								ppd.Show();
							} 
							else 
							{
								MessageBox.Show("Couldn't create PrintDocument", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
							}
						}
					}
				}
			} 
			catch (System.Drawing.Printing.InvalidPrinterException) 
			{
			}
		}
	}

	public class ClearRecentFiles : AbstractMenuCommand
	{
		public override void Run()
		{			
			try 
			{
				IFileService fileService = (IFileService)NetFocus.DataStructure.Services.ServiceManager.Services.GetService(typeof(IFileService));
				fileService.RecentOpenMemeto.ClearRecentFiles();
			} 
			catch {}
		}
	}
}
