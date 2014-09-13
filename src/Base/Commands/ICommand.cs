

using System;
using System.Collections;
using System.CodeDom.Compiler;
using System.Windows.Forms;

namespace NetFocus.DataStructure.Commands
{
	/// <summary>
	/// A basic command interface. A command has simply an owner which "runs" the command
	/// and a Run method which invokes the command.
	/// </summary>
	public interface ICommand
	{
		
		/// <summary>
		/// Returns the owner of the command.
		/// </summary>
		object Owner {
			get;
			set;
		}
		
		/// <summary>
		/// Invokes the command.
		/// </summary>
		void Run();
	}
}
