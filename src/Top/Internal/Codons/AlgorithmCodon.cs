using System;
using System.Collections;
using System.Diagnostics;

using NetFocus.DataStructure.Internal.Algorithm;
using NetFocus.Components.AddIns.Attributes;
using NetFocus.Components.AddIns.Codons;
using NetFocus.Components.AddIns.Conditions;

namespace NetFocus.DataStructure.AddIns.Codons
{
	[Codon("Algorithm")]
	public class AlgorithmCodon : AbstractCodon
	{
		[XmlMemberArrayAttribute("codeFiles", IsRequired=true)]
		string[] codeFiles = null;

		[XmlMemberArrayAttribute("lastLines", IsRequired=true)]
		string[] lastLines;

		int[] lastLines1;

		public string[] CodeFiles {
			get{
				return codeFiles;
			}
			set{
				codeFiles = value;
			}
		}
		public string[] LastLines {
			get{
				return lastLines;
			}
			set{
				lastLines = value;
			}
		}
		void AssignValues()
		{
			lastLines1 = new int[lastLines.Length];
			for(int i = 0;i < lastLines.Length;i++)
			{
				lastLines1[i] = Convert.ToInt32(lastLines[i]);
			}
		}
		/// <summary>
		/// Creates an item with the specified sub items. And the current
		/// Condition status for this item.
		/// </summary>
        public override object BuildItem(object owner, ArrayList subItems, ConditionCollection conditions)
        {
			Debug.Assert(Class != null && Class.Length > 0);
			IAlgorithm algorithm = (IAlgorithm)AddIn.CreateObject(Class);
			algorithm.CodeFiles = codeFiles;
			AssignValues();
			algorithm.LastLines = lastLines1;
			return algorithm;
		}
	}
}
