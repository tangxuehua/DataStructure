
using System;
using System.Diagnostics;
using System.Drawing;
using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.TextEditor.Undo;


namespace NetFocus.DataStructure.TextEditor.Undo
{
	/// <summary>
	/// This class is for the undo of Document replace operations
	/// </summary>
	public class UndoableReplace : IUndoableOperation
	{
		IDocument document;
		int       offset;
		string    text;
		string    origText;
			
		public UndoableReplace(IDocument document, int offset, string origText, string text)
		{
			if (document == null) {
				throw new ArgumentNullException("ÎÄµµ");
			}
			if (offset < 0 || offset > document.TextLength) {
				throw new ArgumentOutOfRangeException("Î»ÖÃ");
			}

			this.document = document;
			this.offset   = offset;
			this.origText = origText;
			this.text     = text;
		}

		public void Undo()
		{
			document.UndoStack.AcceptChanges = false;
			document.Replace(offset, text.Length, origText);
			document.UndoStack.AcceptChanges = true;
		}
		
		public void Redo()
		{
			document.UndoStack.AcceptChanges = false;
			document.Replace(offset, origText.Length, text);
			document.UndoStack.AcceptChanges = true;
		}
	}
}
