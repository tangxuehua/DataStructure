
using System;
using System.Collections;
using System.Diagnostics;

using NetFocus.DataStructure.Gui;
using NetFocus.Components.AddIns.Codons;
using NetFocus.Components.AddIns.Attributes;
using NetFocus.Components.AddIns.Conditions;

namespace NetFocus.DataStructure.AddIns.Codons
{
	[Codon("ViewType")]
	public class ViewTypeCodon : AbstractCodon
	{
		[XmlMemberArrayAttribute("supportedformats")]
		string[] supportedFormats;
		
		public string[] SupportedFormats {
			get {
				return supportedFormats;
			}
			set {
				supportedFormats = value;
			}
		}

		public override object BuildItem(object owner, ArrayList subItems, ConditionCollection conditions)
		{
			Debug.Assert(Class != null && Class.Length > 0);
			
			return (IViewType)AddIn.CreateObject(Class);
			
		}
	
	}
}
