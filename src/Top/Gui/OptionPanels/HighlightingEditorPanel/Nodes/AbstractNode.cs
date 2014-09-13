
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Panels;

namespace NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Nodes
{
	public abstract class AbstractNode : TreeNode
	{
		NodeOptionPanel optionPanel;
		
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		
		public string ResNodeName(string resName)
		{
			try {
				return resourceService.GetString("Dialog.HighlightingEditor.TreeView." + resName);
			} catch {
				return resName;
			}
		}
		
		public NodeOptionPanel OptionPanel {
			get {
				return optionPanel;
			}
			set{
				optionPanel = value;
			}
		}
		
		public abstract void UpdateNodeText();

		public virtual string ToXml() 
		{ 
			return ""; 
		}
		
		
		public static string ReplaceXmlChars(string str)
		{
			return str.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
		}
	}
}
