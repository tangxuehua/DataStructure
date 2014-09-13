using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Resources;


namespace NetFocus.DataStructure.Gui.Dialogs {
	
	public class SplashScreenForm : Form
	{
		static SplashScreenForm splashScreen = new SplashScreenForm();
		static ArrayList requestedFileList = new ArrayList();
		
		public static SplashScreenForm SplashScreen {
			get {
				return splashScreen;
			}
		}		
		
		private SplashScreenForm()
		{
#if !DEBUG
			TopMost         = true;
#endif
			FormBorderStyle = FormBorderStyle.None;
			StartPosition   = FormStartPosition.CenterScreen;
			ShowInTaskbar   = false;
			Bitmap bitmap = new Bitmap(Assembly.GetEntryAssembly().GetManifestResourceStream("SplashScreen.png"));
			Size = bitmap.Size;
			BackgroundImage = bitmap;
		}
		

		public static string[] GetRequestedFileList()
		{
			return GetStringArray(requestedFileList);
		}
		
		static string[] GetStringArray(ArrayList list)
		{
			return (string[])list.ToArray(typeof(string));
		}

		public static void SetCommandLineArgs(string[] args)
		{
			requestedFileList.Clear();
			
			foreach (string arg in args)
			{
				requestedFileList.Add(arg);
			}
		}
	}
}
