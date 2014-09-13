
using System;
using System.Collections;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using NetFocus.Components.AddIns;
using NetFocus.Components.AddIns.Codons;
using NetFocus.DataStructure.Properties;


namespace NetFocus.DataStructure.Gui.Dialogs.OptionPanels
{
	public enum DialogMessage {
		OK,
		Cancel,
		Help,
		Next,
		Prev,
		Finish,
		Activated
	}
	
	public interface IDialogPanel
	{
		object CustomizationObject {
			get;
			set;
		}
		
		Control Control {
			get;
		}
		
		bool EnableFinish {
			get;
		}
		
		/// <returns>
		/// true, if the DialogMessage could be executed.
		/// </returns>
		bool ReceiveDialogMessage(DialogMessage message);
		
		event EventHandler EnableFinishChanged;
	}
}
