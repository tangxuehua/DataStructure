
using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using NetFocus.Components.AddIns;

using NetFocus.DataStructure.Properties;
using NetFocus.Components.AddIns.Codons;

using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.Gui.Dialogs;


namespace NetFocus.DataStructure.Commands
{
	public class Undo : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (content != null && content is IEditable) {
				((IEditable)content).Undo();
				content.Control.Refresh();
			}
		}
	}
	public class Redo : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (content != null && content is IEditable) {
				((IEditable)content).Redo();
				content.Control.Refresh();
			}
		}
	}

	public class Cut : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (content != null && content is IEditable) {
				if (((IEditable)content).ClipboardHandler != null) {
					((IEditable)content).ClipboardHandler.Cut(null, null);
				}
			}
		}
	}
	
	public class Copy : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (content != null && content is IEditable) {
				if (((IEditable)content).ClipboardHandler != null) {
					((IEditable)content).ClipboardHandler.Copy(null, null);
				}
			}
		}
	}
	
	public class Paste : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (content != null && content is IEditable) {
				if (((IEditable)content).ClipboardHandler != null) {
					((IEditable)content).ClipboardHandler.Paste(null, null);
				}
			}
		}
	}
	
	public class Delete : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (content != null && content is IEditable) {
				if (((IEditable)content).ClipboardHandler != null) {
					((IEditable)content).ClipboardHandler.Delete(null, null);
				}
			}
		}
	}
	
	public class SelectAll : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (content != null && content is IEditable) {
				if (((IEditable)content).ClipboardHandler != null) {
					((IEditable)content).ClipboardHandler.SelectAll(null, null);
				}
			}
		}
	}

	public class WordCount : AbstractMenuCommand
	{
		public override void Run()
		{
			using (WordCountDialog wcd = new WordCountDialog()) {
				wcd.Owner = (Form)WorkbenchSingleton.Workbench;
				wcd.ShowDialog();
			}
		}
	}
}
