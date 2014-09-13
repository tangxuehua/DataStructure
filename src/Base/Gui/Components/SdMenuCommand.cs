
using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Windows.Forms;

using DevComponents.DotNetBar;

namespace NetFocus.DataStructure.Gui.Components
{
	public class SdMenuCommand : ButtonItem
	{
		public SdMenuCommand(string label)
		{
			this.Text = label;
		}
		public SdMenuCommand(string label, EventHandler handler)
		{
			this.Click += handler;
			this.Text = label;
		}
	}
}
