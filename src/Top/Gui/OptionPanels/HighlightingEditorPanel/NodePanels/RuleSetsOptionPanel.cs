
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using NetFocus.DataStructure.Gui.Dialogs;
using NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Nodes;

namespace NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Panels
{
	class RuleSetsOptionPanel : NodeOptionPanel
	{
		private System.Windows.Forms.Button addBtn;
		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.Button removeBtn;
		
		public RuleSetsOptionPanel(RuleSetsNode parent) : base(parent)
		{
			SetupFromXmlFile(System.IO.Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\HighlightingEditor\RuleSets.xfrm"));
			
			addBtn = (Button)ControlDictionary["addBtn"];
			addBtn.Click += new EventHandler(addClick);
			removeBtn = (Button)ControlDictionary["removeBtn"];
			removeBtn.Click += new EventHandler(removeClick);
			
			listView  = (ListView)ControlDictionary["listView"];
		}
		
		public override void StoreSettings()
		{
		}
		
		public override void LoadSettings()
		{
			RuleSetsNode node = (RuleSetsNode)parentNode;
			listView.Items.Clear();
			
			foreach (RuleSetNode rn in node.Nodes) {
				if (rn.Name == "") continue;
				ListViewItem lv = new ListViewItem(rn.Name);
				lv.Tag = rn;
				listView.Items.Add(lv);
			}
		}
		
		void addClick(object sender, EventArgs e)
		{
			using (InputBox box = new InputBox()) {
				box.Label.Text = ResourceService.GetString("Dialog.HighlightingEditor.RuleSets.EnterName");
				if (box.ShowDialog() == DialogResult.Cancel) return;
				
				if (box.TextBox.Text == "") return;
				foreach (ListViewItem item in listView.Items) {
					if (item.Text == box.TextBox.Text)
						return;
				}
				
				RuleSetNode rsn = new RuleSetNode(box.TextBox.Text, "&<>~!@%^*()-+=|\\#/{}[]:;\"' ,	.?", "", false, false);
				ListViewItem lv = new ListViewItem(box.TextBox.Text);
				lv.Tag = rsn;
				parentNode.Nodes.Add(rsn);
				listView.Items.Add(lv);
			}
		}
		
		void removeClick(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count != 1) return;
			
			((TreeNode)listView.SelectedItems[0].Tag).Remove();
			listView.SelectedItems[0].Remove();
		}
	}
}
