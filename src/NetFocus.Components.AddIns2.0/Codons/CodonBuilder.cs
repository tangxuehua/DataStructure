
using System;
using System.Reflection;

using NetFocus.Components.AddIns.Exceptions;
using NetFocus.Components.AddIns.Attributes;

namespace NetFocus.Components.AddIns.Codons
{
	/// <summary>
	/// The condition builder builds a new codon
	/// </summary>
	public class CodonBuilder
	{
		Assembly assembly;
		string className;
		string codonType;
		
		/// <summary>
		/// Initializes a new CodonBuilder instance with beeing
		/// className the name of the condition class and assembly
		/// in which the class is defined.
		/// </summary>
		public CodonBuilder(string className, Assembly assembly)
		{
			this.assembly  = assembly;
			this.className = className;
			
			// get codon name from attribute
			CodonAttribute codonTypeAttribute = (CodonAttribute)Attribute.GetCustomAttribute(assembly.GetType(ClassName), typeof(CodonAttribute));
			codonType = codonTypeAttribute.Type;
		}
		
		/// <summary>
		/// Returns the className the name of the condition class;
		/// </summary>
		public string ClassName {
			get {
				return className;
			}
		}
		
		/// <summary>
		/// Returns the type of the codon
		/// </summary>
		public string CodonType {
			get {
				return codonType;
			}
		}
		
		/// <summary>
		/// Returns a newly build <code>ICodon</code> object.
		/// </summary>
		public ICodon BuildCodon(AddIn addIn)
		{
			ICodon codon;
			try {
				// create instance (ignore case)
				codon = (ICodon)assembly.CreateInstance(ClassName, true);
				
				// set default values
				codon.AddIn = addIn;
			} catch (Exception) {
				codon = null;
			}
			return codon;
		}
		
	}
}
