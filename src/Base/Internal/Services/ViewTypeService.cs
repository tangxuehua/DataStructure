using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using NetFocus.Components.AddIns;
using NetFocus.DataStructure.AddIns.Codons;
using NetFocus.DataStructure.Gui;

namespace NetFocus.DataStructure.Services
{
	/// <summary>
	/// This class handles the installed view types
	/// and provides a simple access point to these viewTypes.
	/// </summary>
	public class ViewTypeService : AbstractService
	{
		readonly static string viewTypesPath = "/DataStructure/Workbench/ViewTypes";
		IViewType[] viewTypes = null;
		
		public ViewTypeService()
		{
			//first,get all the displayBinding condons
			viewTypes = (IViewType[])AddInTreeSingleton.AddInTree.GetTreeNode(viewTypesPath).BuildChildItems(this).ToArray(typeof(IViewType));
		}
		
		
		public IViewType GetViewTypePerFileName(string filename)
		{
			foreach (IViewType viewType in viewTypes) 
			{
				if (viewType.CanCreateContentForFile(filename)) 
				{
					return viewType;
				}
			}
			return null;
		}
		
		
		public IViewType GetViewTypePerLanguageName(string languagename)
		{
			foreach (IViewType viewType in viewTypes) 
			{
				if (viewType.CanCreateContentForLanguage(languagename)) 
				{
					return viewType;
				}
			}
			return null;
		}
		
	}
}
