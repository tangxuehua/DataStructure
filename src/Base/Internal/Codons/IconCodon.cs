
using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

using NetFocus.Components.AddIns.Codons;
using NetFocus.Components.AddIns.Attributes;
using NetFocus.Components.AddIns.Conditions;

namespace NetFocus.DataStructure.AddIns.Codons
{
	[Codon("Icon")]
	public class IconCodon : AbstractCodon
	{
		[PathAttribute()]
		[XmlMemberAttribute("location")]
		string location  = null;
		
		[XmlMemberAttributeAttribute("language")]
		string language  = null;

		[XmlMemberAttributeAttribute("resource")]
		string resource  = null;
		
		[XmlMemberArrayAttribute("extensions")]
		string[] extensions = null;
		
		
		public string Language {
			get {
				return language;
			}
			set {
				language = value;
			}
		}
		
		public string Location {
			get {
				return location;
			}
			set {
				location = value;
			}
		}

		public string Resource 
		{
			get 
			{
				return resource;
			}
			set 
			{
				resource = value;
			}
		}
		
		public string[] Extensions {
			get {
				return extensions;
			}
			set {
				extensions = value;
			}
		}
		
		public override object BuildItem(object owner, ArrayList subItems, ConditionCollection conditions)
		{
			if (subItems.Count > 0) {
				throw new ApplicationException("more than one level of icons don't make sense!");
			}
			
			return this;
		}
		
	}
}
