
using System;
using System.Drawing;
using System.Windows.Forms;
using NetFocus.DataStructure.Services;

namespace NetFocus.DataStructure.Gui.Components
{
	public class SdStatusBar : AxStatusBar
	{
		ProgressBar statusProgressBar      = new ProgressBar();
		
		AxStatusBarPanel txtStatusBarPanel    = new AxStatusBarPanel();
		AxStatusBarPanel cursorStatusBarPanel = new AxStatusBarPanel();
		AxStatusBarPanel modeStatusBarPanel   = new AxStatusBarPanel();

		public AxStatusBarPanel CursorStatusBarPanel {
			get {
				return cursorStatusBarPanel;
			}
		}
		
		public AxStatusBarPanel ModeStatusBarPanel {
			get {
				return modeStatusBarPanel;
			}
		}
		
		
		public SdStatusBar(IStatusBarService manager)
		{
			txtStatusBarPanel.Width = 500;
			txtStatusBarPanel.AutoSize = StatusBarPanelAutoSize.Spring;
			this.Panels.Add(txtStatusBarPanel);//在状态栏中添加显示内容的一个面板.
			
			statusProgressBar.Width  = 200;
			statusProgressBar.Height = 14;
			statusProgressBar.Location = new Point(160, 4);
			statusProgressBar.Minimum = 0;
			statusProgressBar.Visible = false;
			Controls.Add(statusProgressBar);//在状态栏中添加显示进度的一个面板.
			
			cursorStatusBarPanel.Width = 150;
			cursorStatusBarPanel.AutoSize = StatusBarPanelAutoSize.None;
			cursorStatusBarPanel.Alignment = HorizontalAlignment.Left;
			Panels.Add(cursorStatusBarPanel);//在状态栏中添加显示光标位置的一个面板.
				
			modeStatusBarPanel.Width = 35;
			modeStatusBarPanel.AutoSize = StatusBarPanelAutoSize.None;
			modeStatusBarPanel.Alignment = HorizontalAlignment.Right;
			Panels.Add(modeStatusBarPanel);//在状态栏中添加显示模式的一个面板.
			
			this.ShowPanels = true;
		}
		
		
		public void ShowErrorMessage(string message)
		{
			txtStatusBarPanel.Text = "错误 : " + message;
		}
		
		
		public void ShowErrorMessage(Image image, string message)
		{
			txtStatusBarPanel.Text = "错误 : " + message;
		}
		
		
		public void SetMessage(string message)
		{
			txtStatusBarPanel.Text = message;
		}
		
		
		public void SetMessage(Image image, string message)
		{
			txtStatusBarPanel.Text = message;
		}
		
		
	}
}
