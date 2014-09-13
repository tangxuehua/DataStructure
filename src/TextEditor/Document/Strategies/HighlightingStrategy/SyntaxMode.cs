
using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Xml;

namespace NetFocus.DataStructure.TextEditor.Document
{
	public class SyntaxMode
	{
		string   fileName;
		string   name;
		string[] extensions;
		
		public string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
			}
		}
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public string[] Extensions {
			get {
				return extensions;
			}
			set {
				extensions = value;
			}
		}
		
		
		public SyntaxMode(string fileName, string name, string extensions)
		{
			this.fileName   = fileName;
			this.name       = name;
			this.extensions = extensions.Split(';', '|', ',');
		}
		
		public SyntaxMode(string fileName, string name, string[] extensions)
		{
			this.fileName = fileName;
			this.name = name;
			this.extensions = extensions;
		}
		

	}
}
