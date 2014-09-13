
using System;
using System.Reflection;

namespace NetFocus.Components.AddIns.Attributes
{
	/// <summary>
	/// Indicates that class represents a codon.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class CodonAttribute : Attribute
	{
		string type;
		
		/// <summary>
		/// Creates a new instance.
		/// </summary>
        public CodonAttribute(string type) 
		{
			this.type = type;
		}
		
		/// <summary>
		/// Returns the name of the codon.
		/// </summary>
		public string Type {
			get { 
				return type; 
			}
			set { 
				type = value; 
			}
		}
	}
}
