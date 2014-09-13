using System;
using System.Configuration;
using System.Collections;
using System.Xml;
using System.Reflection;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;

using NetFocus.Components.AddIns.Conditions;
using NetFocus.Components.AddIns.Exceptions;
using NetFocus.Components.AddIns.Codons;

namespace NetFocus.Components.AddIns
{
	/// <summary>
	/// Here is the only point to get an <see cref="IAddInTree"/> object.
	/// </summary>
    public class AddInTreeSingleton : DefaultAddInTree
	{
		static IAddInTree addInTree = null;		
		static string[] addInDirectories       = null;
		
		/// <summary>
		/// Returns an <see cref="IAddInTree"/> object.
		/// </summary>
		public static IAddInTree AddInTree {
			get {
				if (addInTree == null) {
					CreateAddInTree();
				}
				return addInTree;
			}
		}

		public static void SetAddInDirectories(string[] addInDirectories)
		{
			AddInTreeSingleton.addInDirectories = addInDirectories;
		}
		
		
        private static void InsertAddIns(StringCollection addInFiles)
        {
	        foreach (string addInFile in addInFiles) {
		        AddIn addIn = new AddIn();
		        try {
			        addIn.Initialize(addInFile);
			        addInTree.InsertAddIn(addIn);
		        } 

		        catch (Exception e) {
			        throw new AddInInitializeException(addInFile, e);
		        } 
	        }
        }
		
		
		private static void CreateAddInTree()
		{
            //首先创建插件树实例
			addInTree = new DefaultAddInTree();
			
			InternalFileService fileUtilityService = new InternalFileService();	
			StringCollection addInFiles = null;

			if (addInDirectories != null) 
			{
                //扫描每一个插件文件，生成插件并逐个插入到插件树
				foreach(string path in addInDirectories) 
				{
					addInFiles = fileUtilityService.SearchDirectory(Application.StartupPath + Path.DirectorySeparatorChar + path, "*.addin");
					InsertAddIns(addInFiles);
				}
			}
			

		}
	}
}
