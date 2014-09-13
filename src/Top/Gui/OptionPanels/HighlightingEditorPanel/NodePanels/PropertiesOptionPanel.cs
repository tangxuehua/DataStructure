
using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Nodes;
using NetFocus.DataStructure.Gui.Dialogs;

namespace NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Panels
{
	class PropertiesOptionPanel : NodeOptionPanel
	{
		private System.Windows.Forms.Button addBtn;
		private System.Windows.Forms.Button editBtn;
		private System.Windows.Forms.Button removeBtn;
		private System.Windows.Forms.ListView listView;	
		
		public PropertiesOptionPanel(PropertiesNode parent) : base(parent)
		{
			SetupFromXmlFile(System.IO.Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\HighlightingEditor\Properties.xfrm"));
			
			addBtn = (Button)ControlDictionary["addBtn"];
			addBtn.Click += new EventHandler(addClick);
			editBtn = (Button)ControlDictionary["editBtn"];
			editBtn.Click += new EventHandler(editClick);
			removeBtn = (Button)ControlDictionary["removeBtn"];
			removeBtn.Click += new EventHandler(removeClick);
			
			listView  = (ListView)ControlDictionary["listView"];
		}
	
		public override void StoreSettings()
		{
			PropertiesNode node = (PropertiesNode)parentNode;
			
			node.Properties.Clear();
			foreach (ListViewItem item in listView.Items) {
				node.Properties.Add(item.Text, item.SubItems[1].Text);
			}
		}
		
		public override void LoadSettings()
		{
			PropertiesNode node = (PropertiesNode)parentNode;
			listView.Items.Clear();
			
			foreach (DictionaryEntry de in node.Properties) {
				ListViewItem lv = new ListViewItem(new string[] {(string)de.Key, (string)de.Value});
				listView.Items.Add(lv);
			}
		}
		
		void addClick(object sender, EventArgs e)
		{
			using (InputBox box = new InputBox()) {
				box.Label.Text = ResourceService.GetString("Dialog.HighlightingEditor.Properties.EnterName");
				if (box.ShowDialog() == DialogResult.Cancel) return;
				
				foreach (ListViewItem item in listView.Items) {
					if (item.Text == box.TextBox.Text)
						return;
				}
				
				listView.Items.Add(new ListViewItem(new string[] {box.TextBox.Text, ""}));
			}
		}
		
		void removeClick(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count != 1) return;
			
			listView.SelectedItems[0].Remove();
		}
		
		void editClick(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count != 1) return;

			using (InputBox box = new InputBox()) {
				box.Text = ResourceService.GetString("Dialog.HighlightingEditor.EnterText");
				box.Label.Text = String.Format(ResourceService.GetString("Dialog.HighlightingEditor.Properties.EnterValue"), listView.SelectedItems[0].Text);
				if (box.ShowDialog() == DialogResult.Cancel) return;
				
				listView.SelectedItems[0].SubItems[1].Text = box.TextBox.Text;
			}
		}
	}
}
