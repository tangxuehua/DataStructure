
using System;

namespace NetFocus.Components.AddIns.Exceptions
{
	/// <summary>
	/// Is thrown when the AddInTree could not find the requested path.
	/// </summary>
	public class ConditionWithoutRequiredAttributesException : Exception
	{
		/// <summary>
		/// Constructs a new <see cref="ConditionWithoutRequiredAttributesException"/>
		/// </summary>
		public ConditionWithoutRequiredAttributesException() : base("conditions need at least one required attribute to be identified.")
		{
		}
	}
}
