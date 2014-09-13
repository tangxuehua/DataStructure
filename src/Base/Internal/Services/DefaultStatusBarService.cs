
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using NetFocus.DataStructure.Gui.Components;


namespace NetFocus.DataStructure.Services
{
	public class DefaultStatusBarService : AbstractService, IStatusBarService
	{
		SdStatusBar statusBar = null;
		StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
		string lastMessage = "";

		public DefaultStatusBarService()
		{
			statusBar = new SdStatusBar(this);
		}
		
		
		public void Dispose()
		{
			if (statusBar != null) 
			{
				statusBar.Dispose();
				statusBar = null;
			}
		}
		
		
		public Control Control 
		{
			get 
			{
				Debug.Assert(statusBar != null);
				return statusBar;
			}
		}
		

		public void SetCaretPosition(int x, int y, int charOffset)
		{
			if(x !=0 && y != 0)
			{
				statusBar.CursorStatusBarPanel.Text = String.Format("行 {0,-5} 列 {1,-5}", y, x);
			}
			else
			{
				statusBar.CursorStatusBarPanel.Text = "";
			}
		}
		
		
		public void SetInsertMode(bool insertMode)
		{
			statusBar.ModeStatusBarPanel.Text = insertMode ? "INS" : "OVR";
		}
		
		
		public void ShowErrorMessage(string message)
		{
			Debug.Assert(statusBar != null);
			statusBar.ShowErrorMessage(stringParserService.Parse(message));
		}
		
		
		public void SetMessage(string message)
		{
			Debug.Assert(statusBar != null);
			lastMessage = message;
			statusBar.SetMessage(stringParserService.Parse(message));
		}
		
		
		public void SetMessage(Image image, string message)
		{
			Debug.Assert(statusBar != null);
			statusBar.SetMessage(image, stringParserService.Parse(message));
		}
		
		
	}
}
