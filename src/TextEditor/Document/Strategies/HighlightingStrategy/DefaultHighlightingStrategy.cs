
using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Text;
using NetFocus.DataStructure.Properties;

using NetFocus.DataStructure.TextEditor;


namespace NetFocus.DataStructure.TextEditor.Document
{
	public class DefaultHighlightingStrategy : IHighlightingStrategy
	{
		string    name;
		ArrayList ruleSets = new ArrayList();
		Hashtable properties       = new Hashtable();
		string[]  extensions;
		HighlightColor   digitColor;

		Hashtable environmentColors = new Hashtable();
		HighlightRuleSet defaultRuleSet = null;
		
		public Hashtable Properties 
		{
			get 
			{
				return properties;
			}
		}
		
		public string Name
		{
			get 
			{
				return name;
			}
		}
		
		public string[] Extensions
		{
			set 
			{
				extensions = value;
			}
			get 
			{
				return extensions;
			}
		}
		
		public ArrayList RuleSets
		{
			get 
			{
				return ruleSets;
			}
		}
		
		public HighlightColor DigitColor 
		{
			get {
				return digitColor;
			}
			set {
				digitColor = value;
			}
		}
		
		
		public DefaultHighlightingStrategy() : this("Default")
		{
		}
		
		public DefaultHighlightingStrategy(string name) 
		{
			this.name = name;
			this.digitColor      = new HighlightColor("WindowText", "Window", false, false);
			
			// set default color environment'
			environmentColors["Default"]          = new HighlightColor(Color.Black, Color.White, false, false);
			environmentColors["Selection"]        = new HighlightColor(Color.Black, Color.FromArgb(((System.Byte)(102)), ((System.Byte)(210)), ((System.Byte)(214))), false, false);
			environmentColors["VRuler"]           = new HighlightColor("ControlLight", "Window", false, false);
			environmentColors["InvalidLines"]     = new HighlightColor(Color.Red, false, false);
			environmentColors["CaretMarker"]      = new HighlightColor(Color.Yellow, false, false);
			environmentColors["LineNumbers"]      = new HighlightColor("ControlDark", "Window", false, false);
			
			environmentColors["FoldLine"]         = new HighlightColor(Color.FromArgb(0x80, 0x80, 0x80), Color.Black, false, false);
			environmentColors["FoldMarker"]       = new HighlightColor(Color.FromArgb(0x80, 0x80, 0x80), Color.White, false, false);
			environmentColors["SelectedFoldLine"] = new HighlightColor(Color.Black, false, false);
			environmentColors["EOLMarkers"]       = new HighlightColor("ControlLight", "Window", false, false);
			environmentColors["SpaceMarkers"]     = new HighlightColor("ControlLight", "Window", false, false);
			environmentColors["TabMarkers"]       = new HighlightColor("ControlLight", "Window", false, false);
			
		}
		

		public HighlightRuleSet FindHighlightRuleSet(string name)
		{
			foreach(HighlightRuleSet ruleSet in ruleSets) {
				if (ruleSet.Name == name) {
					return ruleSet;
				}
			}
			return null;
		}
		
		public void AddRuleSet(HighlightRuleSet aRuleSet)
		{
			ruleSets.Add(aRuleSet);
		}
		
		internal void ResolveReferences()
		{
			// Resolve references from Span definitions to RuleSets
			ResolveRuleSetReferencesForSpan();
			// Resolve references from RuleSet defintitions to Highlighters defined in an external mode file
			ResolveHighlightingStrategyReferencesForRuleSet();
		}
		
		void ResolveRuleSetReferencesForSpan() 
		{
			foreach (HighlightRuleSet ruleSet in RuleSets) {
				if (ruleSet.Name == null) {
					defaultRuleSet = ruleSet;
				}
				
				foreach (Span aSpan in ruleSet.Spans) {
					if (aSpan.Rule != null) {//如果a当前Span引用的RuleSet不为空,则查找该引用.
						bool found = false;
						foreach (HighlightRuleSet refSet in RuleSets) {
							if (refSet.Name == aSpan.Rule) {
								found = true;
								aSpan.RuleSet = refSet;
								break;
							}
						}
						if (!found) {
							MessageBox.Show("The RuleSet " + aSpan.Rule + " could not be found in mode definition " + this.Name, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
							aSpan.RuleSet = null;
						}
					} else {
						aSpan.RuleSet = null;
					}
				}
			}
			
			if (defaultRuleSet == null) {
				MessageBox.Show("No default RuleSet is defined for mode definition " + this.Name, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
			}
		}
		
		void ResolveHighlightingStrategyReferencesForRuleSet() 
		{
			foreach (HighlightRuleSet ruleSet in RuleSets) {
				if (ruleSet.Reference != null) {//如果当前ruleSet引用的高亮度策略不为空.
					IHighlightingStrategy highlighter = HighlightingManager.Manager.FindHighlighterByName(ruleSet.Reference);
					
					if (highlighter != null) {
						ruleSet.Highlighter = highlighter;//设置当前ruleSet的高亮度策略.
					} else {
						MessageBox.Show("The mode defintion " + ruleSet.Reference + " which is refered from the " + this.Name + " mode definition could not be found", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
						ruleSet.Highlighter = this;
					}
				} else {//若当前ruleSet引用的高亮度策略为空,则把当前的策略给它.
					ruleSet.Highlighter = this;
				}
			}
		}
		
		
		internal void SetColorForEnvironment(string name, HighlightColor color)
		{
			environmentColors[name] = color;
		}

		public HighlightColor GetEnvironmentColorForName(string name)
		{
			if (environmentColors[name] == null) {
				throw new Exception("Color : " + name + " not found!");
			}
			return (HighlightColor)environmentColors[name];
		}
		
		
		public HighlightColor GetColor(IDocument document, LineSegment currentSegment, int currentOffset, int currentLength)
		{
			//如果不指定RuleSet,则用defaultRuleSet来作为规则集.
			return GetColor(defaultRuleSet, document, currentSegment, currentOffset, currentLength);
		}

		HighlightColor GetColor(HighlightRuleSet ruleSet, IDocument document, LineSegment currentSegment, int currentOffset, int currentLength)
		{
			if (ruleSet != null) {
				if (ruleSet.Reference != null) {
					return ruleSet.Highlighter.GetColor(document, currentSegment, currentOffset, currentLength);
				} else {
					return (HighlightColor)ruleSet.KeyWords[document,  currentSegment, currentOffset, currentLength];
				}				
			}
			return null;
		}
		
		public HighlightRuleSet GetRuleSetForSpan(Span aSpan)
		{
			if (aSpan == null) {
				return this.defaultRuleSet;
			} else {
				if (aSpan.RuleSet != null)
				{
					if (aSpan.RuleSet.Reference != null) {
						return aSpan.RuleSet.Highlighter.GetRuleSetForSpan(null);
					} else {
						return aSpan.RuleSet;
					}
				} else {
					return null;
				}
			}
		}

		
		
		LineSegment currentLine;
		bool inSpan;
		Span activeSpan;
		HighlightRuleSet activeRuleSet;
		int currentOffset;
		int currentLength;
		Stack currentSpanStack;
		// Mark整个文档.
		public void MarkTokens(IDocument document)
		{
			if (RuleSets.Count == 0) {//如果没有Rule Set则退出,即不给当前文档mark token
				return;
			}
			
			int lineNumber = 0;//从第一行开始mark token
			
			while (lineNumber < document.TotalNumberOfLines) {//一直循环到最后一行完成后结束,注:最后一行可能只有一个行结束符.
				LineSegment previousLine = (lineNumber > 0 ? document.GetLineSegment(lineNumber - 1) : null);
				if (lineNumber >= document.LineSegmentCollection.Count) { // may be, if the last line ends with a delimiter
					break;                                                // then the last line is not in the LineSegmentCollection :)
				}
				
				//当前行中的SpanStack总是获取前面一行中的SpanStack
				currentSpanStack = ((previousLine != null && previousLine.HighlightSpanStack != null) ? ((Stack)(previousLine.HighlightSpanStack.Clone())) : null);
				
				if (currentSpanStack != null) {
					while (currentSpanStack.Count > 0 && ((Span)currentSpanStack.Peek()).StopEOL)
					{
						currentSpanStack.Pop();
					}
					if (currentSpanStack.Count == 0) currentSpanStack = null;
				}
				
				currentLine = (LineSegment)document.LineSegmentCollection[lineNumber];
				
				if (currentLine.Length == -1) { // happens when buffer is empty !
					return;
				}
				
				ArrayList words = ParseLine(document);

				if (currentLine.Words!=null) currentLine.Words.Clear();
				currentLine.Words = words;
				currentLine.HighlightSpanStack = (currentSpanStack==null || currentSpanStack.Count==0) ? null : currentSpanStack;
				
				++lineNumber;
			}
			document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
			document.OnUpdateCommited();
		}
		
		// Mark document文档中的一行.
		bool MarkTokensInLine(IDocument document, int lineNumber, ref bool spanChanged)
		{
			bool processNextLine = false;
			LineSegment previousLine = (lineNumber > 0 ? document.GetLineSegment(lineNumber - 1) : null);
			
			currentSpanStack = ((previousLine != null && previousLine.HighlightSpanStack != null) ? ((Stack)(previousLine.HighlightSpanStack.Clone())) : null);
			if (currentSpanStack != null) {
				while (currentSpanStack.Count > 0 && ((Span)currentSpanStack.Peek()).StopEOL) {
					currentSpanStack.Pop();
				}
				if (currentSpanStack.Count == 0) {
					currentSpanStack = null;
				}
			}
			
			currentLine = (LineSegment)document.LineSegmentCollection[lineNumber];
			
			if (currentLine.Length == -1) { // happens when buffer is empty !
				return false;
			}
			
			ArrayList words = ParseLine(document);
			
			if (currentSpanStack != null && currentSpanStack.Count == 0) {
				currentSpanStack = null;
			}
			
			// Check if the span state has changed, if so we must re-render the next line
			// This check may seem utterly complicated but I didn't want to introduce any function calls
			// or alllocations here for perf reasons.
			if(currentLine.HighlightSpanStack != currentSpanStack) {
				if (currentLine.HighlightSpanStack == null) {
					processNextLine = false;
					foreach (Span sp in currentSpanStack) {
						if (!sp.StopEOL) {
							spanChanged = true;
							processNextLine = true;
							break;
						}
					}
				} else if (currentSpanStack == null) {
					processNextLine = false;
					foreach (Span sp in currentLine.HighlightSpanStack) {
						if (!sp.StopEOL) {
							spanChanged = true;
							processNextLine = true;
							break;
						}
					}
				} else {
					IEnumerator e1 = currentSpanStack.GetEnumerator();
					IEnumerator e2 = currentLine.HighlightSpanStack.GetEnumerator();
					bool done = false;
					while (!done) {
						bool blockSpanIn1 = false;
						while (e1.MoveNext()) {
							if (!((Span)e1.Current).StopEOL) {
								blockSpanIn1 = true;
								break;
							}
						}
						bool blockSpanIn2 = false;
						while (e2.MoveNext()) {
							if (!((Span)e2.Current).StopEOL) {
								blockSpanIn2 = true;
								break;
							}
						}
						if (blockSpanIn1 || blockSpanIn2) {
							if (blockSpanIn1 && blockSpanIn2) {
								if (e1.Current != e2.Current) {
									done = true;
									processNextLine = true;
									spanChanged = true;
								}											
							} else {
								spanChanged = true;
								done = true;
								processNextLine = true;
							}
						} else {
							done = true;
							processNextLine = false;
						}
					}
				}
			} else {
				processNextLine = false;
			}
			
//// Alex: remove old words
			if (currentLine.Words!=null) currentLine.Words.Clear();
			currentLine.Words = words;
			currentLine.HighlightSpanStack = (currentSpanStack != null && currentSpanStack.Count > 0) ? currentSpanStack : null;
			
			return processNextLine;
		}
		
		// Mark document文档中的某些行.
		public void MarkTokens(IDocument document, ArrayList Lines)
		{
			if (RuleSets.Count == 0) {
				return;
			}
			
			Hashtable processedLines = new Hashtable();
			
			bool spanChanged = false;
			
			foreach (LineSegment lineToProcess in Lines) {
				if (processedLines[lineToProcess] == null) {
					int lineNumber = document.GetLineNumberForOffset(lineToProcess.Offset);
					bool processNextLine = true;
					
					if (lineNumber != -1) {
						while (processNextLine && lineNumber < document.TotalNumberOfLines) {
							if (lineNumber >= document.LineSegmentCollection.Count) { // may be, if the last line ends with a delimiter
								break;                                                // then the last line is not in the collection :)
							}
							
							processNextLine = MarkTokensInLine(document, lineNumber, ref spanChanged);
 							processedLines[currentLine] = String.Empty;
							++lineNumber;
						}
					}
				} 
			}
			
			if (spanChanged) {
				document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
			} else {				
				foreach (LineSegment lineToProcess in Lines) {
					document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, document.GetLineNumberForOffset(lineToProcess.Offset)));
				}
				
			}
			document.OnUpdateCommited();
		}
		

		void UpdateSpanStateVariables() 
		{
			inSpan = (currentSpanStack != null && currentSpanStack.Count > 0);
			activeSpan = inSpan ? (Span)currentSpanStack.Peek() : null;
			activeRuleSet = GetRuleSetForSpan(activeSpan);
		}

		//根据doucment,currentLine为参数,来分析得到该行中的所有单词即words
		ArrayList ParseLine(IDocument document)
		{
			ArrayList words = new ArrayList();
			HighlightColor markNext = null;
			
			currentOffset = 0;
			currentLength = 0;
			UpdateSpanStateVariables();
			//从当前行的第一个字符开始循环,逐个分析.
			for (int i = 0; i < currentLine.Length; ++i) {
				char ch = document.GetCharAt(currentLine.Offset + i);
				switch (ch) {
					case '\n':
					case '\r':
						PushCurWord(document, ref markNext, words);
						++currentOffset;
						break;
					case ' ':
						PushCurWord(document, ref markNext, words);
						if (activeSpan != null && activeSpan.Color.HasBackground) {
							words.Add(new SpaceTextWord(activeSpan.Color));
						} else {
							words.Add(TextWord.Space);
						}
						++currentOffset;
						break;
					case '\t':
						PushCurWord(document, ref markNext, words);
						if (activeSpan != null && activeSpan.Color.HasBackground) {
							words.Add(new TabTextWord(activeSpan.Color));
						} else {
							words.Add(TextWord.Tab);
						}
						++currentOffset;
						break;
					case '\\': // handle escape chars
						if ((activeRuleSet != null && activeRuleSet.NoEscapeSequences) || 
						    (activeSpan != null && activeSpan.NoEscapeSequences)) {
							goto default;
						}
						++currentLength;
						if (i + 1 < currentLine.Length) {
							++currentLength;
						}
						PushCurWord(document, ref markNext, words);
						++i;
						continue;
					default: {
						// highlight digits
						if (!inSpan && (Char.IsDigit(ch) || (ch == '.' && i + 1 < currentLine.Length && Char.IsDigit(document.GetCharAt(currentLine.Offset + i + 1)))) && currentLength == 0) {
							bool ishex = false;
							bool isfloatingpoint = false;
							
							if (ch == '0' && i + 1 < currentLine.Length && Char.ToUpper(document.GetCharAt(currentLine.Offset + i + 1)) == 'X') { // hex digits
								const string hex = "0123456789ABCDEF";
								++currentLength;
								++i; // skip 'x'
								++currentLength; 
								ishex = true;
								while (i + 1 < currentLine.Length && hex.IndexOf(Char.ToUpper(document.GetCharAt(currentLine.Offset + i + 1))) != -1) {
									++i;
									++currentLength;
								}
							} else {
								++currentLength; 
								while (i + 1 < currentLine.Length && Char.IsDigit(document.GetCharAt(currentLine.Offset + i + 1))) {
									++i;
									++currentLength;
								}
							}
							if (!ishex && i + 1 < currentLine.Length && document.GetCharAt(currentLine.Offset + i + 1) == '.') {
								isfloatingpoint = true;
								++i;
								++currentLength;
								while (i + 1 < currentLine.Length && Char.IsDigit(document.GetCharAt(currentLine.Offset + i + 1))) {
									++i;
									++currentLength;
								}
							} 
								
							if (i + 1 < currentLine.Length && Char.ToUpper(document.GetCharAt(currentLine.Offset + i + 1)) == 'E') {
								isfloatingpoint = true;
								++i;
								++currentLength;
								if (i + 1 < currentLine.Length && (document.GetCharAt(currentLine.Offset + i + 1) == '+' || document.GetCharAt(currentLine.Offset + i + 1) == '-')) {
									++i;
									++currentLength;
								}
								while (i + 1 < currentLine.Length && Char.IsDigit(document.GetCharAt(currentLine.Offset + i + 1))) {
									++i;
									++currentLength;
								}
							}
							
							if (i + 1 < currentLine.Length) {
								char nextch = Char.ToUpper(document.GetCharAt(currentLine.Offset + i + 1));
								if (nextch == 'F' || nextch == 'M' || nextch == 'D') {
									isfloatingpoint = true;
									++i;
									++currentLength;
								}
							}
							
							if (!isfloatingpoint) {
								bool isunsigned = false;
								if (i + 1 < currentLine.Length && Char.ToUpper(document.GetCharAt(currentLine.Offset + i + 1)) == 'U') {
									++i;
									++currentLength;
									isunsigned = true;
								}
								if (i + 1 < currentLine.Length && Char.ToUpper(document.GetCharAt(currentLine.Offset + i + 1)) == 'L') {
									++i;
									++currentLength;
									if (!isunsigned && i + 1 < currentLine.Length && Char.ToUpper(document.GetCharAt(currentLine.Offset + i + 1)) == 'U') {
										++i;
										++currentLength;
									}
								}
							}
							
							words.Add(new TextWord(document, currentLine, currentOffset, currentLength, DigitColor, false));
							currentOffset += currentLength;
							currentLength = 0;
							continue;
						}

						// Check for SPAN ENDs
						if (inSpan) {
							if (activeSpan.End != null && !activeSpan.End.Equals("")) {
								if (currentLine.MatchExpr(activeSpan.End, i, document)) {
									PushCurWord(document, ref markNext, words);
									string regex = currentLine.GetRegString(activeSpan.End, i, document);
									currentLength += regex.Length;
									words.Add(new TextWord(document, currentLine, currentOffset, currentLength, activeSpan.EndColor, false));
									currentOffset += currentLength;
									currentLength = 0;
									i += regex.Length - 1;
									currentSpanStack.Pop();
									UpdateSpanStateVariables();
									continue;
								}
							}
						}
						
						// check for SPAN BEGIN
						if (activeRuleSet != null) {
							foreach (Span span in activeRuleSet.Spans) {
								if (currentLine.MatchExpr(span.Begin, i, document)) {
									PushCurWord(document, ref markNext, words);
									string regex = currentLine.GetRegString(span.Begin, i, document);
									currentLength += regex.Length;
									words.Add(new TextWord(document, currentLine, currentOffset, currentLength, span.BeginColor, false));
									currentOffset += currentLength;
									currentLength = 0;
									
									i += regex.Length - 1;
									if( currentSpanStack == null) currentSpanStack = new Stack();
									currentSpanStack.Push(span);
									
									UpdateSpanStateVariables();
									
									goto skip;
								}
							}
						}
						
						// check if the char is a delimiter
						if (activeRuleSet != null && (int)ch < 256 && activeRuleSet.Delimiters[(int)ch]) {
							PushCurWord(document, ref markNext, words);
							if (currentOffset + currentLength +1 < currentLine.Length) {
								++currentLength;
								PushCurWord(document, ref markNext, words);
								goto skip;
							}
						}
						
						++currentLength;
						skip: continue;
					}
				}
			}
			
			PushCurWord(document, ref markNext, words);			
			
			return words;
		}		
		
		void PushCurWord(IDocument document, ref HighlightColor markNext, ArrayList words)
		{
			//Need to look through the next prev logic.
			if (currentLength > 0) {
				if (words.Count > 0 && activeRuleSet != null) {
					TextWord prevWord = null;
					int pInd = words.Count - 1;
					while (pInd >= 0) {
						if (!((TextWord)words[pInd]).IsWhiteSpace) {
							prevWord = (TextWord)words[pInd];
							if (prevWord.HasDefaultColor) {
								PrevMarker marker = (PrevMarker)activeRuleSet.PrevMarkers[document, currentLine, currentOffset, currentLength];
								if (marker != null) {
									prevWord.HighlightColor = marker.Color;
								}
							}
							break;
						}
						pInd--;
					}
				}
				
				if (inSpan) {
					HighlightColor c = null;
					bool hasDefaultColor = true;
					if (activeSpan.Rule == null) {
						c = activeSpan.Color;
					} else {
						c = GetColor(activeRuleSet, document, currentLine, currentOffset, currentLength);
						hasDefaultColor = false;
					}
					
					if (c == null) {
						c = activeSpan.Color;
						if (c.Color == Color.Transparent) {
							c = GetEnvironmentColorForName("Default");
						}
						hasDefaultColor = true;
					}
					words.Add(new TextWord(document, currentLine, currentOffset, currentLength, markNext != null ? markNext : c, hasDefaultColor));
				} else {
					HighlightColor c = markNext != null ? markNext : GetColor(activeRuleSet, document, currentLine, currentOffset, currentLength);
					if (c == null) {
						words.Add(new TextWord(document, currentLine, currentOffset, currentLength, GetEnvironmentColorForName("Default"), true));
					} else {
						words.Add(new TextWord(document, currentLine, currentOffset, currentLength, c, false));
					}
				}
				
				if (activeRuleSet != null) {
					NextMarker nextMarker = (NextMarker)activeRuleSet.NextMarkers[document, currentLine, currentOffset, currentLength];
					if (nextMarker != null) {
						if (nextMarker.MarkMarker && words.Count > 0) {
							TextWord prevword = ((TextWord)words[words.Count - 1]);
							prevword.HighlightColor = nextMarker.Color;
						}
						markNext = nextMarker.Color;
					} else {
						markNext = null;
					}
				}
				currentOffset += currentLength;
				currentLength = 0;					
			}
		}		
	}
}
