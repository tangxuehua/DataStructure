
using System;
using System.IO;
using System.Threading;


namespace NetFocus.DataStructure.TextEditor.Document
{
	public class DocumentFactory
	{
		/// <remarks>
		/// Creates a new <see cref="IDocument"/> object.
		/// </remarks>
		public IDocument CreateDefaultDocument()
		{
			DefaultDocument doc = new DefaultDocument();
			doc.TextBufferStrategy    = new GapTextBufferStrategy();
			//doc.TextBufferStrategy    = new StringTextBufferStrategy();
			doc.FormattingStrategy    = new DefaultFormattingStrategy();
			doc.LineManager          = new DefaultLineManager(doc, null);//因为这里是创建默认的文档,所以还不知道要什么样的高亮度显示策略.所以这里用null作为参数.
			doc.FoldingManager        = new FoldingManager(doc, doc.LineManager);
			doc.FoldingManager.FoldingStrategy       = new ParserFoldingStrategy();
			doc.TextMarkerStrategy       = new TextMarkerStrategy(doc);
			doc.BookmarkManager      = new BookmarkManager(doc.LineManager);
			return doc;
		}
		
		/// <summary>
		/// Creates a new document from the given file
		/// </summary>
		public IDocument CreateFromFile(string fileName)
		{
			//先创建一个默认的文档,然后读取文件的内容到该文档对象中,最后返回该文档对象.
			IDocument document = CreateDefaultDocument();
			StreamReader stream = new StreamReader(fileName, System.Text.Encoding.GetEncoding(Thread.CurrentThread.CurrentCulture.TextInfo.ANSICodePage));
			document.TextContent = stream.ReadToEnd();
			stream.Close();
			return document;
		}
	}
}
