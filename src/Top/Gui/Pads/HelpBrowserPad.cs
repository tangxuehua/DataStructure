

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.IO;
using System.Diagnostics;
using System.Xml;

using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Gui.Views;


namespace NetFocus.DataStructure.Gui.Pads
{

	public class HelpBrowserPad : AbstractPadContent
	{
		static readonly string helpPath = Application.StartupPath + Path.DirectorySeparatorChar + System.Configuration.ConfigurationSettings.AppSettings["HelpPath"].ToString();
		
		static readonly string helpFileConvertFileName = helpPath + "HelpConv.xml";
		
		Panel     browserPanel = new Panel();
		TreeView  treeView     = new TreeView();
		
		HtmlView htmlView = null;
		
		public override Control Control {
			get {
				return browserPanel;
			}
		}
		
		void browserPanel_Paint(object sender,PaintEventArgs e)
		{
			
			e.Graphics.DrawRectangle(new Pen(Color.Gray,1),e.ClipRectangle.X,e.ClipRectangle.Y,e.ClipRectangle.Width - 1,e.ClipRectangle.Height - 1);
		}
		void topPanel_Resize(object sender,EventArgs e)
		{
			browserPanel.Invalidate();
		}
		
		public HelpBrowserPad() : base("${res:MainWindow.Windows.HelpScoutLabel}", "Icons.16x16.HelpIcon")
		{
			
			treeView.Dock = DockStyle.Fill;
			treeView.BorderStyle = BorderStyle.None;
			treeView.ImageList = new ImageList();
			ResourceService ResourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			
			treeView.ImageList.Images.Add(ResourceService.GetBitmap("Icons.16x16.HelpClosedFolder"));
			treeView.ImageList.Images.Add(ResourceService.GetBitmap("Icons.16x16.HelpOpenFolder"));
			
			treeView.ImageList.Images.Add(ResourceService.GetBitmap("Icons.16x16.HelpTopic"));
			treeView.BeforeExpand   += new TreeViewCancelEventHandler(BeforeExpand);			
			treeView.BeforeCollapse += new TreeViewCancelEventHandler(BeforeCollapse);
			treeView.DoubleClick += new EventHandler(DoubleClick);
			browserPanel.DockPadding.All = 2;
			browserPanel.Controls.Add(treeView);

			browserPanel.Paint +=new PaintEventHandler(browserPanel_Paint);
			
			LoadHelpfile();
		}
		

		void ParseTree(TreeNodeCollection nodeCollection, XmlNode parentNode)//递归分析XML文件中的元素.
		{
			foreach (XmlNode node in parentNode.ChildNodes) {
				switch (node.Name) {
					case "HelpFolder":
						TreeNode newFolderNode = new TreeNode(node.Attributes["name"].InnerText);
						newFolderNode.ImageIndex = newFolderNode.SelectedImageIndex = 0;
						ParseTree(newFolderNode.Nodes, node);
						nodeCollection.Add(newFolderNode);
						break;
					case "HelpTopic":
						TreeNode newNode = new TreeNode(node.Attributes["name"].InnerText);
						newNode.ImageIndex = newNode.SelectedImageIndex = 2;
						newNode.Tag = node.Attributes["link"].InnerText;
						nodeCollection.Add(newNode);
						break;
				}
			}
		}
		
		void LoadHelpfile()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(helpFileConvertFileName);
			ParseTree(treeView.Nodes, doc.DocumentElement);
		}
		
		void HelpBrowserWindowClose(object sender, EventArgs e)
		{
			htmlView = null;
		}
		
		void DoubleClick(object sender, EventArgs e)
		{
			TreeNode node = treeView.SelectedNode;
			if (node.Tag != null) {
				string navigationName = "mk:@MSITStore:" + helpPath + node.Tag.ToString();
				if (htmlView == null) {
					htmlView = new HtmlView();
					WorkbenchSingleton.Workbench.ShowView(htmlView);
					htmlView.CloseEvent += new EventHandler(HelpBrowserWindowClose);
				}
				htmlView.LoadFile(navigationName);
			}
		}
		
		void BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Node.ImageIndex < 2) {
				e.Node.ImageIndex = e.Node.SelectedImageIndex = 1;
			}
		}
		
		void BeforeCollapse(object sender, TreeViewCancelEventArgs e)
		{ 
			if (e.Node.ImageIndex < 2) {
				e.Node.ImageIndex = e.Node.SelectedImageIndex = 0;
			}
		}
	}
}
