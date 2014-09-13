using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

using DevComponents.DotNetBar;


namespace NetFocus.DataStructure.Commands
{
	public interface ISubmenuBuilder
	{
		ButtonItem[] BuildSubmenu(object owner);
	}
}
