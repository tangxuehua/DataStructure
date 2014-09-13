
using System;
using System.Collections;

namespace NetFocus.DataStructure.Gui
{
	/// <summary>
	/// This is the basic interface to the workspace.
	/// </summary>
	public interface IWorkbench
	{
		/// <summary>
		/// The title shown in the title bar.
		/// </summary>
		string Title {
			get;
			set;
		}
		
		/// <summary>
		/// A collection in which all active workspace windows are saved.
		/// </summary>
		ViewContentCollection ViewContentCollection {
			get;
		}
		
		/// <summary>
		/// A collection in which all pads are saved.
		/// </summary>
		PadContentCollection PadContentCollection {
			get;
		}
		
		/// <summary>
		/// 当前工作台所对应的布局管理器
		/// </summary>
		IWorkbenchLayout WorkbenchLayout {
			get;
			set;
		}
		
		/// <summary>
		/// The active workbench window.
		/// </summary>
		IViewContent ActiveViewContent {
			get;
		}
		/// <summary>
		/// Inserts a new <see cref="IViewContent"/> object in the workspace.
		/// </summary>
		void ShowView(IViewContent content);
		/// <summary>
		/// Inserts a new <see cref="IPadContent"/> object in the workspace.
		/// </summary>
		void ShowPad(IPadContent content);
		/// <summary>
		/// Returns a pad from a specific type.
		/// </summary>
		IPadContent GetPad(Type type);
		/// <summary>
		/// Closes the IViewContent content when content is open.
		/// </summary>
		void CloseView(IViewContent content);
		/// <summary>
		/// Closes all views inside the workbench.
		/// </summary>
		void CloseViews();


	}
}
