
using System;
using System.Reflection;

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
		string codonName;
		
		/// <summary>
		/// Initializes a new CodonBuilder instance with the
		/// className and the assembly in which the class is defined.
		/// </summary>
		public CodonBuilder(string className, Assembly assembly)
		{
			this.assembly  = assembly;
			this.className = className;
			
			// get codon name from attribute
			CodonNameAttribute codonNameAttribute = (CodonNameAttribute)Attribute.GetCustomAttribute(assembly.GetType(ClassName), typeof(CodonNameAttribute));
			codonName = codonNameAttribute.Name;
		}
		
		/// <summary>
		/// Returns the className the name of the condon class;
		/// </summary>
		public string ClassName {
			get {
				return className;
			}
		}
		
		/// <summary>
		/// Returns the name of the codon (it is used to determine which xml element
		/// represents which codon.
		/// </summary>
		public string CodonName {
			get {
				return codonName;
			}
		}
		
		/// <summary>
		/// Returns a newly build <code>ICodon</code> object.
		/// </summary>
		public ICodon BuildCodon(AddIn addIn)
		{
			ICodon codon;
			try {
				// create a codon instance (ignore case)
				codon = (ICodon)assembly.CreateInstance(ClassName, true);
				
				// 使该代码子(一个功能)和当前插件关联
				codon.AddIn = addIn;
			} catch (Exception) {
				codon = null;
			}
			return codon;
		}
		
	}
}
