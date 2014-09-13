using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;


namespace NetFocus.DataStructure.Commands
{
	public interface IMenuCommand : ICommand
	{
		bool IsEnabled {
			get;
			set;
		}
		
		bool IsChecked {
			get;
			set;
		}
	}
}
