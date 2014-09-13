
using System;
using System.Collections;
using System.Xml;

namespace NetFocus.Components.AddIns.Exceptions
{
	public class CodonNotFoundException : System.Exception
	{
		public CodonNotFoundException(string message) : base(message)
		{
		}
	}
}
