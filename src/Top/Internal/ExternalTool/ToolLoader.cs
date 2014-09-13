

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Xml;
using System.Windows.Forms;

using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Services;

namespace NetFocus.DataStructure.Internal.ExternalTool
{
	/// <summary>
	/// This class handles the external tools 
	/// </summary>
	public class ToolLoader
	{
		static string TOOLFILE        = "DataStructure-Tools.xml";
		static string TOOLFILEVERSION = "1";
		
		static ArrayList tool         = new ArrayList();
		
		public static  ArrayList Tool {
			get {
				return tool;
			}
			set {
				tool = value;
			}
		}
		
		static bool LoadToolsFromStream(string filename)
		{
			XmlDocument doc = new XmlDocument();
			try {
				doc.Load(filename);
				
				if (doc.DocumentElement.Attributes["VERSION"].InnerText != TOOLFILEVERSION)
					return false;
				
				tool = new ArrayList();
				
				XmlNodeList nodes  = doc.DocumentElement.ChildNodes;
				foreach (XmlElement el in nodes)
					tool.Add(new ExternalTool(el));
			} catch (Exception) {
				return false;
			}
			return true;
		}
		
		static void WriteToolsToFile(string fileName)
		{
			XmlDocument doc    = new XmlDocument();
			doc.LoadXml("<TOOLS VERSION = \"" + TOOLFILEVERSION + "\" />");
			
			foreach (ExternalTool et in tool) {
				doc.DocumentElement.AppendChild(et.ToXmlElement(doc));
			}
			
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			fileUtilityService.ObservedSave(new NamedFileOperationDelegate(doc.Save), fileName, FileErrorPolicy.ProvideAlternative);
		}
		
		/// <summary>
		/// This method loads the external tools from a XML based
		/// configuration file.
		/// </summary>
		static ToolLoader()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			if (!LoadToolsFromStream(propertyService.ConfigDirectory + TOOLFILE)) {
				Console.WriteLine("Tools: can't load user defaults, reading system defaults");
				if (!LoadToolsFromStream(Application.StartupPath + 
				                         Path.DirectorySeparatorChar + ".." + 
				                         Path.DirectorySeparatorChar + "data" + 
				                         Path.DirectorySeparatorChar + "options" + 
				                         Path.DirectorySeparatorChar + TOOLFILE)) {
					ResourceService ResourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
					MessageBox.Show(ResourceService.GetString("Internal.ExternalTool.CantLoadToolConfigWarining"), ResourceService.GetString("Global.WarningText"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
		}
		
		/// <summary>
		/// This method saves the external tools to a XML based
		/// configuration file in the current user's own files directory
		/// </summary>
		public static void SaveTools()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			WriteToolsToFile(propertyService.ConfigDirectory + TOOLFILE);
		}
	}
}
