
using System;
using System.IO;

namespace NetFocus.DataStructure.Gui
{
	/// <summary>
	/// This class defines the DataStructure display binding interface, it is a factory
	/// structure, which creates IViewContents.
	/// </summary>
	public interface IViewType
	{
		/// <remarks>
		/// This function determines, if this display binding is able to create
		/// an IViewContent for the file given by fileName.
		/// </remarks>
		/// <returns>
		/// true, if this display binding is able to create
		/// an IViewContent for the file given by fileName.
		/// false otherwise
		/// </returns>
		bool CanCreateContentForFile(string fileName);
		
		/// <remarks>
		/// Creates a new IViewContent object for the file fileName
		/// </remarks>
		/// <returns>
		/// A newly created IViewContent object.
		/// </returns>
		IViewContent CreateContentForFile(string fileName);
		
		/// <remarks>
		/// This function determines, if this display binding is able to create
		/// an IViewContent for the language given by languageName.
		/// </remarks>
		/// <returns>
		/// true, if this display binding is able to create
		/// an IViewContent for the language given by languageName.
		/// false otherwise
		/// </returns>
		bool CanCreateContentForLanguage(string languageName);
		
		/// <remarks>
		/// Creates a new IViewContent object for the language given by 
		/// languageName with the content given by content
		/// </remarks>
		/// <returns>
		/// A newly created IViewContent object.
		/// </returns>
		IViewContent CreateContentForLanguage(string languageName, string content);
	}
}
