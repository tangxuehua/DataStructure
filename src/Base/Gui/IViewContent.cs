
using System;
using System.Windows.Forms;

namespace NetFocus.DataStructure.Gui
{
	/// <summary>
	/// IViewContent is the base interface for all editable data
	/// inside DataStructure.
	/// </summary>
	public interface IViewContent : IDisposable
	{
		/// <summary>
		/// This is the Windows.Forms control which is inside the view.
		/// </summary>
		Control Control {
			get;
		}
		
		/// <summary>
		/// A generic name for the file, when it does have no file name
		/// (e.g. newly created files)
		/// </summary>
		string UntitledName {
			get;
			set;
		}
		
		/// <summary>
		/// This is the whole name of the content, e.g. the file name or
		/// the url depending on the type of the content.
		/// </summary>
		string ContentName {
			get;
			set;
		}
		
		/// <summary>
		/// If this property returns true the view is untitled.
		/// </summary>
		bool IsUntitled {
			get;
		}
		
		/// <summary>
		/// If this property returns true the content has changed since
		/// the last load/save operation.
		/// </summary>
		bool IsDirty {
			get;
			set;
		}
		
		/// <summary>
		/// If this property returns true the content could not be altered.
		/// </summary>
		bool IsReadOnly {
			get;
		}
		
		/// <summary>
		/// If this property returns true the content can't be written.
		/// </summary>
		bool IsViewOnly {
			get;
		}
		/// <summary>
		/// if the content is an algorithm,then return the type of the current algorithm
		/// </summary>
		Type AlgorithmType
		{
			get;
			set;
		}
		/// <summary>
		/// Reinitializes the content. (Re-initializes all add-in tree stuff)
		/// and redraws the content. Call this not directly unless you know
		/// what you do.
		/// </summary>
		void RedrawContent();
		/// <summary>
		/// Saves this content to the last load/save location.
		/// </summary>
		void SaveFile();
		/// <summary>
		/// Saves the content to the location <code>fileName</code>
		/// </summary>
		void SaveFile(string fileName);
		/// <summary>
		/// Loads the content from the location <code>fileName</code>
		/// </summary>
		void LoadFile(string fileName);
		/// <summary>
		/// Closes the view, if force == true it closes the view
		/// without ask, even the content is dirty.
		/// </summary>
		void CloseView(bool force);
		/// <summary>
		/// Brings this view to front and sets the user focus to this
		/// view.
		/// </summary>
		void SelectView();

		/// <summary>
		/// 用于触发WindowSelected事件
		/// </summary>
		/// <param name="e"></param>
		void OnViewSelected(EventArgs e);
		/// <summary>
		/// 用于触发TitleChanged事件
		/// </summary>
		/// <param name="e"></param>
		void OnContentNameChanged(EventArgs e);
		/// <summary>
		/// 用于触发CloseEvent事件
		/// </summary>
		/// <param name="e"></param>
		void OnCloseEvent(EventArgs e);
		/// <summary>
		/// 用于触发DirtyChanged事件
		/// </summary>
		/// <param name="e"></param>
		void OnDirtyChanged(EventArgs e);
		/// <summary>
		/// Is called when the view is selected.
		/// </summary>
		event EventHandler ViewSelected;
		/// <summary>
		/// Is called each time the name for the content has changed.
		/// </summary>
		event EventHandler ContentNameChanged;
		/// <summary>
		/// Is called after the view closes.
		/// </summary>
		event EventHandler CloseEvent;
		/// <summary>
		/// Is called when the content is changed after a save/load operation
		/// and this signals that changes could be saved.
		/// </summary>
		event EventHandler DirtyChanged;

	}
}
