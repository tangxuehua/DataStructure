
using System;
using System.Collections;
using System.CodeDom.Compiler;
using System.Windows.Forms;

namespace NetFocus.DataStructure.Commands
{
	public abstract class AbstractMenuCommand : AbstractCommand, IMenuCommand
	{
		bool isEnabled = true;
		bool isChecked = false;
		
		public virtual bool IsEnabled {
			get {
				return isEnabled;
			}
			set {
				isEnabled = value;
			}
		}
		
		public virtual bool IsChecked {
			get {
				return isChecked;
			}
			set {
				isChecked = value;
			}
		}
		
	}
}
