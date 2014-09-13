using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using NetFocus.Components.AddIns;

using NetFocus.DataStructure.Properties;
using NetFocus.Components.AddIns.Codons;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.Gui.Dialogs;
using NetFocus.DataStructure.Gui.Components;


namespace NetFocus.DataStructure.Commands.TabStrip
{

	public class CopyPathName : AbstractMenuCommand
	{
		public override void Run()
		{
			OpenFileTab tab = (OpenFileTab)Owner;
			
			IViewContent content = tab.ClickedView;
			
			if (content != null && content.ContentName != null) 
			{
				Clipboard.SetDataObject(new DataObject(DataFormats.Text, content.ContentName));
			}
		}
	}

}
