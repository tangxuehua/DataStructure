
using System;
using System.Xml;
using System.Collections;
using System.Drawing;
using System.Drawing.Text;
using System.Reflection;
using System.Windows.Forms;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Gui.XmlForms;

namespace NetFocus.DataStructure.Gui.XmlForms
{
	public class DataStructureObjectCreator : DefaultObjectCreator
	{
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		public override object CreateObject(string name)
		{
			object o = base.CreateObject(name);
			if (o != null) {
				try {
					PropertyInfo propertyInfo = o.GetType().GetProperty("FlatStyle");
					if (propertyInfo != null) {
						if (o is Label) {
							propertyInfo.SetValue(o, FlatStyle.Standard, null);
						} else {
							propertyInfo.SetValue(o, FlatStyle.System, null);
						}
					}
				} catch (Exception) {}
			}
			return o;
		}
	}
}
