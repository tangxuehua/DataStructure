
using System;
using System.Drawing;
using System.Collections;

namespace NetFocus.DataStructure.TextEditor.Document
{
	/// <summary>
	/// This interface is used for the folding capabilities
	/// of the textarea.
	/// </summary>
	public interface IFoldingStrategy
	{
		/// <remarks>
		/// Calculates the fold level of a specific line.
		/// </remarks>
		ArrayList GenerateFoldMarkers(IDocument document, string fileName, object parseInformation);
	}
}
