
using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;


namespace NetFocus.DataStructure.Services
{
	public enum DriveType {
		Unknown     = 0,
		NoRoot      = 1,
		Removeable  = 2,
		Fixed       = 3,
		Remote      = 4,
		Cdrom       = 5,
		Ramdisk     = 6
	}
	
	public enum FileErrorPolicy {
		Inform,
		ProvideAlternative
	}
	
	public enum FileOperationResult {
		OK,
		Failed,
		SavedAlternatively
	}
	

	public delegate void FileOperationDelegate();
	public delegate void NamedFileOperationDelegate(string fileName);
	
	/// <summary>
	/// 此服务类处理一些底层的文件操作.
	/// </summary>
	public class FileUtilityService : AbstractService
	{
		readonly static char[] separators = {Path.DirectorySeparatorChar, Path.VolumeSeparatorChar};
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		
		public override void InitializeService()
		{
			base.InitializeService();
		}
		
		
		#region NativeMethods
		
		class NativeMethods 
		{
			[DllImport("kernel32.dll", SetLastError=true)]
			public static extern int GetVolumeInformation(string volumePath,
				StringBuilder volumeNameBuffer,
				int volNameBuffSize,
				ref int volumeSerNr,
				ref int maxComponentLength,
				ref int fileSystemFlags,
				StringBuilder fileSystemNameBuffer,
				int fileSysBuffSize);
			
			[DllImport("kernel32.dll")]
			public static extern DriveType GetDriveType(string driveName);
		}

		#endregion

		public string VolumeLabel(string volumePath)
		{
			try {
				StringBuilder volumeName  = new StringBuilder(128);
				int dummyInt = 0;
				NativeMethods.GetVolumeInformation(volumePath,
				                                   volumeName,
				                                   128,
				                                   ref dummyInt,
				                                   ref dummyInt,
				                                   ref dummyInt,
				                                   null,
				                                   0);
				return volumeName.ToString();
			} catch (Exception) {
				return String.Empty;
			}
		}
		
		public DriveType GetDriveType(string driveName)
		{
			return NativeMethods.GetDriveType(driveName);
		}
		
		
		public StringCollection SearchDirectory(string directory, string filemask, bool searchSubdirectories)
		{
			StringCollection collection = new StringCollection();
			SearchDirectory(directory, filemask, collection, searchSubdirectories);
			return collection;
		}
		
		public StringCollection SearchDirectory(string directory, string filemask)
		{
			return SearchDirectory(directory, filemask, true);
		}
		
		void SearchDirectory(string directory, string filemask, StringCollection collection, bool searchSubdirectories)
		{
			try {
				string[] file = Directory.GetFiles(directory, filemask);
				foreach (string f in file) {
					collection.Add(f);
				}
				
				if (searchSubdirectories) {
					string[] dir = Directory.GetDirectories(directory);
					foreach (string d in dir) {
						SearchDirectory(d, filemask, collection, searchSubdirectories);
					}
				}
			} catch (Exception e) {
				MessageBox.Show("Can't access directory " + directory + " reason:\n" + e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		

		public string AbsoluteToRelativePath(string baseDirectoryPath, string absPath)
		{
			string[] bPath = baseDirectoryPath.Split(separators);
			string[] aPath = absPath.Split(separators);
			int indx = 0;
			for(; indx < Math.Min(bPath.Length, aPath.Length); ++indx){
				if(!bPath[indx].Equals(aPath[indx]))
					break;
			}
			
			if (indx == 0) {
				return absPath;
			}
			
			string erg = "";
			
			if(indx == bPath.Length) {
				erg += "." + Path.DirectorySeparatorChar;
			} else {
				for (int i = indx; i < bPath.Length; ++i) {
					erg += ".." + Path.DirectorySeparatorChar;
				}
			}
			erg += String.Join(Path.DirectorySeparatorChar.ToString(), aPath, indx, aPath.Length-indx);
			
			return erg;
		}
		
		public string RelativeToAbsolutePath(string baseDirectoryPath, string relPath)
		{
			if (separators[0] != separators[1] && relPath.IndexOf(separators[1]) != -1) {
				return relPath;
			}
			string[] bPath = baseDirectoryPath.Split(separators[0]);
			string[] rPath = relPath.Split(separators[0]);
			int indx = 0;
			
			for (; indx < rPath.Length; ++indx) {
				if (!rPath[indx].Equals("..")) {
					break;
				}
			}
			
			if (indx == 0) {
				return baseDirectoryPath + separators[0] + String.Join(Path.DirectorySeparatorChar.ToString(), rPath, 1, rPath.Length-1);
			}
			
			string erg = String.Join(Path.DirectorySeparatorChar.ToString(), bPath, 0, bPath.Length-indx);
			
			erg += separators[0] + String.Join(Path.DirectorySeparatorChar.ToString(), rPath, indx, rPath.Length-indx);
			
			return erg;
		}
		
		
		public bool IsValidFileName(string fileName)
		{
			// Fixme: 260 is the hardcoded maximal length for a path on my Windows XP system
			if (fileName == null || fileName.Length == 0 || fileName.Length >= 260) {
				return false;
			}
			
			// platform independend : check for invalid path chars
			foreach (char invalidChar in Path.GetInvalidPathChars()) {
				if (fileName.IndexOf(invalidChar) >= 0) {
					return false;
				}
			}
			
			// platform dependend : Check for invalid file names (DOS)
			// this routine checks for follwing bad file names :
				// CON, PRN, AUX, NUL, COM1-9 and LPT1-9
				string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			if (nameWithoutExtension != null) {
				nameWithoutExtension = nameWithoutExtension.ToUpper();
			}
			if (nameWithoutExtension == "CON" ||
			    nameWithoutExtension == "PRN" ||
			    nameWithoutExtension == "AUX" ||
			    nameWithoutExtension == "NUL") {
			    	return false;
			    }
			    
			    char ch = nameWithoutExtension.Length == 4 ? nameWithoutExtension[3] : '\0';
			
			return !((nameWithoutExtension.StartsWith("COM") ||
			          nameWithoutExtension.StartsWith("LPT")) &&
			          Char.IsDigit(ch));
		}
		
		public bool TestFileExists(string filename)
		{
			if (!File.Exists(filename)) {
				StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
				
				MessageBox.Show(stringParserService.Parse(resourceService.GetString("Fileutility.CantFindFileError"), new string[,] { {"FILE",  filename} }),
				                resourceService.GetString("Global.WarningText"),
				                MessageBoxButtons.OK,
				                MessageBoxIcon.Warning);
				return false;
			}
			return true;
		}
		
		public bool IsDirectory(string filename)
		{
			FileAttributes attr = File.GetAttributes(filename);
			return (attr & FileAttributes.Directory) != 0;
		}
		

		public string GetDirectoryNameWithSeparator(string directoryName)
		{
			if (directoryName.EndsWith(Path.DirectorySeparatorChar.ToString())) {
				return directoryName;
			}
			return directoryName + Path.DirectorySeparatorChar;
		}
		
		
		//定义一些受保护的加载文件和保存文件的方�?具体执行的函数是通过委托实例来执行的.
		#region ObservedSaveOrLoad Methods

		public FileOperationResult ObservedSave(FileOperationDelegate saveFile, string fileName, string message, FileErrorPolicy policy)
		{
			Debug.Assert(IsValidFileName(fileName));
			try 
			{
				saveFile();
				return FileOperationResult.OK;
			} 
			catch (Exception e) 
			{
				switch (policy) 
				{
					case FileErrorPolicy.Inform:
						using (SaveErrorInformDialog informDialog = new SaveErrorInformDialog(fileName, message, "Error while saving", e)) 
						{
							informDialog.ShowDialog();
						}
						break;
					case FileErrorPolicy.ProvideAlternative:
						using (SaveErrorChooseDialog chooseDialog = new SaveErrorChooseDialog(fileName, message, "Error while saving", e, false)) 
						{
							switch (chooseDialog.ShowDialog()) 
							{
								case DialogResult.OK: // choose location (never happens here)
									break;
								case DialogResult.Retry:
									return ObservedSave(saveFile, fileName, message, policy);
								case DialogResult.Ignore:
									return FileOperationResult.Failed;
							}
						}
						break;
				}
			}
			return FileOperationResult.Failed;
		}
		
		public FileOperationResult ObservedSave(FileOperationDelegate saveFile, string fileName, FileErrorPolicy policy)
		{
			return ObservedSave(saveFile,
				fileName,
				resourceService.GetString("NetFocus.Services.FileUtilityService.CantSaveFileStandardText"),
				policy);
		}
		
		public FileOperationResult ObservedSave(FileOperationDelegate saveFile, string fileName)
		{
			return ObservedSave(saveFile, fileName, FileErrorPolicy.Inform);
		}
		
		
		public FileOperationResult ObservedSave(NamedFileOperationDelegate saveFileAs, string fileName, string message, FileErrorPolicy policy)
		{
			Debug.Assert(IsValidFileName(fileName));
			try 
			{
				saveFileAs(fileName);
				return FileOperationResult.OK;
			} 
			catch (Exception e) 
			{
				switch (policy) 
				{
					case FileErrorPolicy.Inform:
						using (SaveErrorInformDialog informDialog = new SaveErrorInformDialog(fileName, message, "Error while saving", e)) 
						{
							informDialog.ShowDialog();
						}
						break;
					case FileErrorPolicy.ProvideAlternative:
						restartlabel:
							using (SaveErrorChooseDialog chooseDialog = new SaveErrorChooseDialog(fileName, message, "Error while saving", e, true)) 
							{
								switch (chooseDialog.ShowDialog()) 
								{
									case DialogResult.OK:
										using (SaveFileDialog fdiag = new SaveFileDialog()) 
										{
											fdiag.OverwritePrompt = true;
											fdiag.AddExtension    = true;
											fdiag.CheckFileExists = false;
											fdiag.CheckPathExists = true;
											fdiag.Title           = "Choose alternate file name";
											fdiag.FileName        = fileName;
											if (fdiag.ShowDialog() == DialogResult.OK) 
											{
												return ObservedSave(saveFileAs, fdiag.FileName, message, policy);
											} 
											else 
											{
												goto restartlabel;
											}
										}
									case DialogResult.Retry:
										return ObservedSave(saveFileAs, fileName, message, policy);
									case DialogResult.Ignore:
										return FileOperationResult.Failed;
								}
							}
						break;
				}
			}
			return FileOperationResult.Failed;
		}
		
		public FileOperationResult ObservedSave(NamedFileOperationDelegate saveFileAs, string fileName, FileErrorPolicy policy)
		{
			return ObservedSave(saveFileAs,
				fileName,
				resourceService.GetString("NetFocus.Services.FileUtilityService.CantSaveFileStandardText"),
				policy);
		}
		
		public FileOperationResult ObservedSave(NamedFileOperationDelegate saveFileAs, string fileName)
		{
			return ObservedSave(saveFileAs, fileName, FileErrorPolicy.Inform);
		}
		

		class LoadWrapper
		{
			NamedFileOperationDelegate loadNamedFileDelegate;
			string fileName;
			
			public LoadWrapper(NamedFileOperationDelegate loadNamedFileDelegate, string fileName)
			{
				this.loadNamedFileDelegate = loadNamedFileDelegate;
				this.fileName   = fileName;
			}
			public void Invoke()
			{
				loadNamedFileDelegate(fileName);
			}
		}
		

		public FileOperationResult ObservedLoad(FileOperationDelegate loadFileDelegate, string fileName, string message, FileErrorPolicy policy)
		{
			Debug.Assert(IsValidFileName(fileName));
			//Eventually do the load file action.
			try 
			{
				loadFileDelegate();//通过这个委托来真正最后调用实际的方法,所有的ObservedLoad方法
				//无论是NamedFileOperation还是FileOperation最终都会通过这个委托来调用所对应的方�?
				return FileOperationResult.OK;
			} 
			catch (Exception e) 
			{
				switch (policy) 
				{
					case FileErrorPolicy.Inform:
						using (SaveErrorInformDialog informDialog = new SaveErrorInformDialog(fileName, message, "Error while loading", e)) 
						{
							informDialog.ShowDialog();
						}
						break;
					case FileErrorPolicy.ProvideAlternative:
						using (SaveErrorChooseDialog chooseDialog = new SaveErrorChooseDialog(fileName, message, "Error while loading", e, false)) 
						{
							switch (chooseDialog.ShowDialog()) 
							{
								case DialogResult.OK: // choose location (never happens here)
									break;
								case DialogResult.Retry:
									return ObservedLoad(loadFileDelegate, fileName, message, policy);
								case DialogResult.Ignore:
									return FileOperationResult.Failed;
							}
						}
						break;
				}
			}
			return FileOperationResult.Failed;
		}
		
		public FileOperationResult ObservedLoad(FileOperationDelegate loadFileDelegate, string fileName, FileErrorPolicy policy)
		{
			string cantLoadFileStandardText = resourceService.GetString("NetFocus.Services.FileUtilityService.CantLoadFileStandardText");

			return ObservedLoad(loadFileDelegate,fileName,cantLoadFileStandardText,policy);
		}
		
		public FileOperationResult ObservedLoad(FileOperationDelegate loadFileDelegate, string fileName)
		{
			return ObservedLoad(loadFileDelegate, fileName, FileErrorPolicy.Inform);
		}
		
		
		public FileOperationResult ObservedLoad(NamedFileOperationDelegate loadNamedFileDelegate, string fileName, string message, FileErrorPolicy policy)
		{
			LoadWrapper loadWrapper = new LoadWrapper(loadNamedFileDelegate, fileName);
			FileOperationDelegate fileOperationDelegate = new FileOperationDelegate(loadWrapper.Invoke);
			return ObservedLoad(fileOperationDelegate, fileName, message, policy);
		}
		
		public FileOperationResult ObservedLoad(NamedFileOperationDelegate loadFileAsDelegate, string fileName, FileErrorPolicy policy)
		{
			string cantLoadFileStandardText = resourceService.GetString("NetFocus.Services.FileUtilityService.CantLoadFileStandardText");

			return ObservedLoad(loadFileAsDelegate,fileName,cantLoadFileStandardText,policy);
		}
		
		public FileOperationResult ObservedLoad(NamedFileOperationDelegate loadFileAsDelegate, string fileName)
		{
			return ObservedLoad(loadFileAsDelegate, fileName, FileErrorPolicy.Inform);
		}

		
		#endregion
	}

}
