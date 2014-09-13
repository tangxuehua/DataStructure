
using System;
using System.Collections;
using System.Text;
using System.Diagnostics;

using NetFocus.DataStructure.TextEditor.Undo;


namespace NetFocus.DataStructure.TextEditor.Document
{
	//一个终极类,即一个不能被继承的类.
	public sealed class TextUtilities
	{
		//为文本中的一行在其前面追加一些tab.
		public static string LeadingWhiteSpaceToTabs(string line, int tabIndent) {
			
			StringBuilder sb = new StringBuilder(line.Length);
			//定义一个表示连续的空格数的整数.
			int consecutiveSpaces = 0;
			int i = 0;
			for(i = 0; i < line.Length; i++) {
				if(line[i] == ' ') {
					consecutiveSpaces++;
					if(consecutiveSpaces == tabIndent) {
						sb.Append('\t');
						consecutiveSpaces = 0;
					}
				}
				else if(line[i] == '\t') {
					sb.Append('\t');
					// if we had say 3 spaces then a tab and tabIndent was 4 then
					// we would want to simply replace all of that with 1 tab
					consecutiveSpaces = 0;					
				}
				else {//如果出现一般的字符,则退出循环.
					break;
				}
			}
			
			if(i < line.Length) {
				sb.Append(line.Substring(i-consecutiveSpaces));//将后面的实际字符串全部加到前面的空格之后.
			}
			return sb.ToString();
		}
		
		
		public static bool IsLetterDigitOrUnderscore(char c)//这里underscore就是underline.
		{
			if(!Char.IsLetterOrDigit(c)) {//好啊,写得精练.
				return c == '_';
			}
			return true;
		}
		
		
		public enum CharacterType {
			LetterDigitOrUnderscore, //字母,数字,下划线
			WhiteSpace, //空格
			Other //其它字符
		}
		
		/// <remarks>
		/// This method returns the expression before a specified offset.
		/// That method is used in code completion to determine the expression given
		/// to the parser for type resolve.
		/// </remarks>
		public static string GetExpressionBeforeOffset(TextArea textArea, int initialOffset)
		{
			IDocument document = textArea.Document;
			int offset = initialOffset;
			while (offset - 1 > 0) {
				switch (document.GetCharAt(offset - 1)) {
					case '\n':
					case '\r':
					case '}':
						goto done;
//						offset = SearchBracketBackward(document, offset - 2, '{','}');
//						break;
					case ']':
						offset = SearchBracketBackward(document, offset - 2, '[',']');
						break;
					case ')':
						offset = SearchBracketBackward(document, offset - 2, '(',')');
						break;
					case '.':
						--offset;
						break;
					case '"':
						if (offset < initialOffset - 1) {
							return null;
						}
						return "\"\"";
					case '\'':
						if (offset < initialOffset - 1) {
							return null;
						}
						return "'a'";
					case '>':
						if (document.GetCharAt(offset - 2) == '-') {
							offset -= 2;
							break;
						}
						goto done;
					default:
						if (Char.IsWhiteSpace(document.GetCharAt(offset - 1))) {
							--offset;
							break;
						}
						int start = offset - 1;
						if (!IsLetterDigitOrUnderscore(document.GetCharAt(start))) {
							goto done;
						}
						
						while (start > 0 && IsLetterDigitOrUnderscore(document.GetCharAt(start - 1))) {
							--start;
						}
						string word = document.GetText(start, offset - start).Trim();
						switch (word) {
							case "ref":
							case "out":
							case "in":
							case "return":
							case "throw":
							case "case":
								goto done;
						}
						
						if (word.Length > 0 && !IsLetterDigitOrUnderscore(word[0])) {
							goto done;
						}
						offset = start;
						break;
				}
			}
			done:
//			Console.WriteLine("ofs : {0} cart:{1}", offset, document.Caret.Offset);
//			Console.WriteLine("return:" + document.GetText(offset, document.Caret.Offset - offset).Trim());
			//// simple exit fails when : is inside comment line or any other character
			//// we have to check if we got several ids in resulting line, which usually happens when
			//// id. is typed on next line after comment one
			//// Would be better if lexer would parse properly such expressions. However this will cause
			//// modifications in this area too - to get full comment line and remove it afterwards
			string resText=document.GetText(offset, textArea.Caret.Offset - offset ).Trim();
			int pos=resText.LastIndexOf('\n');
			if (pos>=0) {
				offset+=pos+1;
				//// whitespaces and tabs, which might be inside, will be skipped by trim below
			}							
			string expression = document.GetText(offset, textArea.Caret.Offset - offset ).Trim();
			Console.WriteLine("Expr: >" + expression + "<");
			return expression;
		}
		
		
		public static CharacterType GetCharacterType(char c) 
		{
			if(IsLetterDigitOrUnderscore(c))
				return CharacterType.LetterDigitOrUnderscore;
			if(Char.IsWhiteSpace(c))
				return CharacterType.WhiteSpace;
			return CharacterType.Other;
		}
		
		
		//得到第一个非空格字符的位置.
		public static int GetFirstNonWSChar(IDocument document, int offset)
		{
			while (offset < document.TextLength && Char.IsWhiteSpace(document.GetCharAt(offset))) {
				++offset;
			}
			return offset;
		}
		
		
		//得到当前字的结束位置.
		public static int FindWordEnd(IDocument document, int offset)
		{
			LineSegment line   = document.GetLineSegmentForOffset(offset);
			int     endPos = line.Offset + line.Length;
			while (offset < endPos && IsLetterDigitOrUnderscore(document.GetCharAt(offset))) {
				++offset;
			}
			
			return offset;
		}
		
		
		public static int FindWordStart(IDocument document, int offset)
		{
			LineSegment line = document.GetLineSegmentForOffset(offset);
			
			while (offset > line.Offset && !IsLetterDigitOrUnderscore(document.GetCharAt(offset - 1))) {
				--offset;
			}
			
			return offset;
		}
		
		
		//得到下一个字的其实位置.
		public static int FindNextWordStart(IDocument document, int offset)
		{
			int originalOffset = offset;
			LineSegment line   = document.GetLineSegmentForOffset(offset);
			int     endPos = line.Offset + line.Length;
			// lets go to the end of the word, whitespace or operator
			CharacterType t = GetCharacterType(document.GetCharAt(offset));
			while (offset < endPos && GetCharacterType(document.GetCharAt(offset)) == t) {
				++offset;
			}
			
			// now we're at the end of the word, lets find the start of the next one by skipping whitespace
			while (offset < endPos && GetCharacterType(document.GetCharAt(offset)) == CharacterType.WhiteSpace) {
				++offset;
			}

			return offset;
		}
		

		public static int FindPrevWordStart(IDocument document, int offset)
		{
			int originalOffset = offset;
			LineSegment line = document.GetLineSegmentForOffset(offset);
			if (offset > 0) {
				CharacterType t = GetCharacterType(document.GetCharAt(offset - 1));
				while (offset > line.Offset && GetCharacterType(document.GetCharAt(offset - 1)) == t) {
					--offset;
				}
				
				// if we were in whitespace, and now we're at the end of a word or operator, go back to the beginning of it
				if(t == CharacterType.WhiteSpace && offset > line.Offset) {
					t = GetCharacterType(document.GetCharAt(offset - 1));
					while (offset > line.Offset && GetCharacterType(document.GetCharAt(offset - 1)) == t) {
						--offset;
					}
				}
			}
			
			return offset;
		}
		
		
		public static string GetLineAsString(IDocument document, int lineNumber)
		{
			LineSegment line = document.GetLineSegment(lineNumber);
			return document.GetText(line.Offset, line.Length);
		}
		

		public static int SearchBracketBackward(IDocument document, int offset, char openBracket, char closingBracket)
		{
			return document.FormattingStrategy.SearchBracketBackward(document, offset, openBracket, closingBracket);
		}
		
		public static int SearchBracketForward(IDocument document, int offset, char openBracket, char closingBracket)
		{
			return document.FormattingStrategy.SearchBracketForward(document, offset, openBracket, closingBracket);
		}
		
		/// <remarks>
		/// Returns true, if the line lineNumber is empty or filled with whitespaces.
		/// </remarks>
		public static bool IsEmptyLine(IDocument document, int lineNumber)
		{
			return IsEmptyLine(document, document.GetLineSegment(lineNumber));
		}

		/// <remarks>
		/// Returns true, if the line lineNumber is empty or filled with whitespaces.
		/// </remarks>
		public static bool IsEmptyLine(IDocument document, LineSegment line)
		{
			for (int i = line.Offset; i < line.Offset + line.Length; ++i) {
				char ch = document.GetCharAt(i);
				if (!Char.IsWhiteSpace(ch)) {
					return false;
				}
			}
			return true;
		}
		
		
		static bool IsWordPart(char ch)
		{
			return IsLetterDigitOrUnderscore(ch) || ch == '.';
		}
		
		
		public static string GetWordAt(IDocument document, int offset)
		{
			if (offset < 0 || offset >= document.TextLength - 1 || !IsWordPart(document.GetCharAt(offset))) {
				return String.Empty;
			}
			int startOffset = offset;
			int endOffset   = offset;
			while (startOffset > 0 && IsWordPart(document.GetCharAt(startOffset - 1))) {
				--startOffset;//得到当前字的起始位置.
			}
			
			while (endOffset < document.TextLength - 1 && IsWordPart(document.GetCharAt(endOffset + 1))) {
				++endOffset;//得到当前字的结束位置.
			}
			
			return document.GetText(startOffset, endOffset - startOffset + 1);
		}
	}
}
