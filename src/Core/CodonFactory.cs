using System;
using System.Collections;
using System.Xml;

namespace NetFocus.Components.AddIns.Codons
{
	/// <summary>
	/// Creates a new <code>ICodon</code> object.
	/// 这里用到了抽象工厂设计模式（而且这个抽象工厂模式是基于类来设计的）。
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
			if (codonBuilderHashtable[builder.CodonName] != null) {
				throw new Exception("已经存在一个名为 : " + builder.CodonName + " 的代码子");
			}
			codonBuilderHashtable[builder.CodonName] = builder;
		}
		
		/// <remarks>
		/// Creates a new <code>ICodon</code> object using  <code>codonNode</code>
		/// as a mark of which builder to take for creation.
		/// </remarks>
		public ICodon CreateCodon(AddIn addIn, XmlNode codonNode)
		{
			CodonBuilder builder = codonBuilderHashtable[codonNode.Name] as CodonBuilder;
			
			if (builder != null) {
				return builder.BuildCodon(addIn);
			}
			
			throw new Exception(String.Format("no codon builder found for <{0}>", codonNode.Name));
		}
	}
}
