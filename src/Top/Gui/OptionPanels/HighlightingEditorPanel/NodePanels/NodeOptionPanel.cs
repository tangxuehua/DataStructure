
using System;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;

using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.Gui.XmlForms;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Nodes;

namespace NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Panels
{
	public abstract class NodeOptionPanel : BaseXmlUserControl
	{
		//此节点表示当点击该节点并显示当前面板的那个treeNode节点.
		//eg.当单击DigitsNode节点,则会显示DigitsOptionPanel面板,那么DigitsNode节点就是DigitsOptionPanel面板的ParentNode,即父节点.
		protected AbstractNode parentNode;
		
		public AbstractNode ParentNode {
			get {
				return parentNode;
			}
		}
		
		
		public NodeOptionPanel(AbstractNode parentNode) {
			this.parentNode = parentNode;
			this.Dock = DockStyle.Fill;
			this.ClientSize = new Size(320, 392);
		}
		
		
		public virtual bool ValidateSettings()
		{
			return true;
		}
		
		
		protected void ValidationMessage(string str)
		{
			MessageService.ShowWarning("${res:Dialog.HighlightingEditor.ValidationError}\n\n" + str);
		}

		
		protected static Font ParseFont(string font)
		{
			string[] descr = font.Split(new char[]{',', '='});
			return new Font(descr[1], Single.Parse(descr[3]));
		}
			
		//更新预览标签.
		protected static void PreviewUpdate(Label label, EditorHighlightColor color)
		{
			if (label == null) return;
			
			if (color == null) {
				label.ForeColor = label.BackColor = Color.Transparent;
				return;
			}
			if (color.NoColor) {
				label.ForeColor = label.BackColor = Color.Transparent;
				return;
			}
			
			label.ForeColor = color.GetForeColor();
			label.BackColor = color.GetBackColor();
			
			FontStyle fs = FontStyle.Regular;
			if (color.Bold)   fs |= FontStyle.Bold;
			if (color.Italic) fs |= FontStyle.Italic;
			
			label.Font = new Font(label.Font, fs);
		}
		
		
		public abstract void StoreSettings();
		
		public abstract void LoadSettings();
	}
}
