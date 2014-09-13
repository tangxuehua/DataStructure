using System;
using System.Drawing;
using System.Windows.Forms;


namespace NetFocus.DataStructure.Gui 
{
	/// <summary>
	/// The IPadContent interface is the basic interface to all "tool" windows in DataStructure
	/// </summary>
	public interface IPadContent : IDisposable
	{
		/// <summary>
		/// Returns or Set the title of the pad.
		/// </summary>
		string Title {
			get;
		}
		
		/// <summary>
		/// Returns or Set the icon of the pad. May be null, if the pad has no
		/// icon defined.
		/// </summary>
		Bitmap Icon {
			get;
		}

		/// <summary>
		/// Returns the menu shortcut for the view menu item.
		/// </summary>
		string[] Shortcut 
		{
			get;
			set;
		}
		
		/// <summary>
		/// Returns the Windows.Control for this pad.
		/// </summary>
		Control Control {
			get;
		}
		
		/// <summary>
		/// Re-initializes all components of the pad. Don't call unless
		/// you know what you do.
		/// </summary>
		void RedrawContent();
		/// <summary>
		/// Is called when the title of this pad has changed.
		/// </summary>
		event EventHandler TitleChanged;
		/// <summary>
		/// Is called when the icon of this pad has changed.
		/// </summary>
		event EventHandler IconChanged;

	}
}
