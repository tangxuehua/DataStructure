
using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace NetFocus.DataStructure.TextEditor.Document
{
	public class SyntaxModeProvider
	{
		string    directory;
		ArrayList syntaxModes = null;
		
		public ArrayList SyntaxModes {
			get {
				return syntaxModes;
			}
		}
		

		public SyntaxModeProvider(string directory)
		{
			this.directory = directory;
			syntaxModes = ScanDirectory(directory);
		}
		

		public XmlTextReader GetSyntaxModeXmlTextReader(SyntaxMode syntaxMode)
		{
			string syntaxModeFile = Path.Combine(directory, syntaxMode.FileName);
			if (!File.Exists(syntaxModeFile)) {
				MessageBox.Show("Can't load highlighting definition " + syntaxModeFile + " (file not found)!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
				return null;
			}
			return new XmlTextReader(File.OpenRead(syntaxModeFile));
		}
		
		
		ArrayList ScanDirectory(string directory)
		{
			string[] files = Directory.GetFiles(directory);
			ArrayList modes = new ArrayList();
			foreach (string file in files) {
				if (Path.GetExtension(file).ToUpper() == ".XSHD") {
					XmlTextReader reader = new XmlTextReader(file);
					while (reader.Read()) {
						if (reader.NodeType == XmlNodeType.Element) {
							switch (reader.Name) {
								case "SyntaxDefinition":
									string name       = reader.GetAttribute("name");
									string extensions = reader.GetAttribute("extensions");
									modes.Add(new SyntaxMode(Path.GetFileName(file),
									                               name,
									                               extensions));
									goto bailout;
								default:
									MessageBox.Show("Unknown root node in syntax highlighting file :" + reader.Name, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
									goto bailout;
							}
						}
					}
					bailout:
					reader.Close();
			
				}
			}
			return modes;
		}
	}
}
