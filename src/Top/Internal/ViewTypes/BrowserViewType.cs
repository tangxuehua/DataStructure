
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;

using NetFocus.DataStructure.Properties;
using NetFocus.Components.AddIns.Codons;
using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.Gui.Views;

namespace NetFocus.DataStructure.ViewTypes
{
	public class BrowserViewType : IViewType
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return fileName.StartsWith("http") || fileName.StartsWith("ftp");
		}
		
		public bool CanCreateContentForLanguage(string language)
		{
			return false;
		}
		
		public IViewContent CreateContentForFile(string fileName)
		{
			HtmlView htmlView = new HtmlView();
			
			htmlView.LoadFile(fileName);

			return htmlView;
		}
		
		public IViewContent CreateContentForLanguage(string language, string content)
		{
			return null;
		}		
	}
}
