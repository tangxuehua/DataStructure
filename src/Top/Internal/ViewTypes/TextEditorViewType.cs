
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;

using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.Gui.Pads;
using NetFocus.DataStructure.TextEditor;
using NetFocus.DataStructure.TextEditor.Undo;
using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.Properties;
using NetFocus.Components.AddIns;
using NetFocus.DataStructure.Services;
using NetFocus.Components.AddIns.Codons;
using NetFocus.DataStructure.Gui.Views;


namespace NetFocus.DataStructure.ViewTypes
{
	/// <summary>
	/// 用于绑定一个文本编辑器.
	/// </summary>
	public class TextEditorViewType : IViewType
	{
		static TextEditorViewType()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			if(propertyService != null)
			{
				//实例化一个高亮度显示语法的对象.(该对象包含各种文件的高亮度显示策略)
				SyntaxModeProvider syntaxModeProvider = new SyntaxModeProvider(Path.Combine(propertyService.DataDirectory,"modes"));

				HighlightingManager.Manager.AddSyntaxModeProvider(syntaxModeProvider);//添加一个语法醒目显示提供者.
			}
		}
		
		
		public virtual bool CanCreateContentForFile(string fileName)
		{
			return true;//因为是文本编辑器,所以一直返回真.
		}
		
		
		public virtual bool CanCreateContentForLanguage(string language)
		{
			return true;//因为是文本编辑器,所以一直返回真.
		}
		

		public virtual IViewContent CreateContentForFile(string fileName)
		{

			TextEditorView t = new TextEditorView();
			
			t.LoadFile(fileName);

			return t;
		}
		

		public virtual IViewContent CreateContentForLanguage(string language, string content)
		{
			TextEditorView t = new TextEditorView();
			
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			((TextEditorControl)t.Control).Document.TextContent = stringParserService.Parse(content);
			((TextEditorControl)t.Control).Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighterByName(language);

			return t;
		}		


	}
	
}
