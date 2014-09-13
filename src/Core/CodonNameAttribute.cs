using System;
using System.Reflection;

namespace NetFocus.Components.AddIns.Attributes
{
	/// <summary>
	/// Indicates that class represents a codon.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class CodonNameAttribute : Attribute
	{
		string name;
		
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public CodonNameAttribute(string name) 
		{
			this.name = name;
		}
		
		/// <summary>
		/// Returns the name of the codon.
		/// </summary>
		public string Name {
			get { 
				return name; 
			}
			set { 
				name = value; 
			}
		}
	}
}
