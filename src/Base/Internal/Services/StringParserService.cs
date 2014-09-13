
using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Specialized;

namespace NetFocus.DataStructure.Services
{
	/// <summary>
	/// 这个类用于分析一个字符串,并得到正确的字符串,主要用于读取资源文件中的信息.
	/// </summary>
	public class StringParserService : AbstractService
	{
		PropertyDictionary properties = new PropertyDictionary();
		
		public PropertyDictionary Properties {
			get {
				return properties;
			}
		}
		
		
		public StringParserService()
		{
            IDictionary variables = Environment.GetEnvironmentVariables();
            foreach (string name in variables.Keys) {
                properties.Add("env:" + name, (string)variables[name]);
            }
		}
		

		public void Parse(ref string[] inputs)
		{
			for (int i = inputs.GetLowerBound(0); i <= inputs.GetUpperBound(0); ++i) 
			{
				inputs[i] = Parse(inputs[i], null);
			}
		}
		

		public string Parse(string input)
		{
			return Parse(input, null);
		}
			
		
		public string Parse(string input, string[,] customTags)
		{
			string output = input;
			if (input != null) {
				const string pattern = @"\$\{([^\}]*)\}";
				foreach (Match m in Regex.Matches(input, pattern)) {
					if (m.Length > 0) {
						string token         = m.ToString();
						string propertyName  = m.Groups[1].Captures[0].Value;
						string propertyValue = null;
						switch (propertyName.ToUpper()) {
							case "DATE": // current date
								propertyValue = DateTime.Today.ToShortDateString();
								break;
							case "TIME": // current time
								propertyValue = DateTime.Now.ToShortTimeString();
								break;
							default:
								propertyValue = null;
								if (customTags != null) {
									for (int j = 0; j < customTags.GetLength(0); ++j) {
										if (propertyName.ToUpper() == customTags[j, 0].ToUpper()) {
											propertyValue = customTags[j, 1];
											break;
										}
									}
								}
								if (propertyValue == null) {
									propertyValue = properties[propertyName.ToUpper()];
								}
								if (propertyValue == null) {
									int k = propertyName.IndexOf(':');
									if (k > 0) {
										switch (propertyName.Substring(0, k).ToUpper()) {
											case "RES":
												ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
												propertyValue = Parse(resourceService.GetString(propertyName.Substring(k + 1)), customTags);
												break;
											case "PROPERTY":
												PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
												propertyValue = propertyService.GetProperty(propertyName.Substring(k + 1)).ToString();
												break;
										}
									}
								}
								break;
						}
						if (propertyValue != null) {
							output = output.Replace(token, propertyValue);
						}
					}
				}
			}
			return output;
		}
	}
	

	/// <summary>
	/// 这里定义了一个新的继承自DictionaryBase的类的原因是因为还要设置一些只读的字符串属性,把这些属性放在一个单独的StringCollection中
	/// </summary>
	public class PropertyDictionary : DictionaryBase
	{
		/// <summary>
		/// 用于保存一些只读的属性的名称的一个字符串集合
		/// </summary>
		StringCollection readOnlyProperties = new StringCollection();
		
		/// <summary>
		/// 添加一个只读的属性的键值对信息到一个字符串集合中
		/// </summary>
		public void AddReadOnly(string name, string value) 
		{
			if (!readOnlyProperties.Contains(name)) {
				readOnlyProperties.Add(name);
				Dictionary.Add(name, value);
			}
		}
		
		
		/// <summary>
		/// 添加一个可读写的属性的键值对信息到一个字符串集合中
		/// </summary>
		public void Add(string name, string value) 
		{
			if (!readOnlyProperties.Contains(name)) {
				Dictionary.Add(name, value);
			}
		}
		
		
		public string this[string name] {  //一个默认的带参数的属性
			get { 
				return (string)Dictionary[(object)name.ToUpper()];
			}
			set {
				Dictionary[name.ToUpper()] = value;
			}
		}
		
		
		protected override void OnClear() 
		{
			readOnlyProperties.Clear();
		}
	}	
	

}
