
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Xml;

using NetFocus.Components.AddIns.Conditions;
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
		/// Returns the default condition factory. ICondition objects
		/// are created only with this factory during the tree 
		/// construction process.
		/// </summary>
		ConditionFactory ConditionFactory {
			get;
		}

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
		/// Searches a requested path and returns the TreeNode in this path as value.
		/// If path is <code>null</code> or path.Length is zero the root node is returned.
		/// </summary>
		/// <param name="path">
		/// The path inside the tree structure.
		/// </param>
		/// <exception cref="TreePathNotFoundException">
		/// Is thrown when the path is not found in the tree.
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
		Assembly LoadAssemblyFromFile(string assemblyFile);
	}
}
