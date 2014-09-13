
using System;
using System.Collections;
using System.Xml.Schema;
using System.Text;
using System.IO;
using System.Xml;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Reflection;


namespace NetFocus.DataStructure.TextEditor.Document
{
	//一个管理高亮度显示策略的管理器,用到了singleton设计模式.
	public class HighlightingManager
	{
		ArrayList syntaxModeProviders = new ArrayList();
		static HighlightingManager highlightingManager;
		//定义一个哈希表,key是syntaxMode.Name,value是一个IHighlightingStrategy对象.
		Hashtable highlightingDefs = new Hashtable();
		//定义一个哈希表,key是文件的扩展名字符串,value是一个syntaxMode.Name的字符串.
		Hashtable extensionsToName = new Hashtable();
		
		public Hashtable HighlightingDefinitions {
			get {
				return highlightingDefs;
			}
		}
		
		public static HighlightingManager Manager {
			get {
				return highlightingManager;		
			}
		}
		
		static HighlightingManager()
		{
			highlightingManager = new HighlightingManager();
		}
		
		private HighlightingManager()
		{
			CreateDefaultHighlightingStrategy();
		}
		//创建一个默认的语法醒目显示对象.
		void CreateDefaultHighlightingStrategy()
		{
			
			DefaultHighlightingStrategy defaultHighlightingStrategy = new DefaultHighlightingStrategy();
			defaultHighlightingStrategy.Extensions = new string[] {};
			defaultHighlightingStrategy.RuleSets.Add(new HighlightRuleSet());
			highlightingDefs["Default"] = defaultHighlightingStrategy;
		}

		
		public void AddSyntaxModeProvider(SyntaxModeProvider syntaxModeProvider)
		{
			foreach (SyntaxMode syntaxMode in syntaxModeProvider.SyntaxModes) 
			{
				DefaultHighlightingStrategy highlightingStrategy = ParseSyntaxMode(syntaxMode, syntaxModeProvider.GetSyntaxModeXmlTextReader(syntaxMode));
				highlightingStrategy.ResolveReferences();

				highlightingDefs[syntaxMode.Name] = highlightingStrategy;
				
				foreach (string extension in syntaxMode.Extensions) 
				{
					extensionsToName[extension.ToUpper()] = syntaxMode.Name;//对某一类文件都使用这个语法模式.
				}
			}
			if (!syntaxModeProviders.Contains(syntaxModeProvider)) 
			{
				syntaxModeProviders.Add(syntaxModeProvider);//注意:这里添加对象.
			}
		}

		
		public IHighlightingStrategy FindHighlighterByName(string name)
		{
			object def = highlightingDefs[name];

			return (IHighlightingStrategy)(def == null ? highlightingDefs["Default"] : def);
		}
		
		
		public IHighlightingStrategy FindHighlighterForFile(string fileName)
		{
			string name = (string)extensionsToName[Path.GetExtension(fileName).ToUpper()];
			if (name != null) {
				return FindHighlighterByName(name);
			} else {
				return (IHighlightingStrategy)highlightingDefs["Default"];
			}
		}
		
		//用于触发事件.
		protected virtual void OnReloadSyntaxHighlighting(EventArgs e)
		{
			if (ReloadSyntaxHighlighting != null) {
				ReloadSyntaxHighlighting(this, e);
			}
		}
		//重新装在语法醒目显示策略.
		public void ReloadSyntaxModes()
		{
			highlightingDefs.Clear();
			extensionsToName.Clear();
			CreateDefaultHighlightingStrategy();
			foreach (SyntaxModeProvider provider in syntaxModeProviders) 
			{
				AddSyntaxModeProvider(provider);
			}
			OnReloadSyntaxHighlighting(EventArgs.Empty);//触发事件,使相应的委托实例能够被调用到.
		}
		

		public event EventHandler ReloadSyntaxHighlighting;

		static ArrayList errors = null;
		
		//此函数接收一个syntaxMode对象和一个XmlTextReader对象为参数,分析此xmlTextReader对象所对应的语法模式文件,
		//并返回一个高亮度显示策略对象(DefaultHighlightingStrategy).
		DefaultHighlightingStrategy ParseSyntaxMode(SyntaxMode syntaxMode, XmlTextReader xmlTextReader)
		{
			try 
			{
                //XmlValidatingReader validatingReader = new XmlValidatingReader(xmlTextReader);
                //Stream shemaStream = Assembly.GetCallingAssembly().GetManifestResourceStream("Mode.xsd");
                //validatingReader.Schemas.Add("", new XmlTextReader(shemaStream));
                //validatingReader.ValidationType = ValidationType.Schema;
                //validatingReader.ValidationEventHandler += new ValidationEventHandler (ValidationHandler);
				//以上定义了一个XmlValidatingReader对象,用于验证语法模式文件.(如CSharp-Mode.xshd)
				
				XmlDocument doc = new XmlDocument();
                doc.Load(xmlTextReader);
				
				DefaultHighlightingStrategy highlighter = new DefaultHighlightingStrategy(doc.DocumentElement.Attributes["name"].InnerText);
				
				if (doc.DocumentElement.Attributes["extensions"]!= null) 
				{
					highlighter.Extensions = doc.DocumentElement.Attributes["extensions"].InnerText.Split(new char[] { ';', '|' });
				}
				
				// parse environment color
				XmlElement environment = doc.DocumentElement["Environment"];
				if (environment != null) 
				{
					foreach (XmlNode node in environment.ChildNodes) 
					{
						if (node is XmlElement) 
						{
							XmlElement el = (XmlElement)node;
							highlighter.SetColorForEnvironment(el.Name, new HighlightColor(el));
						}
					}
				}
				
				// parse properties,not all xshd files have
				if (doc.DocumentElement["Properties"]!= null) 
				{
					foreach (XmlElement propertyElement in doc.DocumentElement["Properties"].ChildNodes) 
					{
						highlighter.Properties[propertyElement.Attributes["name"].InnerText] =  propertyElement.Attributes["value"].InnerText;
					}
				}
				
				// parse digits
				if (doc.DocumentElement["Digits"]!= null) 
				{
					highlighter.DigitColor = new HighlightColor(doc.DocumentElement["Digits"]);
				}
				
				XmlNodeList nodes = doc.DocumentElement.GetElementsByTagName("RuleSet");
				foreach (XmlElement element in nodes) 
				{
					highlighter.AddRuleSet(new HighlightRuleSet(element));
				}
				
				xmlTextReader.Close();
				
				if(errors!=null) 
				{
					ReportErrors(syntaxMode.FileName);
					errors = null;
					return null;
				} 
				else 
				{
					return highlighter;		
				}
			} 
			catch (Exception e) 
			{
				MessageBox.Show("不能加载语法模式文件.\n" + e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				return null;
			}
		}	
		
		
		void ValidationHandler(object sender, ValidationEventArgs args)
		{
			if (errors==null) 
			{
				errors=new ArrayList();
			}
			errors.Add(args);
		}

		
		void ReportErrors(string fileName)
		{
			StringBuilder msg = new StringBuilder();
			msg.Append("不能加载语法模式文件: " + fileName + " ,原因:\n\n");
			foreach(ValidationEventArgs args in errors) 
			{
				msg.Append(args.Message);
				msg.Append(Console.Out.NewLine);
			}
			MessageBox.Show(msg.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
		}

	}

	

}
