
using System;
using System.Reflection;

namespace NetFocus.Components.AddIns.Attributes
{
	/// <summary>
	/// Indicates that class represents a condition.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
	public class ConditionAttribute : Attribute
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public ConditionAttribute()
		{
		}
	}
}
