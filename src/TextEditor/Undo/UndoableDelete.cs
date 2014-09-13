
using System;
using System.Diagnostics;
using System.Drawing;

using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.TextEditor.Undo;
	

namespace NetFocus.DataStructure.TextEditor.Undo
{
	/// <summary>
	/// This class is for the undo of Document delete operations
	/// </summary>
	public class UndoableDelete : IUndoableOperation
	{
		IDocument document;
		int      offset;
		string   text;

		public UndoableDelete(IDocument document, int offset, string text)
		{
			if (document == null) {
				throw new ArgumentNullException("ÎÄµµ");
			}
			if (offset < 0 || offset > document.TextLength) {
				throw new ArgumentOutOfRangeException("Î»ÖÃ");
			}
			
			this.document = document;
			this.offset   = offset;
			this.text     = text;
		}
	
		public void Undo()
		{
			document.UndoStack.AcceptChanges = false;
			document.Insert(offset, text);
			document.UndoStack.AcceptChanges = true;
		}
		
		public void Redo()
		{
			document.UndoStack.AcceptChanges = false;
			document.Remove(offset, text.Length);
			document.UndoStack.AcceptChanges = true;
		}
	}
}
