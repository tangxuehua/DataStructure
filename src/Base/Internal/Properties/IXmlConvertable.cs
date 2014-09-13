

using System.Xml;

namespace NetFocus.DataStructure.Properties
{
	/// <summary>
	/// 如果你想定义复杂的属性,你可以实现该接口,以便将属性的信息保存为XML的形式.
	/// </summary>
	public interface IXmlConvertable
	{
		/// <summary>
		/// 将一个XML Element 转换成一个对象(注意:此对象也是可以被保存为XML Element的)
		/// </returns>
		object FromXmlElement(XmlElement element);
		
		/// <summary>
		/// 将上面的一个对象保存为一个XML Element元素.
		/// </returns>
		XmlElement ToXmlElement(XmlDocument doc);
	}
}
