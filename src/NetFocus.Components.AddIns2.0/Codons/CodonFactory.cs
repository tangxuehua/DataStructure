
using System;
using System.Collections;
using System.Xml;

using NetFocus.Components.AddIns.Exceptions;

namespace NetFocus.Components.AddIns.Codons
{
	/// <summary>
	/// Creates a new <code>ICodon</code> object.
	/// </summary>
	public class CodonFactory
	{
		Hashtable codonBuilderHashtable = new Hashtable();
		
		/// <remarks>
		/// Adds a new builder to this factory. After the builder is added
		/// codons from the builder type can be created by the factory
		/// </remarks>
		/// <exception cref="DuplicateCodonException">
		/// Is thrown when a codon builder with the same <code>CodonName</code>
		/// was already inserted
		/// </exception>
		public void AddCodonBuilder(CodonBuilder builder)
		{
			if (codonBuilderHashtable[builder.CodonType] != null) {
				throw new DuplicateCodonException(builder.CodonType);
			}
			codonBuilderHashtable[builder.CodonType] = builder;
		}
		
		/// <remarks>
		/// Creates a new <code>ICodon</code> object using  <code>codonNode</code>
		/// as a mark of which builder to take for creation.
		/// </remarks>
		public ICodon CreateCodon(AddIn addIn, XmlNode codonNode)
		{
			//通过代码子名字得到一个代码子创建器。
			CodonBuilder builder = codonBuilderHashtable[codonNode.Name] as CodonBuilder;
			
			if (builder != null) {
				return builder.BuildCodon(addIn);
			}
			
			throw new CodonNotFoundException(String.Format("no codon builder found for <{0}>", codonNode.Name));
		}
	}
}
