
using System;
using System.Reflection;

namespace NetFocus.Components.AddIns.Attributes
{
	[AttributeUsage(AttributeTargets.Field, Inherited=true)]
	public class PathAttribute : Attribute
	{
		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		public PathAttribute()
		{
		}
	}
}
