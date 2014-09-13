using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

using NetFocus.Components.AddIns.Codons;
using NetFocus.Components.AddIns.Attributes;
using NetFocus.DataStructure.Services;
using NetFocus.Components.AddIns.Conditions;


namespace NetFocus.DataStructure.AddIns.Codons
{
	[Codon("FileFilter")]
	public class FileFilterCodon : AbstractCodon
	{
		[XmlMemberAttribute("name", IsRequired=true)]
		string filtername       = null;
		
		[XmlMemberArrayAttribute("extensions", IsRequired=true)]
		string[] extensions = null;
		
		public string FilterName {
			get {
				return filtername;
			}
			set {
				filtername = value;
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
				throw new ApplicationException("more than one level of file filters don't make sense!");
			}
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));

			return stringParserService.Parse(filtername) + "|" + String.Join(";", extensions);
		}
	}
}
