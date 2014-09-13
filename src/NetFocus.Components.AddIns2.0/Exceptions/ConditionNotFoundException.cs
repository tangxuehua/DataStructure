
using System;
using System.Collections;
using System.Xml;

namespace NetFocus.Components.AddIns.Exceptions
{
	public class ConditionNotFoundException : System.Exception
	{
		public ConditionNotFoundException(string message) : base(message)
		{
		}
	}
}
