

using System;
using System.IO;
using System.Diagnostics;
using NetFocus.DataStructure.TextEditor.Undo;

namespace NetFocus.DataStructure.TextEditor.Document
{
	/// <summary>
	/// Simple implementation of the ITextBuffer interface implemented using a
	/// string.
	/// Only for fall-back purposes.
	/// </summary>
	public class StringTextBufferStrategy : ITextBufferStrategy
	{
		string storedText = "";
		
		public int Length {
			get {
				return storedText.Length;
			}
		}
		
		public void Insert(int offset, string text)
		{
			if (text != null) {
				storedText = storedText.Insert(offset, text);
			}
		}
		
		public void Remove(int offset, int length)
		{
			storedText = storedText.Remove(offset, length);
		}
		
		public void Replace(int offset, int length, string text)
		{
			Remove(offset, length);
			Insert(offset, text);
		}
		
		public string GetText(int offset, int length)
		{
			if (length == 0) {
				return "";
			}
			return storedText.Substring(offset, Math.Min(length, storedText.Length - offset));
		}
		
		public char GetCharAt(int offset)
		{
			if (offset == Length) {
				return '\0';
			}
			return storedText[offset];
		}
		
		public void SetContent(string text)
		{
			storedText = text;
		}
		
		public StringTextBufferStrategy()
		{
		}
		
		StringTextBufferStrategy(string fileName)
		{
			StreamReader streamReader = File.OpenText(fileName);			
			SetContent(streamReader.ReadToEnd());
			streamReader.Close();
		}
		
		public static ITextBufferStrategy CreateTextBufferFromFile(string fileName)
		{
			if (!File.Exists(fileName)) {
				throw new System.IO.FileNotFoundException(fileName);
			}
			return new StringTextBufferStrategy(fileName);
		}
	}
}
