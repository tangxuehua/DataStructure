
using System;
using System.Xml;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace NetFocus.DataStructure.Gui.XmlForms {
	
	/// <summary>
	/// This interface is used to create the objects which are given by name in 
	/// the xml definition.
	/// </summary>
	public interface IObjectCreator
	{
		/// <summary>
		/// Creates a new instance of object name. 
		/// </summary>
		object CreateObject(string name);
	}
}
