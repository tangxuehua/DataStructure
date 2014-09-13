
using System;
using System.Diagnostics;
using System.Collections;

namespace NetFocus.DataStructure.TextEditor.Undo
{
	/// <summary>
	/// This class stacks the last x operations from the undostack and makes
	/// one undo/redo operation from it.
	/// </summary>
	public class UndoQueue : IUndoableOperation
	{
		ArrayList undolist = new ArrayList();
		
		public UndoQueue(UndoStack stack, int numops)
		{
			if (stack == null)  {
				throw new ArgumentNullException("stack");
			}
			
			for (int i = 0; i < numops; ++i) {
				if (stack._UndoStack.Count > 0) {
					undolist.Add(stack._UndoStack.Pop());
				}
			}
		}
		public void Undo()
		{
			for (int i = 0; i < undolist.Count; ++i) {
				((IUndoableOperation)undolist[i]).Undo();
			}
		}
		
		public void Redo()
		{
			for (int i = undolist.Count - 1 ; i >= 0 ; --i) {
				((IUndoableOperation)undolist[i]).Redo();
			}
		}		
	}
}
