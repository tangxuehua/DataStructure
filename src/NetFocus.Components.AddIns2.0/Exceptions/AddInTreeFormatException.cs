
using System;

namespace NetFocus.Components.AddIns.Exceptions
{
	/// <summary>
	/// Is thrown when the xml has a false format.
	/// </summary>
	public class AddInTreeFormatException : Exception
	{
		/// <summary>
		/// Constructs a new <see cref="AddInTreeFormatException"/>
		/// </summary>
		public AddInTreeFormatException(string msg) : base("error reading the addin xml : " + msg)
		{
		}
	}
}
