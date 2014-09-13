using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using System.Text;

namespace NetFocus.Components.AddIns
{
	/// <summary>
	/// this class is used to provide some simple file operation services,like save or searchDirections
	/// </summary>
	public class InternalFileService
	{
		public string GetDirectoryNameWithSeparator(string directoryName)
		{
			if (directoryName.EndsWith(Path.DirectorySeparatorChar.ToString())) 
			{
				return directoryName;
			}
			return directoryName + Path.DirectorySeparatorChar;
		}
		
		
		public StringCollection SearchDirectory(string directory, string filemask)
		{
			return SearchDirectory(directory, filemask, true);
		}
		

		public StringCollection SearchDirectory(string directory, string filemask, bool searchSubdirectories)
		{
			StringCollection collection = new StringCollection();
			SearchDirectory(directory, filemask, collection, searchSubdirectories);
			return collection;
		}
		
		
		void SearchDirectory(string directory, string filemask, StringCollection collection, bool searchSubdirectories)
		{
			try 
			{
				string[] file = Directory.GetFiles(directory, filemask);
				foreach (string f in file) 
				{
					collection.Add(f);
				}
				
				if (searchSubdirectories) 
				{
					string[] dir = Directory.GetDirectories(directory);
					foreach (string d in dir) 
					{
						SearchDirectory(d, filemask, collection, searchSubdirectories);
					}
				}
			} 
			catch (Exception e) 
			{
				MessageBox.Show("Can't access directory " + directory + " reason:\n" + e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		

	}
}
