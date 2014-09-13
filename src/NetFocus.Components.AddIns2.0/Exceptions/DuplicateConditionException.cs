
using System;
using System.Collections.Specialized;

namespace NetFocus.Components.AddIns.Exceptions
{
	/// <summary>
	/// Is thrown when the AddInTree could not find the requested path.
	/// </summary>
	public class DuplicateConditionException : Exception
	{
		/// <summary>
		/// Constructs a new <see cref="DuplicateConditionException"/>
		/// </summary>
		public DuplicateConditionException(StringCollection attributes) : base("there already exists a condition with the required attributes : " + GenAttrList(attributes))
		{
		}
		
		static string GenAttrList(StringCollection attributes) 
		{
			string tmp = "";
			foreach (string attrib in attributes) {
				tmp += " " + attrib;
			}
			return tmp;
		}
	}
	
}
