

using System;
using System.Collections;
using System.Drawing;
using System.Text;

namespace NetFocus.DataStructure.TextEditor.Document
{
	/// <summary>
	/// A highlighting strategy for a buffer.
	/// </summary>
	public interface IHighlightingStrategy
	{
		/// <value>
		/// The name of the highlighting strategy, must be unique
		/// </value>
		string Name {
			get;
		}
		
		/// <value>
		/// The file extenstions on which this highlighting strategy gets
		/// used
		/// </value>
		string[] Extensions {
			set;
			get;
		}
		
		Hashtable Properties {
			get;
		}
		
		HighlightColor   GetEnvironmentColorForName(string name);
		
		HighlightRuleSet GetRuleSetForSpan(Span span);
		

		HighlightColor   GetColor(IDocument document, LineSegment keyWord, int index, int length);
		

		void MarkTokens(IDocument document);

		void MarkTokens(IDocument document, ArrayList lines);
		
	}
}
