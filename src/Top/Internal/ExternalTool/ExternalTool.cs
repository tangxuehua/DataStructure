

using System;
using System.Reflection;
using System.Collections;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;

namespace NetFocus.DataStructure.Internal.ExternalTool
{
	/// <summary>
	/// This class describes an external tool, which is a external program
	/// that can be launched from the toolmenu inside Sharp Develop.
	/// </summary>
	public class ExternalTool
	{
		string menuCommand       = "ÐÂ¹¤¾ß";
		string command           = "";
		
		public string MenuCommand {
			get {
				return menuCommand;
			}
			set {
				menuCommand = value;
				Debug.Assert(menuCommand != null, "NetFocus.DataStructure.Internal.ExternalTool.ExternalTool : string MenuCommand == null");
			}
		}
		
		public string Command {
			get {
				return command;
			}
			set {
				command = value;
				Debug.Assert(command != null, "NetFocus.DataStructure.Internal.ExternalTool.ExternalTool : string Command == null");
			}
		}
		
		
		public ExternalTool() 
		{
		}
		
		public ExternalTool(XmlElement el)
		{
			if (el == null) {
				throw new ArgumentNullException("ExternalTool(XmlElement el) : el can't be null");
			}
			
			if (el["COMMAND"] == null ||
				el["MENUCOMMAND"] == null) {
				throw new Exception("ExternalTool(XmlElement el) : COMMAND and MENUCOMMAND attributes must exist.(check the ExternalTool XML)");
			}
			
			Command           = el["COMMAND"].InnerText;
			MenuCommand       = el["MENUCOMMAND"].InnerText;
			
		}
		
		public override string ToString()
		{
			return menuCommand;
		}
		
		public XmlElement ToXmlElement(XmlDocument doc)
		{
			if (doc == null) {
				throw new ArgumentNullException("ExternalTool.ToXmlElement(XmlDocument doc) : doc can't be null");
			}
			
			XmlElement el = doc.CreateElement("TOOL");
			XmlElement x;
			x = doc.CreateElement("COMMAND");
			x.InnerText = command;
			el.AppendChild(x);
			
			x = doc.CreateElement("MENUCOMMAND");
			x.InnerText = MenuCommand;
			el.AppendChild(x);
			
			el.AppendChild(x);
			
			return el;
		}
	}
}
