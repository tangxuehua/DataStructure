
using System.Drawing;
using System.Windows.Forms;
using System;

using NetFocus.DataStructure.TextEditor.Document;


namespace NetFocus.DataStructure.TextEditor.Actions 
{
	public class ToggleBookmark : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			textArea.Document.BookmarkManager.ToggleMarkAt(textArea.Caret.Line);
			textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, textArea.Caret.Line));
			textArea.Document.OnUpdateCommited();
			
		}
	}
	
	public class GotoPrevBookmark : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			int lineNumber = textArea.Document.BookmarkManager.GetPrevMark(textArea.Caret.Line);
			if (lineNumber >= 0 && lineNumber < textArea.Document.TotalNumberOfLines) {
				textArea.Caret.Line = lineNumber;
			}
			textArea.SelectionManager.ClearSelection();
		}
	}
	
	public class GotoNextBookmark : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			int lineNumber = textArea.Document.BookmarkManager.GetNextMark(textArea.Caret.Line);
			if (lineNumber >= 0 && lineNumber < textArea.Document.TotalNumberOfLines) {
				textArea.Caret.Line = lineNumber;
			}
			textArea.SelectionManager.ClearSelection();
		}
	}
	
	public class ClearAllBookmarks : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			textArea.Document.BookmarkManager.Clear();
			textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
			textArea.Document.OnUpdateCommited();
		}
	}
}
