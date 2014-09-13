using System;
using System.Collections;
using System.Xml;
using System.Configuration;

namespace NetFocus.DataStructure
{
	public class AddInSettingsHandler : IConfigurationSectionHandler
	{
		public AddInSettingsHandler()
		{
		}

        #region IConfigurationSectionHandler ≥…‘±

        public object Create(object parent, object configContext, XmlNode section)
        {
            ArrayList addInDirectories = new ArrayList();
            XmlNodeList addInDirList = section.SelectNodes("AddInDirectory");

            foreach (XmlNode addInDir in addInDirList)
            {
                XmlNode path = addInDir.Attributes.GetNamedItem("path");
                if (path != null)
                {
                    addInDirectories.Add(path.Value);
                }
            }
            return addInDirectories;
        }

        #endregion

        public static string[] GetAddInDirectories()
        {
            ArrayList addInDirs = ConfigurationManager.GetSection("AddInDirectories") as ArrayList;
            if (addInDirs != null)
            {
                string[] directories = new string[addInDirs.Count];
                for (int i = 0; i < addInDirs.Count; i++)
                {
                    directories[i] = addInDirs[i] as string;
                }
                return directories;
            }
            return null;
        }
	}
}
