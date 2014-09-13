
using System;
using System.Collections;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using NetFocus.DataStructure.Properties;
using NetFocus.Components.AddIns.Codons;
using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.Gui.Dialogs;


namespace NetFocus.DataStructure.Commands
{
	public class WindowCascade : AbstractMenuCommand
	{
		public override void Run()
		{
			((Form)WorkbenchSingleton.Workbench).LayoutMdi(MdiLayout.Cascade);
		}
	}
	
	public class WindowTileHorizontal : AbstractMenuCommand
	{
		public override void Run()
		{
			((Form)WorkbenchSingleton.Workbench).LayoutMdi(MdiLayout.TileHorizontal);
		}
	}
	
	public class WindowTileVertical : AbstractMenuCommand
	{
		public override void Run()
		{
			((Form)WorkbenchSingleton.Workbench).LayoutMdi(MdiLayout.TileVertical);
		}
	}
	
	public class WindowArrangeIcons : AbstractMenuCommand
	{
		public override void Run()
		{
			((Form)WorkbenchSingleton.Workbench).LayoutMdi(MdiLayout.ArrangeIcons);
		}
	}
	
	public class CloseAllWindows : AbstractMenuCommand
	{
		public override void Run()
		{
			WorkbenchSingleton.Workbench.CloseViews();
		}
	}
	
}
