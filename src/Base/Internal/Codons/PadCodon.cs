using System;
using System.Collections;
using System.Reflection;
using System.Diagnostics;

using NetFocus.DataStructure.Gui;
using NetFocus.Components.AddIns.Codons;
using NetFocus.Components.AddIns.Attributes;
using NetFocus.Components.AddIns.Conditions;

namespace NetFocus.DataStructure.AddIns.Codons
{
	[Codon("Pad")]
	public class PadCodon : AbstractCodon
	{
		[XmlMemberArrayAttribute("shortcut", Separator = new char[]{ '|'})]
		string[] shortcut = null;
		
		public string[] Shortcut {
			get {
				return shortcut;
			}
			set {
				shortcut = value;
			}
		}
		
		public override object BuildItem(object owner, ArrayList subItems, ConditionCollection conditions)
		{
			Debug.Assert(Class != null && Class.Length > 0);
			IPadContent pad = AddIn.CreateObject(Class) as IPadContent;
			pad.Shortcut = shortcut;
			
			return pad;
		}
	}
}
