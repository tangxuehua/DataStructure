
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace NetFocus.DataStructure.Gui.Components
{
	public class AxStatusBar : System.Windows.Forms.StatusBar
	{
		public AxStatusBar()
		{
		}
		protected override void OnDrawItem(StatusBarDrawItemEventArgs sbdievent)
		{
			if (sbdievent.Panel is AxStatusBarPanel) {
				((AxStatusBarPanel)sbdievent.Panel).DrawPanel(sbdievent);
			} else {
				base.OnDrawItem(sbdievent);
			}
		}
	}
	
}
