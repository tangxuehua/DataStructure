
using System;

namespace NetFocus.Components.AddIns.Exceptions
{
	/// <summary>
	/// Is thrown when the AddInTree could not find the requested path.
	/// </summary>
	public class AddInLoadException : Exception
	{
		/// <summary>
		/// Constructs a new <see cref="AddInLoadException"/>
		/// </summary>
		public AddInLoadException(string reason) : base(reason)
		{
		}
	}
}
