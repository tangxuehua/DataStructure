using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Resources;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace NetFocus.DataStructure.Services
{
	/// <summary>
	/// 一个为应用程序报告消息的服务
	/// </summary>
	public class MessageService : AbstractService, IMessageService
	{
		StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
		
		public void ShowError(Exception ex)
		{
			ShowError(ex, null);
		}
		
		public void ShowError(string message)
		{
			ShowError(null, message);
		}
		
		public void ShowErrorFormatted(string formatstring, params string[] formatitems)
		{
			ShowError(null, String.Format(stringParserService.Parse(formatstring), formatitems));
		}
		
		public void ShowError(Exception ex, string message)
		{
			string msg = String.Empty;
			
			if (message != null) {
				msg += message;
			}
			
			if (message != null && ex != null) {
				msg += "\n\n";
			}
			
			if (ex != null) {
				msg += "Exception occurred: " + ex.ToString();
			}
				
			MessageBox.Show(stringParserService.Parse(msg), stringParserService.Parse("${res:Global.ErrorText}"), MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		
		
		public void ShowWarning(string message)
		{
			MessageBox.Show(stringParserService.Parse(message), stringParserService.Parse("${res:Global.WarningText}"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
		
		public void ShowWarningFormatted(string formatstring, params string[] formatitems)
		{
			ShowWarning(String.Format(stringParserService.Parse(formatstring), formatitems));
		}
		
		
		public bool AskQuestion(string question, string caption)
		{
			return MessageBox.Show(stringParserService.Parse(question), stringParserService.Parse(caption), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
		}
		
		public bool AskQuestionFormatted(string caption, string formatstring, params string[] formatitems)
		{
			return AskQuestion(String.Format(stringParserService.Parse(formatstring), formatitems), caption);
		}
		
		public bool AskQuestionFormatted(string formatstring, params string[] formatitems)
		{
			return AskQuestion(String.Format(stringParserService.Parse(formatstring), formatitems));
		}
		
		public bool AskQuestion(string question)
		{
			Console.WriteLine("stps: " + stringParserService);
			
			return AskQuestion(stringParserService.Parse(question), stringParserService.Parse("${res:Global.QuestionText}"));
		}
		
		
		public void ShowMessage(string message)
		{
			ShowMessage(message, "DataStructure");
		}
		
		public void ShowMessageFormatted(string formatstring, params string[] formatitems)
		{
			ShowMessage(String.Format(stringParserService.Parse(formatstring), formatitems));
		}
		
		public void ShowMessageFormatted(string caption, string formatstring, params string[] formatitems)
		{
			ShowMessage(String.Format(stringParserService.Parse(formatstring), formatitems), caption);
		}
		
		public void ShowMessage(string message, string caption)
		{
			MessageBox.Show(stringParserService.Parse(message), stringParserService.Parse(caption), MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		
		public int ShowCustomDialog(string caption, string dialogText, params string[] buttontexts)
		{
			// TODO
			return 0;
		}
		

	}
}
