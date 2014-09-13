
using System;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using System.Text;
using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.Gui.Dialogs;
using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Nodes;


namespace NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Panels
{
	class KeywordListOptionPanel : NodeOptionPanel
	{
		private System.Windows.Forms.Button chgBtn;
		private System.Windows.Forms.Button addBtn;
		private System.Windows.Forms.ListBox listBox;
		private System.Windows.Forms.Button removeBtn;
		private System.Windows.Forms.TextBox nameBox;
		private System.Windows.Forms.Label sampleLabel;

		public KeywordListOptionPanel(KeywordListNode parent) : base(parent)
		{
			SetupFromXmlFile(System.IO.Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\HighlightingEditor\KeywordList.xfrm"));
			
			addBtn = (Button)ControlDictionary["addBtn"];
			addBtn.Click += new EventHandler(addBtnClick);
			removeBtn = (Button)ControlDictionary["removeBtn"];
			removeBtn.Click += new EventHandler(removeBtnClick);
			chgBtn = (Button)ControlDictionary["chgBtn"];
			chgBtn.Click += new EventHandler(chgBtnClick);
			
			nameBox = (TextBox)ControlDictionary["nameBox"];
			sampleLabel = (Label)ControlDictionary["sampleLabel"];
			listBox  = (ListBox)ControlDictionary["listBox"];
		}
		
		EditorHighlightColor color;
		
		public override void StoreSettings()
		{
			KeywordListNode node = (KeywordListNode)parentNode;
			StringCollection col = new StringCollection();
			
			foreach (string word in listBox.Items) {
				col.Add(word);
			}
			node.Words = col;
			node.Name = nameBox.Text;
			node.HighlightColor = color;
		}
		
		public override void LoadSettings()
		{
			KeywordListNode node = (KeywordListNode)parentNode;
			listBox.Items.Clear();
			
			foreach (string word in node.Words) {
				listBox.Items.Add(word);
			}
			
			IProperties properties = ((IProperties)PropertyService.GetProperty("NetFocus.DataStructure.TextEditor.Document.DefaultDocumentProperties", new DefaultProperties()));
			sampleLabel.Font = ParseFont(properties.GetProperty("DefaultFont", new Font("Courier New", 10).ToString()));

			color = node.HighlightColor;
			nameBox.Text = node.Name;
			PreviewUpdate(sampleLabel, color);
		}
		
		public override bool ValidateSettings()
		{
			if (nameBox.Text == "") {
				ValidationMessage("${res:Dialog.HighlightingEditor.KeywordList.NameEmpty}");
				return false;
			}
			
			return true;
		}
		
		void chgBtnClick(object sender, EventArgs e)
		{
			using (EditHighlightingColorDialog dlg = new EditHighlightingColorDialog(color)) {
				if (dlg.ShowDialog(this) == DialogResult.OK) {
					color = dlg.Color;
					PreviewUpdate(sampleLabel, color);
				}
			}
		}
		
		void addBtnClick(object sender, EventArgs e)
		{
			using (InputBox box = new InputBox()) {
				box.Label.Text = ResourceService.GetString("Dialog.HighlightingEditor.KeywordList.EnterName");
				if (box.ShowDialog() == DialogResult.Cancel) return;
				
				if (box.TextBox.Text == "") return;
				foreach (string item in listBox.Items) {
					if (item == box.TextBox.Text)
						return;
				}
				
				listBox.Items.Add(box.TextBox.Text);
			}
		}
		
		void removeBtnClick(object sender, EventArgs e)
		{
			if (listBox.SelectedIndex == -1) return;
			
			listBox.Items.RemoveAt(listBox.SelectedIndex);
		}
	}
}
