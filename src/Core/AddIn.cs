
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Xml;

using NetFocus.Components.AddIns.Codons;
using NetFocus.Components.AddIns.Attributes;

namespace NetFocus.Components.AddIns
{
	/// <summary>
	/// The <code>AddIn</code> class handles the extensibility of the AddInTree by loading
	/// xml descriptions about nodes to insert.
	/// </summary>
	public class AddIn
	{
		string name        = null;
		string author      = null;
		string copyright   = null;
		string url         = null;
		string description = null;
		string version     = null;
		string fileName    = null;
		
		Hashtable        runtimeLibraries       = new Hashtable();  //存放该插件文件中的所有<Runtime>节点指定的程序集
		ArrayList        extensions = new ArrayList();   //存放该插件文件中的所有<Extension>节点指定的功能模块(一个程序扩展)
		
		InternalFileService fileUtilityService = new InternalFileService();
		/// <summary>
		/// returns the filename of the xml definition in which
		/// this AddIn is defined.
		/// </summary>
		public string FileName {
			get {
				return fileName;
			}
		}
		
		/// <summary>
		/// returns the Name of the AddIn
		/// </summary>
		public string Name {
			get {
				return name;
			}
		}
		
		/// <summary>
		/// returns the Author of the AddIn
		/// </summary>
		public string Author {
			get {
				return author;
			}
		}
		
		/// <summary>
		/// returns a copyright string of the AddIn
		/// </summary>
		public string Copyright {
			get {
				return copyright;
			}
		}
		
		/// <summary>
		/// returns a url of the homepage of the plugin
		/// or the author.
		/// </summary>
		public string Url {
			get {
				return url;
			}
		}
		
		/// <summary>
		/// returns a brief description of what the plugin
		/// does.
		/// </summary>
		public string Description {
			get {
				return description;
			}
		}
		
		/// <summary>
		/// returns the version of the plugin.
		/// </summary>
		public string Version {
			get {
				return version;
			}
		}
		
		/// <summary>
		/// returns a hashtable with the runtime libraries
		/// where the key is the assembly name and the value
		/// is the assembly object.
		/// </summary>
		public Hashtable RuntimeLibraries {
			get {
				return runtimeLibraries;
			}
		}
		
		/// <summary>
		/// returns a arraylist with all extensions defined by
		/// this addin.
		/// </summary>
		public ArrayList Extensions {
			get {
				return extensions;
			}
		}
		
		/// <summary>
		/// Initializes this addIn. It loads the xml definition in add in file
		/// </summary>
		public void Initialize(string fileName)
		{
			this.fileName = fileName;
			XmlDocument doc = new XmlDocument();
			doc.Load(fileName);
			
			try {
				name        = doc.SelectSingleNode("AddIn/@name").Value;
				author      = doc.SelectSingleNode("AddIn/@author").Value;
				copyright   = doc.SelectSingleNode("AddIn/@copyright").Value;
				url         = doc.SelectSingleNode("AddIn/@url").Value;
				description = doc.SelectSingleNode("AddIn/@description").Value;
				version     = doc.SelectSingleNode("AddIn/@version").Value;
			} catch (Exception) {
				throw new Exception("No or malformed 'AddIn' node");
			}
			
			foreach (object o in doc.DocumentElement.ChildNodes) {
				if (!(o is XmlElement)) {
					continue;
				}
				XmlElement curEl = (XmlElement)o;
				
				switch (curEl.Name) {
					case "Runtime":
						AddRuntimeLibraries(curEl);
						break;
					case "Extension":
						AddExtensions(curEl);
						break;
				}
			}
		}
		// 此函数分析<Runtime>元素
		void AddRuntimeLibraries(XmlElement el)
		{
			foreach (object o in el.ChildNodes) {
				
				if (!(o is XmlElement)) {
					continue;
				}
				
				XmlElement curEl = (XmlElement)o;
				
				if (curEl.Attributes["assembly"] == null) {
					throw new Exception("One import node has no assembly attribute defined.");
				}
				string assemblyName = curEl.Attributes["assembly"].InnerText;
				string assemblyFullName = fileUtilityService.GetDirectoryNameWithSeparator(Path.GetDirectoryName(fileName)) + assemblyName;
				Assembly asm = AddInTreeSingleton.AddInTree.LoadAssembly(assemblyFullName);
				runtimeLibraries[assemblyName] = asm;
			}
		}
		
		// 此函数分析<Extension>元素
		void AddExtensions(XmlElement el)
		{
			if (el.Attributes["path"] == null) {
				throw new Exception("One extension node has no path attribute defined.");
			}			
			Extension e = new Extension(el.Attributes["path"].InnerText);
			AddCodonsToExtension(e, el);
			extensions.Add(e);
		}
		
		/// <summary>
		/// Autoinitialized all fields of the customizer object to the values
		/// in the codonNode using the XmlMemberAttributeAttribute or XmlMemberArrayAttribute
		/// customizer对象为一个ICondon
		/// </summary>
		void AutoInitializeAttributes(object customizer, XmlNode codonNode)
		{
			Type currentType = customizer.GetType();
			while (currentType != typeof(object)) {
				FieldInfo[] fieldInfoArray = currentType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
				
				foreach (FieldInfo fieldInfo in fieldInfoArray) {
					// process XmlMemberAttribute attributes
					XmlMemberAttributeAttribute codonAttribute = (XmlMemberAttributeAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(XmlMemberAttributeAttribute));
					
					if (codonAttribute != null) {
						// get value from xml file
						XmlNode node = codonNode.SelectSingleNode("@" + codonAttribute.Name);
						
						// check if its required
						if (node == null && codonAttribute.IsRequired) {
							throw new Exception(String.Format("{0} is a required attribute for node '{1}' ", codonAttribute.Name, codonNode.Name));
						}						
						
						if (node != null) {
							if (fieldInfo.FieldType.IsSubclassOf(typeof(System.Enum))) {
								fieldInfo.SetValue(customizer, Convert.ChangeType(Enum.Parse(fieldInfo.FieldType, node.Value), fieldInfo.FieldType));
							} else {
								PathAttribute pathAttribute = (PathAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(PathAttribute));
								if (pathAttribute != null) {
									fieldInfo.SetValue(customizer, fileUtilityService.GetDirectoryNameWithSeparator(Path.GetDirectoryName(fileName)) + Convert.ChangeType(node.Value, fieldInfo.FieldType).ToString());
								} else {
									fieldInfo.SetValue(customizer, Convert.ChangeType(node.Value, fieldInfo.FieldType));
								}
							}
						}
					}
					
					// process XmlMemberArrayAttribute attributes
					XmlMemberArrayAttribute codonArrayAttribute = (XmlMemberArrayAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(XmlMemberArrayAttribute));
					
					if (codonArrayAttribute != null) {
						// get value from xml file
						XmlNode node = codonNode.SelectSingleNode("@" + codonArrayAttribute.Name);
						
						// check if its required
						if (node == null && codonArrayAttribute.IsRequired) {
							throw new ApplicationException(String.Format("{0} is a required attribute.", codonArrayAttribute.Name));
						}
						
						if (node != null) {
							string[] attrArray = node.Value.Split(codonArrayAttribute.Separator);
							// TODO : convert array types (currently only string arrays are supported)
							fieldInfo.SetValue(customizer, attrArray);
						}
					}
					
				}
				currentType = currentType.BaseType;
			}
		}
		
		
		void AddCodonsToExtension(Extension e, XmlElement el)
		{
			foreach (object o in el.ChildNodes) {
				if (!(o is XmlElement)) {
					continue;
				}
				XmlElement curEl = (XmlElement)o;
				
				switch (curEl.Name) {
					default:  //默认为一个代码子节点如:<MenuItem>,<Pad>,等等
						//先创佳一个代码子对象
						ICodon codon = AddInTreeSingleton.AddInTree.CodonFactory.CreateCodon(this, curEl);
						//为一个代码子对象中的所有字段赋值,值从Xml节点CurEl中读取
						AutoInitializeAttributes(codon, curEl);
					
						if (codon.InsertAfter == null && codon.InsertBefore == null && e.CodonCollection.Count > 0) {
							codon.InsertAfter = new string[] { ((ICodon)e.CodonCollection[e.CodonCollection.Count - 1]).ID };
						}
						//最后将当前代码子对象添加到代码子集合中
						e.CodonCollection.Add(codon);
						//下面是,如果当前节点有子节点(如<MenuItem>节点中有子<MenuItem>节点),则对子节点进行递归操作
						if (curEl.ChildNodes.Count > 0) {
							Extension newExtension = new Extension(e.Path + '/' + codon.ID);
							AddCodonsToExtension(newExtension, curEl);
							extensions.Add(newExtension);
						}
						break;
				}
			}
		}
		
		/// <summary>
		/// Creates an object which is related to this Add-In.
		/// </summary>
		/// <exception cref="TypeNotFoundException">
		/// If className could not be created.
		/// </exception>
		public object CreateObject(string className)
		{
			object newInstance;

			foreach (DictionaryEntry library in runtimeLibraries) 
			{
				newInstance = ((Assembly)library.Value).CreateInstance(className);
				if (newInstance != null) 
				{
					return newInstance;
				}
			}
			newInstance = Assembly.GetExecutingAssembly().CreateInstance(className);
				
			if (newInstance == null) 
			{
				newInstance = Assembly.GetCallingAssembly().CreateInstance(className);
			}
			
			if (newInstance == null) 
			{
				throw new Exception("不能从类型 " + className + Environment.NewLine + " 创建对象，请检查 " + fileName );
			}
			
			return newInstance;
		}
		
		/// <summary>
		/// Definies an extension point (path in the tree) with its codons.
		/// </summary>
		public class Extension
		{
			string    path;
			ArrayList codonCollection = new ArrayList();
			
			/// <summary>
			/// returns the path in which the underlying codons are inserted
			/// </summary>
			public string Path {
				get {
					return path;
				}
				set {
					path = value;
				}
			}
			
			/// <summary>
			/// returns a ArrayList with all the codons defined in this extension.
			/// </summary>
			public ArrayList CodonCollection {
				get {
					return codonCollection;
				}
				set {
					codonCollection = value;
				}
			}
			
			/// <summary>
			/// Constructs a new instance.
			/// </summary>
			public Extension(string path)
			{
				this.path = path;
			}
			


		}
	}
}
