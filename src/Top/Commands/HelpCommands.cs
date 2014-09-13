
using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Windows.Forms;

using NetFocus.Components.AddIns.Codons;

using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.Gui.Dialogs;


namespace NetFocus.DataStructure.Commands
{
	public class ShowHelp : AbstractMenuCommand
	{
		public override void Run()
		{
			string fileName = Application.StartupPath + Path.DirectorySeparatorChar + System.Configuration.ConfigurationSettings.AppSettings["HelpFileDirectory"].ToString();
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			if (fileUtilityService.TestFileExists(fileName)) 
			{
				Help.ShowHelp((Form)WorkbenchSingleton.Workbench, fileName);
			}
		}
	}
	
	
	public class GotoWebSite : AbstractMenuCommand
	{
		string site;
		
		public GotoWebSite(string site)
		{
			this.site = site;
		}
		
		public override void Run()
		{
			IFileService fileService = (IFileService)NetFocus.DataStructure.Services.ServiceManager.Services.GetService(typeof(IFileService));
			fileService.OpenFile(site);
		}
	}
	
	
	public class GotoLink : AbstractMenuCommand
	{
		string site;
		
		public GotoLink(string site)
		{
			this.site = site;
		}
		
		public override void Run()
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			string file = site.StartsWith("home://") ? fileUtilityService.GetDirectoryNameWithSeparator(Application.StartupPath) + site.Substring(7).Replace('/', Path.DirectorySeparatorChar) : site;
			try 
			{
				Process.Start(file);
			} 
			catch (Exception) 
			{
				MessageBox.Show("Can't execute/view " + file + "\n Please check that the file exists and that you can open this file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
	

	public class AboutDataStructure : AbstractMenuCommand
	{
		public override void Run()
		{
			using (CommonAboutDialog ad = new CommonAboutDialog()) 
			{
				ad.Owner = (Form)WorkbenchSingleton.Workbench;
				ad.StartPosition=FormStartPosition.CenterParent;
				ad.ShowDialog();
			}
		}
	}


}