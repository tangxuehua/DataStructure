
using System;

namespace NetFocus.DataStructure.Gui
{
	/// <summary>
	/// The IWorkbenchLayout object is responsible for the layout of 
	/// the workspace, it shows the contents, chooses the IWorkbenchWindow
	/// implementation etc. it could be attached/detached at the runtime
	/// to a workbench.
	/// </summary>
	public interface IWorkbenchLayout
	{
		/// <summary>
		/// The active workbench window.
		/// </summary>
		IViewContent ActiveViewContent {
			get;
		}
		/// <summary>
		/// Attaches this layout manager to a workbench object.
		/// </summary>
		void Attach(IWorkbench workbench);
		/// <summary>
		/// Detaches this layout manager from the current workbench object.
		/// </summary>
		void Detach();
		/// <summary>
		/// Get a <see cref="IPadContent"/>.
		/// </summary>
		IPadContent GetPad(Type type);
		/// <summary>
		/// Shows a new <see cref="IPadContent"/>.
		/// </summary>
		void ShowPad(IPadContent content);
		/// <summary>
		/// Shows PadCollection <see cref="PadContentCollection"/>.
		/// </summary>
		void ShowPads(PadContentCollection contentCollection);
		/// <summary>
		/// Activates a pad (Show only makes it visible but Activate does
		/// bring it to foreground)
		/// </summary>
		void ActivatePad(IPadContent content);
		/// <summary>
		/// Hides a new <see cref="IPadContent"/>.
		/// </summary>
		void HidePad(IPadContent content);
		/// <summary>
		/// returns true, if padContent is visible;
		/// </summary>
		bool IsVisible(IPadContent padContent);
		/// <summary>
		/// Show a new <see cref="IViewContent"/>.
		/// </summary>
		void ShowView(IViewContent content);
		/// <summary>
		/// Close a  <see cref="IViewContent"/>.
		/// </summary>
		void CloseView(IViewContent content);
		/// <summary>
		/// Close all the <see cref="IViewContent"/> in the workbench.
		/// </summary>
		void CloseViews();
		/// <summary>
		/// Is called, when the workbench window which the user has into
		/// the foreground (e.g. editable) changed to a new one.
		/// </summary>
		event EventHandler ActiveViewContentChanged;

	}
}
