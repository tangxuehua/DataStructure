
using System;
using System.Collections;
using System.Reflection;

using NetFocus.Components.AddIns.Attributes;

namespace NetFocus.Components.AddIns.Codons
{
	/// <summary>
	/// An abstract implementation of the <code>ICodon</code> interface.
	/// </summary>
	public abstract class AbstractCodon : ICodon
	{
		[XmlMemberAttribute("id", IsRequired=true)]
		string id = null;
		
		[XmlMemberAttributeAttribute("class")]
		string myClass = null;
		
		[XmlMemberArrayAttribute("insertafter")]
		string[] insertafter = null;
		
		[XmlMemberArrayAttribute("insertbefore")]
		string[] insertbefore = null;
		
		AddIn  addIn = null;
		
		/// <summary>
		/// Returns the AddIn in which the codon is defined.
		/// </summary>
		public AddIn AddIn {
			get {
				return addIn;
			}
			set {
				addIn = value;
			}
		}
		
		/// <summary>
		/// Returns the Name of the codon. (XmlNode name)
		/// </summary>
		public string Name {
			get {
				string name = null;
				CodonNameAttribute codonName = (CodonNameAttribute)Attribute.GetCustomAttribute(GetType(), typeof(CodonNameAttribute));
				if (codonName != null) {
					name = codonName.Name;
				}
				return name;
			}
		}
		
		/// <summary>
		/// Returns the uniqe ID of the codon.
		/// </summary>
		public string ID {
			get {
				return id;
			}
			set {
				id = value;
			}
		}
		
		/// <summary>
		/// Returns the class attribute of the codon
		/// (this is optional, but for most codons useful, therefore
		/// it is in the base class).
		/// </summary>
		public string Class {
			get {
				return myClass;
			}
			set {
				myClass = value;
			}
		}
		
		/// <summary>
		/// Insert this codon after the InsertAfter codon ID.
		/// </summary>
		public string[] InsertAfter {
			get {
				return insertafter;
			}
			set {
				insertafter = value;
			}
		}
		
		/// <summary>
		/// Insert this codon before the InsertAfter codon ID.
		/// </summary>
		public string[] InsertBefore {
			get {
				return insertbefore;
			}
			set {
				insertbefore = value;
			}
		}
		
		/// <summary>
		/// Creates an item with the specified sub items. And the current
		/// Condition status for this item.
		/// </summary>
		public abstract object BuildItem(object owner, ArrayList subItems);
	}
}
