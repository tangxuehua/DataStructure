
using System;
using System.Collections;
using System.Reflection;
using NetFocus.Components.AddIns.Attributes;
using NetFocus.Components.AddIns.Codons;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Gui.Dialogs.OptionPanels;
using NetFocus.Components.AddIns.Conditions;

namespace NetFocus.DataStructure.AddIns.Codons
{
	[Codon("DialogPanel")]
	public class DialogPanelCodon : AbstractCodon
	{
		[XmlMemberAttribute("label", IsRequired=true)]
		string label       = null;
		
		public string Label {
			get {
				return label;
			}
			set {
				label = value;
			}
		}
		
		/// <summary>
		/// Creates an item with the specified sub items. And the current
		/// Condition status for this item.
		/// </summary>
        public override object BuildItem(object owner, ArrayList subItems, ConditionCollection conditions)
		{
			IDialogPanelDescriptor newItem = null;
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			if (subItems == null || subItems.Count == 0) {				
				if (Class != null) {
					newItem = new DefaultDialogPanelDescriptor(ID, stringParserService.Parse(Label), (IDialogPanel)AddIn.CreateObject(Class));
				} else {
					newItem = new DefaultDialogPanelDescriptor(ID, stringParserService.Parse(Label));
				}
			} else {
				newItem = new DefaultDialogPanelDescriptor(ID, stringParserService.Parse(Label), subItems);
			}
			return newItem;
		}
	}
}
