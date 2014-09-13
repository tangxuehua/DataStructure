
using System;
using System.Drawing.Printing;

namespace NetFocus.DataStructure.Gui
{
	/// <summary>
	/// If a IViewContent object is from the type IPrintable it signals
	/// that it's contents could be printed to a printer, fax etc.
	/// </summary>
	public interface IPrintable
	{
		/// <summary>
		/// Returns the PrintDocument for this object, see the .NET reference
		/// for more information about printing.
		/// </summary>
		PrintDocument PrintDocument {
			get;
		}
	}
}
