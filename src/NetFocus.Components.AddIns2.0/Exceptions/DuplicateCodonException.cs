
using System;

namespace NetFocus.Components.AddIns.Exceptions
{
	/// <summary>
	/// Is thrown when the AddInTree could not find the requested path.
	/// </summary>
	public class DuplicateCodonException : Exception
	{
		/// <summary>
		/// Constructs a new <see cref="DuplicateCodonException"/> instance.
		/// </summary>
		public DuplicateCodonException(string codon) : base("there already exists a codon with name : " + codon)
		{
		}
	}
}
