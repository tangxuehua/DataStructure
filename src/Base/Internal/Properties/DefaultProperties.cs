
using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Windows.Forms;
using System.Reflection;


namespace NetFocus.DataStructure.Properties
{
	/// <summary>
	/// 对属性接口的一个默认实现.
	/// </summary>
	public class DefaultProperties : IProperties
	{
		#region 实现IProperties接口的代码
		
		Hashtable properties = new Hashtable();//存放属性的一个哈希表.
		
		public object GetProperty(string key, object defaultvalue)
		{
			if (!properties.ContainsKey(key)) {//如果是第一次设置属性,则把它加入hash table中.
				if (defaultvalue != null) {
					properties[key] = defaultvalue;
				}
				return defaultvalue;
			}
			
			object obj = properties[key];

			if (defaultvalue is IXmlConvertable && obj is XmlElement) {
				obj = properties[key] = ((IXmlConvertable)defaultvalue).FromXmlElement((XmlElement)((XmlElement)obj).FirstChild);
			}
			return obj;
		}
		

		public object GetProperty(string key)
		{
			return GetProperty(key, (object)null);
		}
		
		
		public int GetProperty(string key, int defaultvalue)
		{
			return int.Parse(GetProperty(key, (object)defaultvalue).ToString());
		}

		
		public bool GetProperty(string key, bool defaultvalue)
		{
			return bool.Parse(GetProperty(key, (object)defaultvalue).ToString());
		}

		
		public short GetProperty(string key, short defaultvalue)
		{
			return short.Parse(GetProperty(key, (object)defaultvalue).ToString());
		}

		
		public byte GetProperty(string key, byte defaultvalue)
		{
			return byte.Parse(GetProperty(key, (object)defaultvalue).ToString());
		}
		
		
		public string GetProperty(string key, string defaultvalue)
		{
			return GetProperty(key, (object)defaultvalue).ToString();
		}

		
		public System.Enum GetProperty(string key, System.Enum defaultvalue)
		{
			try {
				return (System.Enum)Enum.Parse(defaultvalue.GetType(), GetProperty(key, (object)defaultvalue).ToString());
			} catch (Exception) {
				return defaultvalue;
			}
		}

		
		public void SetProperty(string key, object val)
		{
			object oldValue = properties[key];
			properties[key] = val;
			OnPropertyChanged(new PropertyEventArgs(this, key, oldValue, val));
		}

		
		protected virtual void OnPropertyChanged(PropertyEventArgs e)
		{
			if (PropertyChanged != null) 
			{
				PropertyChanged(this, e);
			}
		}
		
		
		public event PropertyEventHandler PropertyChanged;

		public IProperties Clone()  //浅拷贝
		{
			DefaultProperties df = new DefaultProperties();
			df.properties = (Hashtable)properties.Clone();//注意:这里是浅拷贝.
			return df;
		}
		
		
		#endregion
		
		#region 这两个方法实现IXmlConvertable接口

		/// <summary>
		/// 从一个XmlElement元素节点创建一个DefaultProperties对象
		/// </summary>
		public virtual object FromXmlElement(XmlElement element)
		{
			DefaultProperties defaultProperties = new DefaultProperties();
			defaultProperties.SetValueFromXmlElement(element);
			return defaultProperties;
		}
		
		/// <summary>
		///创建一个Properties节点,把当前内存中的所有属性值添加到Properties节点中,最后把Properties节点返回
		/// </summary>
		public virtual XmlElement ToXmlElement(XmlDocument doc)
		{
			XmlElement propertiesnode  = doc.CreateElement("Properties");
			
			foreach (DictionaryEntry entry in properties) {//注意Hashtable中键值对的表示方法,用DictionaryEntry
				if (entry.Value != null) {
					if (entry.Value is XmlElement) { // write unchanged XmlElement back
						//这里doc.ImportNode函数的作用是将一个XmlElement导入到一个Xml文档中,并把该XmlElement元素返回
						//注意:这里因为XmlElement是继承自XmlNode类的,所以不会有问题,
						//其实ImportNode方法要求的参数类型为XmlNode类型,这个要注意
						XmlNode unChangedNode = doc.ImportNode((XmlElement)entry.Value, true);
						propertiesnode.AppendChild(unChangedNode);
					} else if (entry.Value is IXmlConvertable) { // An Xml convertable object
						XmlElement convertableNode = doc.CreateElement("XmlConvertableProperty");//创建一个元素
						
						XmlAttribute key = doc.CreateAttribute("key");//创建一个属性
						key.InnerText = entry.Key.ToString();
						convertableNode.Attributes.Append(key);//将创建的属性添加到该元素中
						//递归操作entry.Value元素,就是因为这里,所以该函数才需要一个XmlDocument参数,为了确保XmlDocument都是同一个
						convertableNode.AppendChild(((IXmlConvertable)entry.Value).ToXmlElement(doc));
						//最后添加该convertableNode元素
						propertiesnode.AppendChild(convertableNode);
					} else {//对于一个一般的<Property>元素,只要为其创建两个属性即可
						XmlElement el = doc.CreateElement("Property");
						
						XmlAttribute key   = doc.CreateAttribute("key");
						key.InnerText      = entry.Key.ToString();
						el.Attributes.Append(key);
	
						XmlAttribute val   = doc.CreateAttribute("value");
						val.InnerText      = entry.Value.ToString();
						el.Attributes.Append(val);
						
						propertiesnode.AppendChild(el);
					}
				}
			}
			return propertiesnode;  //最后返回一个总的Xml节点
		}
		

		#endregion
		

		public DefaultProperties()
		{
		}
		
		
		/// <summary>
		/// 将全局属性文件中所有的属性元素的值装载到内存中的一个Hashtable中,用于初始化应用程序的所有全局属性
		/// </summary>
		protected void SetValueFromXmlElement(XmlElement element)
		{
			XmlNodeList nodes = element.ChildNodes;
			foreach (XmlElement el in nodes) 
			{
				if (el.Name == "Property") 
				{  //如果是一个普通的属性节点
					properties[el.Attributes["key"].InnerText] = el.Attributes["value"].InnerText;//将这个节点的InnerText值作为Hashtable中当前项的值
				} 
				else if (el.Name == "XmlConvertableProperty") 
				{  //如果是一个含有子节点的属性节点
					properties[el.Attributes["key"].InnerText] = el;//将这个属性节点作为值.
				} 
				else 
				{
					throw new Exception("不可识别的Xml节点 : " + el.Name);//如果属性不满足上面两种形式,则抛出异常.
				}
			}
		}
		

	}
}
