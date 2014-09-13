
using System;

namespace NetFocus.Components.AddIns.Exceptions
{
	/// <summary>
	/// Is thrown when the AddInTree could not find the requested path.
	/// </summary>
	public class AddInInitializeException : Exception
	{
		/// <summary>
		/// Constructs a new <see cref="AddInInitializeException"/>
		/// </summary>
		public AddInInitializeException(string fileName, Exception e) : base("Could not load add-in file : " + fileName + "\n exception got :" + e.ToString())
		{
		}
	}
}
