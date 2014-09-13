
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Xml;
using System.Xml.Schema;
using System.Text;
using System.Windows.Forms;

using NetFocus.Components.AddIns.Conditions;
using NetFocus.Components.AddIns.Attributes;
using NetFocus.Components.AddIns.Codons;
using NetFocus.Components.AddIns.Exceptions;

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
		Hashtable runtimeLibraries = new Hashtable();
		ArrayList extensions = new ArrayList();
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
		/// Initializes this addIn. It loads the xml definition in file
		/// fileName.
		/// </summary>
		public void Initialize(string fileName)
		{
			this.fileName = fileName;
			XmlDocument doc = new XmlDocument();
			doc.Load(fileName);
			
			try {
				name        = doc.DocumentElement.Attributes["name"].InnerText;
				author      = doc.DocumentElement.Attributes["author"].InnerText;
				copyright   = doc.DocumentElement.Attributes["copyright"].InnerText;
				url         = doc.DocumentElement.Attributes["url"].InnerText;
				description = doc.DocumentElement.Attributes["description"].InnerText;
				version     = doc.DocumentElement.Attributes["version"].InnerText;
			} catch (Exception) {
				throw new AddInLoadException("No or malformed 'AddIn' node");
			}
			
			foreach (object o in doc.DocumentElement.ChildNodes) {
				if (o is XmlElement) {
					XmlElement curEl = (XmlElement)o;
					
					switch (curEl.Name) {
						case "Runtime":
							AddRuntimeLibraries(Path.GetDirectoryName(fileName), curEl);
							break;
						case "Extension":
							AddExtensions(curEl);
							break;
					}
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
				MessageBox.Show("Type not found: " + className + ". Please check : " + fileName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
			
			return newInstance;
		}
		
		
		void AddRuntimeLibraries(string path, XmlElement el)
		{
			foreach (object o in el.ChildNodes) {
				if (o is XmlElement){
					XmlElement curEl = (XmlElement)o;
					
					if (curEl.Attributes["assembly"] == null) {
						throw new AddInLoadException("One import node has no assembly attribute defined.");
					}
					string assemblyName = curEl.Attributes["assembly"].InnerText;
					string pathName     = Path.IsPathRooted(assemblyName) ? assemblyName : fileUtilityService.GetDirectoryNameWithSeparator(path) + assemblyName;
					Assembly asm = AddInTreeSingleton.AddInTree.LoadAssemblyFromFile(pathName);
					RuntimeLibraries[assemblyName] = asm;
				}
			}
		}
		
		void AddExtensions(XmlElement el)
		{
			if (el.Attributes["path"] == null) {
				throw new AddInLoadException("One extension node has no path attribute defined.");
			}			
			Extension e = new Extension(el.Attributes["path"].InnerText);
			AddCodonsToExtension(e, el, new ConditionCollection());//注意:这是一个递归的方法
			extensions.Add(e);
		}
		
		
		#region 一些辅助的私有方法
		
		/// <summary>
		/// Autoinitialized all fields of the customizer object to the values
		/// in the codonNode using the XmlMemberAttributeAttribute.
		/// </summary>
		void AutoInitializeAttributes(object obj, XmlNode codonNode)
		{
			Type currentType = obj.GetType();
			while (currentType != typeof(object)) {
				FieldInfo[] fieldInfoArray = currentType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
				
				foreach (FieldInfo fieldInfo in fieldInfoArray) {
					// process XmlMemberAttributeAttribute attributes
					XmlMemberAttributeAttribute codonAttribute = (XmlMemberAttributeAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(XmlMemberAttributeAttribute));
					
					if (codonAttribute != null) {
						// get value from xml file
						XmlNode node = codonNode.SelectSingleNode("@" + codonAttribute.Name);
						
						// check if its required
						if (node == null && codonAttribute.IsRequired) {
							throw new AddInLoadException(String.Format("{0} is a required attribute for node '{1}' ", codonAttribute.Name, codonNode.Name));
						}						
						
						if (node != null) {
							if (fieldInfo.FieldType.IsSubclassOf(typeof(System.Enum))) {
								fieldInfo.SetValue(obj, Convert.ChangeType(Enum.Parse(fieldInfo.FieldType, node.Value), fieldInfo.FieldType));
							} else {
								PathAttribute pathAttribute = (PathAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(PathAttribute));
								if (pathAttribute != null) {
									fieldInfo.SetValue(obj, fileUtilityService.GetDirectoryNameWithSeparator(Path.GetDirectoryName(fileName)) + Convert.ChangeType(node.Value, fieldInfo.FieldType).ToString());
								} else {
									fieldInfo.SetValue(obj, Convert.ChangeType(node.Value, fieldInfo.FieldType));
								}
							}
						}
					}
					
					// process XmlMemberAttributeAttribute attributes
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
							fieldInfo.SetValue(obj, attrArray);
						}
					}
					
				}
				currentType = currentType.BaseType;
			}
		}
		
		ICondition GetCondition(XmlElement el)
		{//这个函数为递归函数
			ConditionCollection conditions = new ConditionCollection();
			
			foreach (XmlElement child in el.ChildNodes) {
				conditions.Add(GetCondition(child));
			}
			
			switch (el.Name) {
				case "Condition":
					if (conditions.Count > 0) {//说明<Condition>结点不可以有子结点
						throw new AddInTreeFormatException("Condition node childs found. (doesn't make sense)");
					}
					ICondition c = AddInTreeSingleton.AddInTree.ConditionFactory.CreateCondition(this, el);
					AutoInitializeAttributes(c, el);
					return c;
				case "And":
					if (conditions.Count <= 1) {
						throw new AddInTreeFormatException("And node with none or only one child found.");
					}
					return new AndCondition(conditions);
				case "Or":
					if (conditions.Count <= 1) {
						throw new AddInTreeFormatException("Or node with none or only one child found.");
					}
					return new OrCondition(conditions);
				case "Not":
					if (conditions.Count > 1) {
						throw new AddInTreeFormatException("Not node with more than one child found");
					}
					if (conditions.Count == 0) {
						throw new AddInTreeFormatException("Not node without child found.");
					}
					return new NegatedCondition(conditions);
			}
			
			throw new AddInTreeFormatException("node " + el.Name + " not valid in expression.");
		}
		
		ICondition BuildComplexCondition(XmlElement el)
		{
			if (el["Or"] != null) {
				return GetCondition(el["Or"]);
			}
			if (el["And"] != null) {
				return GetCondition(el["And"]);
			}
			if (el["Not"] != null) {
				return GetCondition(el["Not"]);
			}
			if (el["Condition"] != null) {
				return GetCondition(el["Condition"]);
			}
			return null;
		}
		
		void AddCodonsToExtension(Extension e, XmlElement el, ConditionCollection conditions)
		{
			foreach (object o in el.ChildNodes) {
				if (!(o is XmlElement)) {
					continue;
				}
				XmlElement curEl = (XmlElement)o;
				
				switch (curEl.Name) {
					case "And": // these nodes are silently ignored.
					case "Or":
					case "Not":
					case "Condition":
						break;
					case "Conditional":
						ICondition condition = null;
						
						// 若当前条件结点的属性个数为零或者(个数为1并且包含action属性),
						//则说明当前条件结点为一个复合逻辑条件(即该条件由一些and,or等条件组成)
						if (curEl.Attributes.Count == 0 || (curEl.Attributes.Count == 1 && curEl.Attributes["FailedAction"] != null)) 
						{
							condition = BuildComplexCondition(curEl);
							
							// set condition action manually
							if (curEl.Attributes["FailedAction"] != null) 
							{
								condition.FailedAction = (ConditionFailedAction)Enum.Parse(typeof(ConditionFailedAction), curEl.Attributes["FailedAction"].InnerText);
							}
							
							if (condition == null) 
							{
								throw new AddInTreeFormatException("empty conditional, but no condition definition found.");
							}
						} 
						else 
						{   //当前条件结点为一个简单条件结点,该条件结点不会包含<Or>,<And>等子结点
							condition = AddInTreeSingleton.AddInTree.ConditionFactory.CreateCondition(this, curEl);
							AutoInitializeAttributes(condition, curEl);
						}
						
						// put the condition at the end of the condition 'stack'
						conditions.Add(condition);
						
						// traverse the subtree
						AddCodonsToExtension(e, curEl, conditions);
						
						// now we are back to the old level, remove the condition
						// that was applied to the subtree.
						conditions.RemoveAt(conditions.Count - 1);
						break;
					default:
						ICodon codon = AddInTreeSingleton.AddInTree.CodonFactory.CreateCodon(this, curEl);
						
						AutoInitializeAttributes(codon, curEl);
						
						// Ensure that the codon is inserted after the codon which is defined
						// before in the add-in definition.
						// The codons get topologically sorted and if I don't set the InsertAfter they may
						// change it's sorting order.
						e.Conditions[codon.ID] = new ConditionCollection(conditions);
						if (codon.InsertAfter == null && codon.InsertBefore == null && e.CodonCollection.Count > 0) {
							codon.InsertAfter = new string[] { ((ICodon)e.CodonCollection[e.CodonCollection.Count - 1]).ID };
						}
						
						e.CodonCollection.Add(codon);
						if (curEl.ChildNodes.Count > 0) {
							Extension newExtension = new Extension(e.Path + '/' + codon.ID);
							AddCodonsToExtension(newExtension, curEl, conditions);
							extensions.Add(newExtension);
						}
						break;
				}
			}
		}
		
		
		#endregion

		/// <summary>
		/// Definies an extension point (path in the tree) with its codons.
		/// </summary>
		public class Extension
		{
			string    path;
			ArrayList codonCollection = new ArrayList();
			Hashtable conditions       = new Hashtable();
			
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
			/// returns a Hashtable with all conditions defined in this extension.
			/// where the key is the codon ID and the value is a <code>ConditionCollection</code>
			/// containing all conditions.
			/// </summary>
			public Hashtable Conditions {
				get {
					return conditions;
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
			/// Constructs a new extension instance.
			/// </summary>
			public Extension(string path)
			{
				this.path = path;
			}
			

		}
	}
}
