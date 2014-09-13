
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Xml;

using NetFocus.Components.AddIns.Codons;

namespace NetFocus.Components.AddIns
{
	/// <summary>
	/// This is the basic interface to add-in tree. You can always get
	/// a valid IAddInTree object in the <see cref="NetFocus.Components.AddIns.AddInTreeSingleton"/>
	/// class.
	/// </summary>
	public interface IAddInTree
	{
		/// <summary>
		/// Returns the default codon factory. ICodon objects
		/// are created only with this factory during the tree 
		/// construction process.
		/// </summary>
		CodonFactory CodonFactory {
			get;
		}

		/// <summary>
		/// Returns a collection of all loaded add-ins.
		/// </summary>
		AddInCollection AddIns {
			get;
		}
		
		/// <summary>
		/// Returns a TreeNode corresponding to <code>path</code>.
		/// </summary>
		/// <param name="path">
		/// The path.
		/// </param>
		/// <exception cref="TreePathNotFoundException">
		/// When the path <code>path</code> does not point to a codon
		/// in the tree.
		/// </exception>
		IAddInTreeNode GetTreeNode(string path);
		
		/// <summary>
		/// Inserts an AddIn into the AddInTree.
		/// </summary>
		void InsertAddIn(AddIn addIn);

		/// <summary>
		/// Removes an AddIn from the AddInTree.
		/// </summary>
		void RemoveAddIn(AddIn addIn);
		
		/// <summary>
		/// This method does load all codons and conditions in the given assembly.
		/// It will create builders for them which could be used by the factories
		/// to create the codon and condition objects.
		/// </summary>
		Assembly LoadAssembly(string assemblyFile);
	}
}
