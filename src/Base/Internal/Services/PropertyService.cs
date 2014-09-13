
using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using NetFocus.DataStructure.Properties;


namespace NetFocus.DataStructure.Services
{ 
	/// <summary>
	/// 这个服务类处理全局的属性信息.
	/// </summary>
	public class PropertyService : DefaultProperties, IService
	{
		
		readonly static string propertyFileName    = "DataStructureProperties.xml";
		readonly static string propertyFileVersion = "1.1";
		readonly static string propertyXmlRootNodeName  = "DataStructureProperties";
		readonly static string defaultPropertyDirectory = Application.StartupPath + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "data" + Path.DirectorySeparatorChar + "options" + Path.DirectorySeparatorChar;
		readonly static string defaultDataDirectory = Application.StartupPath + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "data" + Path.DirectorySeparatorChar;
		/// <summary>
		/// 返回属性配置文件的路径.
		/// </summary>
		public string ConfigDirectory {
			get {
				return defaultPropertyDirectory;
			}
		}
		/// <summary>
		/// 返回数据文件夹的路径.
		/// </summary>
		public string DataDirectory 
		{
			get 
			{
				return defaultDataDirectory;
			}
		}
		
		
		public PropertyService()
		{
			try {
				LoadProperties();
			}
			catch {
				MessageBox.Show("不能加载属性文件!", "警告",MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}
		
		
		void WritePropertiesToFile(string fileName)
		{
			//创建一个新的Xml文档
			XmlDocument doc = new XmlDocument();
			//为该文档添加文件头Xml指令和文件根节点信息
			doc.LoadXml("<?xml version=\"1.0\"?>\n<" + propertyXmlRootNodeName + " fileversion = \"" + propertyFileVersion + "\" />");
			
			doc.DocumentElement.AppendChild(ToXmlElement(doc));
			try
			{
				doc.Save(fileName);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}
		
		bool LoadPropertiesFromFile(string filename)
		{
			try {
				XmlDocument doc = new XmlDocument();
				doc.Load(filename);
				
				if (doc.DocumentElement.Attributes["fileversion"].InnerText != propertyFileVersion) {
					return false;//如果文件版本不匹配,则返回false;
				}
				SetValueFromXmlElement(doc.DocumentElement["Properties"]);
			} catch {
				return false;
			}
			return true;
		}
		

		void LoadProperties()
		{
			if (!LoadPropertiesFromFile(defaultPropertyDirectory + propertyFileName)) 
			{
				throw new Exception("不能加载全局属性文件!");//加载错误则抛出异常.
			}
		}
		
		void SaveProperties()
		{
			WritePropertiesToFile(defaultPropertyDirectory + propertyFileName);
		}
		
		
		#region 实现IService接口

		public virtual void InitializeService()
		{
			OnInitialize(EventArgs.Empty);
		}
		
		public virtual void UnloadService()
		{
			// 当当前服务退出时,保存所有的属性.
			SaveProperties();
			OnUnload(EventArgs.Empty);//最后触发事件,以便相应的处理程序会被调用到.
		}
		
		protected virtual void OnInitialize(EventArgs e)
		{
			if (Initialize != null) {
				Initialize(this, e);
			}
		}
		
		protected virtual void OnUnload(EventArgs e)
		{
			if (Unload != null) {
				Unload(this, e);
			}
		}
		
		public event EventHandler Initialize;
		public event EventHandler Unload;	
		
		#endregion
	}
}
