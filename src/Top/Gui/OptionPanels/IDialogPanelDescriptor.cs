
using System;
using System.Collections;

namespace NetFocus.DataStructure.Gui.Dialogs.OptionPanels
{
	public interface IDialogPanelDescriptor
	{
		/// <value>
		/// Returns the ID of the dialog panel codon
		/// </value>
		string ID {
			get;
		}
		
		/// <value>
		/// Returns the label of the dialog panel
		/// </value>
		string Label {
			get;
			set;
		}
		
		ArrayList DialogPanelDescriptors {
			get;
			set;
		}
		
		/// <value>
		/// Returns the dialog panel object
		/// </value>
		IDialogPanel DialogPanel {
			get;
			set;
		}
	}
}
