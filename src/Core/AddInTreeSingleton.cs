
using System;
using System.Configuration;
using System.Reflection;
using System.Collections.Specialized;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Xml;

using NetFocus.Components.AddIns.Codons;

namespace NetFocus.Components.AddIns
{
	/// <summary>
	/// Here is the ONLY point to get an <see cref="IAddInTree"/> object,the example of singleton design pattern
	/// </summary>
	public class AddInTreeSingleton : DefaultAddInTree,IConfigurationSectionHandler
	{
		static IAddInTree addInTree = null;
		static bool ignoreDefaultCoreAddInDirectory = false;
		static string[] addInDirectories       = null;
		readonly static string defaultCoreAddInDirectory = Application.StartupPath + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "AddIns";

		public static IAddInTree AddInTree 
		{
			get {
				if (addInTree == null) { //这里采用了惰性初始化
					CreateAddInTree();
				}
				return addInTree;
			}
		}

		
		public static bool SetAddInDirectories(string[] addInDirectories, bool ignoreDefaultCoreAddInDirectory)
		{
			if (addInDirectories == null || addInDirectories.Length < 1) 
			{
				// 路径为空,返回.
				return false;
			}
			AddInTreeSingleton.addInDirectories = addInDirectories;
			AddInTreeSingleton.ignoreDefaultCoreAddInDirectory = ignoreDefaultCoreAddInDirectory;
			return true;
		}

		
		public static string[] GetAddInDirectories(out bool ignoreDefaultAddInPath)
		{
			//通过这句话来调用IConfigurationSectionHandler接口中定义地Create函数
			ArrayList addInDirs = ConfigurationManager.GetSection("AddInDirectories") as ArrayList;

			if (addInDirs != null) 
			{
				int count = addInDirs.Count;
				if (count <= 1) //如果连一个都没有指定自定义插件文件的路径
				{
					ignoreDefaultAddInPath = false;
					return null;
				}
				ignoreDefaultAddInPath = (bool) addInDirs[0];

				string [] directories = new string[count-1];
				for (int i = 0; i < count - 1; i++) 
				{
					directories[i] = addInDirs[i+1] as string;//数组中元素前移.
				}
				return directories;
			}
			ignoreDefaultAddInPath = false;
			return null;
		}
		
		
		/// <summary>
		/// Initialize all addIn object from addIn file collection, and insert them into the addIn tree.
		/// </summary>
		static void InsertAddIns(StringCollection addInFiles)
		{
			foreach (string addInFile in addInFiles) 
			{
				AddIn addIn = new AddIn();//先新建一个插件实例
				try 
				{
					addIn.Initialize(addInFile);//通过当前插件文件来初始化这个插件实例
					addInTree.InsertAddIn(addIn);//将这个初始化好的插件插入到插件树中
				}
				catch (Exception e) 
				{
					throw new Exception("初始化插件文件 " + addInFile + " 时出错 : \n" + e.Message, e);
				}
			}
			
		}

		
		/// <summary>
		/// Create a addIn tree
		/// </summary>
		public static void CreateAddInTree()
		{
			if(addInTree == null)
			{
				addInTree = new DefaultAddInTree();
			
				InternalFileService fileUtilityService = new InternalFileService();

				StringCollection addInFiles = null;
			
				if (ignoreDefaultCoreAddInDirectory == false) //如果没有忽略默认的插件路径,即采用默认的插件路径
				{
					addInFiles = fileUtilityService.SearchDirectory(defaultCoreAddInDirectory, "*.addin");
					InsertAddIns(addInFiles);
				}
				else  //如果忽略默认的插件文件的路径
				{
					if (addInDirectories != null) 
					{
						foreach(string path in addInDirectories) 
						{
							addInFiles = fileUtilityService.SearchDirectory(Application.StartupPath + Path.DirectorySeparatorChar + path, "*.addin");
							InsertAddIns(addInFiles);
						}
					}
				}
			}

		}
		
		
		#region IConfigurationSectionHandler 成员
		
		//此函数在本应用程序中读取配置文件中地<AddInDirectories>元素
		public object Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			ArrayList addInDirectories = new ArrayList();
			XmlNode attr = section.Attributes.GetNamedItem("ignoreDefaultPath");

			if (attr != null) 
			{
				try 
				{
					addInDirectories.Add(Convert.ToBoolean(attr.Value));
				} 
				catch (InvalidCastException) 
				{
					addInDirectories.Add(false);//如果文件读取异常,则默认设置为假,即不忽略默认的插件文件路径
				}
			} 
			else 
			{
				addInDirectories.Add(false);//如果该属性不存在同样默认设置为假,即不忽略默认的插件文件路径
			}
               
			XmlNodeList addInDirList = section.SelectNodes("AddInDirectory");//读取子节点

			foreach (XmlNode addInDir in addInDirList) 

			{
				XmlNode path = addInDir.Attributes.GetNamedItem("path");
				if (path != null) 
				{
					addInDirectories.Add(path.Value);//将所有的自定义插件文件的路径字符串加到addInDirectories中
				}
			}
			return addInDirectories;//最后返回自定义插件文件的路径字符串以及是否要忽略默认插件文件路径的一个布尔值.

		}

		
		#endregion

	}
}
