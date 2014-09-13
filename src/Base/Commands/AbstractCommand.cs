
using System;
using System.Collections;
using System.CodeDom.Compiler;
using System.Windows.Forms;

namespace NetFocus.DataStructure.Commands
{
	/// <summary>
	/// Abstract implementation of the <see cref="ICommand"/> interface.
	/// </summary>
	public abstract class AbstractCommand : ICommand
	{
		object owner = null;
		
		/// <summary>
		/// Returns the owner of the command.
		/// </summary>
		public virtual object Owner {
			get {
				return owner;
			}
			set {
				owner = value;
			}
		}
		
		/// <summary>
		/// Invokes the command.
		/// </summary>
		public abstract void Run();
	}
}
