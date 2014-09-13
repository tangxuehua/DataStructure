
using System;

namespace NetFocus.Components.AddIns.Exceptions
{
	/// <summary>
	/// Is thrown when the AddInTree could not create a specified object.
	/// </summary>
	public class TypeNotFoundException : Exception
	{
		/// <summary>
		/// Constructs a new <see cref="TypeNotFoundException"/>
		/// </summary>
		public TypeNotFoundException(string typeName) : base("Unable to create object from type : " + typeName)
		{
		}
	}
}
