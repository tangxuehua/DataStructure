
using System;

namespace NetFocus.Components.AddIns.Exceptions
{
	/// <summary>
	/// Is thrown when the xml has a false format.
	/// </summary>
	public class AddInSignatureException : Exception
	{
		/// <summary>
		/// Constructs a new <see cref="AddInTreeFormatException"/>
		/// </summary>
		public AddInSignatureException(string msg) : base("signature failure : " + msg)
		{
		}
	}
}
