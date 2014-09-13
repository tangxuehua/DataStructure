
using System;
using System.Windows.Forms;

using NetFocus.Components.AddIns.Codons;
using NetFocus.DataStructure.Gui.XmlForms;


namespace NetFocus.DataStructure.Gui.Dialogs.OptionPanels
{
	public abstract class AbstractOptionPanel : BaseXmlUserControl, IDialogPanel
	{
		bool   wasActivated = false;
		bool   enableFinish   = true;
		object customizationObject = null;
		
		public bool WasActivated 
		{
			get 
			{
				return wasActivated;
			}
		}
		
		public Control Control 
		{
			get {
				return this;
			}
		}
		
		public virtual object CustomizationObject {
			get {
				return customizationObject;
			}
			set {
				customizationObject = value;
				OnCustomizationObjectChanged();
			}
		}
		
		public virtual bool EnableFinish {
			get {
				return enableFinish;
			}
			set {
				if (enableFinish != value) {
					enableFinish = value;
					OnEnableFinishChanged();
				}
			}
		}
		
		
		public AbstractOptionPanel(string fileName) : base(fileName)
		{
		}
		
		public AbstractOptionPanel() : base("")
		{
		}
		
		
		public virtual bool ReceiveDialogMessage(DialogMessage message)
		{
			switch (message) {
				case DialogMessage.Activated:
					if (!wasActivated) {//如果该面板还未被激活
						LoadPanelContents();
						wasActivated = true;
					}
					break;
				case DialogMessage.OK:
					if (wasActivated) {//如果该面板已被激活
						return StorePanelContents();
					}
					break;
			}
			
			return true;
		}
		
		
		public virtual void LoadPanelContents()
		{
			
		}
		
		public virtual bool StorePanelContents()
		{
			return true;
		}
		
		
		protected virtual void OnEnableFinishChanged()
		{
			if (EnableFinishChanged != null) {
				EnableFinishChanged(this, null);
			}
		}
		protected virtual void OnCustomizationObjectChanged()
		{
			if (CustomizationObjectChanged != null) {
				CustomizationObjectChanged(this, null);
			}
		}
		
		public event EventHandler CustomizationObjectChanged;
		public event EventHandler EnableFinishChanged;
	}
}
