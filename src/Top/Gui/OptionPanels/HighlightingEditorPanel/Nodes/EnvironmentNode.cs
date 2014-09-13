using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;

using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Gui.Dialogs;
using NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Panels;

namespace NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Nodes
{

	class EnvironmentNode : AbstractNode
	{
		public static string[]        ColorNames;
		public string[]               ColorDescs;
		public EditorHighlightColor[] Colors;
		
		public EnvironmentNode(XmlElement el)
		{
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			ArrayList envColors            = new ArrayList();
			ArrayList envColorNames        = new ArrayList();
			ArrayList envColorDescriptions = new ArrayList();
			if (el != null) 
			{
				foreach (XmlNode node in el.ChildNodes) 
				{
					if (node is XmlElement) 
					{
						envColorNames.Add(node.Name);
						envColorDescriptions.Add("${res:Dialog.HighlightingEditor.EnvColors." + node.Name + "}");
						envColors.Add(new EditorHighlightColor((XmlElement)node));
					}
				}
			}
			EnvironmentNode.ColorNames = (string[])envColorNames.ToArray(typeof(string));
			this.ColorDescs = (string[])envColorDescriptions.ToArray(typeof(string));
			this.Colors     = (EditorHighlightColor[])envColors.ToArray(typeof(EditorHighlightColor));
			stringParserService.Parse(ref ColorDescs);
			
			Text = ResNodeName("EnvironmentColors");
		
			OptionPanel = new EnvironmentOptionPanel(this);
		}

		public override void UpdateNodeText()
		{
		}
		
		public override string ToXml()
		{
			string str = "\t<Environment>\n";
			for (int i = 0; i <= ColorNames.GetUpperBound(0); ++i) 
			{
				str += "\t\t<" + ColorNames[i] + " " + Colors[i].ToXml() + "/>\n";
			}
			str += "\t</Environment>\n\n";
			return str;
		}
	}
}
