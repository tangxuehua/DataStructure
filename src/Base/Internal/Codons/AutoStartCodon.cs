using System;
using System.Collections;
using System.Diagnostics;

using NetFocus.Components.AddIns.Codons;
using NetFocus.Components.AddIns.Attributes;
using NetFocus.Components.AddIns.Conditions;

namespace NetFocus.DataStructure.AddIns.Codons
{
	[Codon("Autostart")]
	public class AutostartCodon : AbstractCodon
	{

		public override object BuildItem(object owner, ArrayList subItems, ConditionCollection conditions)
		{
			Debug.Assert(Class != null && Class.Length > 0);
			return AddIn.CreateObject(Class);
		}
		
	}
}