using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

using NetFocus.Components.AddIns.Codons;
using NetFocus.Components.AddIns.Attributes;
using NetFocus.Components.AddIns.Conditions;

namespace NetFocus.DataStructure.AddIns.Codons
{
	[Codon("Toolbar")]
	public class ToolbarCodon : AbstractCodon
	{
		[XmlMemberAttribute("icon")]
		string icon        = null;
		[XmlMemberAttribute("text")]
		string text        = null;
		[XmlMemberAttributeAttribute("tooltip")]
		string toolTip     = null;
		[XmlMemberAttribute("begingroup")]
		string beginGroup  = null;
		
		ArrayList subItems = null;
		
		bool      enabled  = true;
		
		public string ToolTip 
		{
			get 
			{
				return toolTip;
			}
			set 
			{
				toolTip = value;
			}
		}
		
		public string Icon 
		{
			get 
			{
				return icon;
			}
			set 
			{
				icon = value;
			}
		}
		public string Text 
		{
			get 
			{
				return text;
			}
			set 
			{
				text = value;
			}
		}
		public string BeginGroup 
		{
			get 
			{
				return beginGroup;
			}
			set 
			{
				beginGroup = value;
			}
		}
		
		public ArrayList SubItems 
		{
			get 
			{
				return subItems;
			}
			set 
			{
				subItems = value;
			}
		}
		
		public bool Enabled 
		{
			get 
			{
				return enabled;
			}
			set 
			{
				enabled = value;
			}
		}
		public override object BuildItem(object owner, ArrayList subItems, ConditionCollection conditions)
		{
			this.subItems = subItems;
			return this;
		}
	}
}
