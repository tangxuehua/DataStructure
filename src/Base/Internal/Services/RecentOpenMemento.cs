
using System;
using System.Xml;
using System.Diagnostics;
using System.Collections;
using System.IO;

using NetFocus.DataStructure.Properties;

namespace NetFocus.DataStructure.Services
{
	/// <summary>
	/// This class handles the recent open files and the recent open project files of DataStructure
	/// it checks, if the files exists at every creation, and if not it doesn't list them in the 
	/// recent files, and they'll not be saved during the next option save.
	/// </summary>
	public class RecentOpenMemeto : IXmlConvertable
	{
		/// <summary>
		/// This variable is the maximal length of lastfile/lastopen entries
		/// must be > 0
		/// </summary>
		int MAX_LENGTH = 10;
		
		ArrayList lastfile    = new ArrayList();
		public event EventHandler RecentFileChanged;

		
		public ArrayList RecentFile {
			get {
				Debug.Assert(lastfile != null, "RecentOpenMemeto : set string[] LastFile (value == null)");
				return lastfile;
			}
		}

		void OnRecentFileChange()
		{
			if (RecentFileChanged != null) {
				RecentFileChanged(this, null);
			}
		}
		
		
		public RecentOpenMemeto()
		{
		}
		
		
		public RecentOpenMemeto(XmlElement element)
		{
			XmlNodeList nodes = element["FILES"].ChildNodes;
			
			for (int i = 0; i < nodes.Count; ++i) {
				if (File.Exists(nodes[i].InnerText)) {
					lastfile.Add(nodes[i].InnerText);
				}
			}
		}
		
		
		public void AddLastFile(string name) // TODO : improve 
		{
			for (int i = 0; i < lastfile.Count; ++i) {
				if (lastfile[i].ToString() == name) {
					lastfile.RemoveAt(i);
				}
			}
			
			while (lastfile.Count >= MAX_LENGTH) {
				lastfile.RemoveAt(lastfile.Count - 1);
			}
			
			if (lastfile.Count > 0) {
				lastfile.Insert(0, name);
			} else {
				lastfile.Add(name);
			}
			
			OnRecentFileChange();
		}

		public void RemoveLastFile() 
		{
			if (lastfile.Count > 0) {
				lastfile.RemoveAt(0);
			}
			OnRecentFileChange();
		}
		
		
		public void ClearRecentFiles()
		{
			lastfile.Clear();
			
			OnRecentFileChange();
		}
		
		
		public object FromXmlElement(XmlElement element)
		{
			return new RecentOpenMemeto(element);
		}
		
		
		public XmlElement ToXmlElement(XmlDocument doc)
		{
			XmlElement recent = doc.CreateElement("RECENT");
			
			XmlElement lastfiles = doc.CreateElement("FILES");
			foreach (string file in lastfile) {
				XmlElement element = doc.CreateElement("FILE");
				element.InnerText  = file;
				lastfiles.AppendChild(element);
			}
			
			recent.AppendChild(lastfiles);
			
			return recent;
		}
		
		
		public void FileRemoved(object sender, FileEventArgs e)
		{
			for (int i = 0; i < lastfile.Count; ++i) {
				string file = lastfile[i].ToString();
				if (e.FileName == file) {
					lastfile.RemoveAt(i);
					OnRecentFileChange();
					break;
				}
			}
		}
		
		
		public void FileRenamed(object sender, FileEventArgs e)
		{
			for (int i = 0; i < lastfile.Count; ++i) {
				string file = lastfile[i].ToString();
				if (e.SourceFile == file) {
					lastfile.RemoveAt(i);
					lastfile.Insert(i, e.TargetFile);
					OnRecentFileChange();
					break;
				}
			}
		}
	}
}
