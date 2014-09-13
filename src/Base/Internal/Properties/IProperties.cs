
using System;

namespace NetFocus.DataStructure.Properties
{
	/// <summary>
	/// 定义一个所有属性必须实现的接口.
	/// </summary>
	public interface IProperties : IXmlConvertable
	{

		//定义八个重载的获取属性值的方法(根据默认值的类型的不同,返回的值的类型也不同).
		object GetProperty(string key, object defaultvalue);

		object GetProperty(string key);

		int GetProperty(string key, int defaultvalue);

		bool GetProperty(string key, bool defaultvalue);

		short GetProperty(string key, short defaultvalue);

		byte GetProperty(string key, byte defaultvalue);

		string GetProperty(string key, string defaultvalue);

		System.Enum GetProperty(string key, System.Enum defaultvalue);

		//设置属性的值
		void SetProperty(string key, object val);

		IProperties Clone();//复制当前的属性对象.
		
		event PropertyEventHandler PropertyChanged;//定义一个事件,表示当属性值改变时触发该事件.
	}
}
