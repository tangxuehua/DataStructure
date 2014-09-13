
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.Gui.Dialogs;
using NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Nodes;

namespace NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Panels
{
	class SpansOptionPanel : NodeOptionPanel
	{
		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.Button addBtn;
		private System.Windows.Forms.Button removeBtn;
		
		public SpansOptionPanel(SpansNode parent) : base(parent)
		{
			SetupFromXmlFile(System.IO.Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\HighlightingEditor\Spans.xfrm"));
			
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
			SpansNode node = (SpansNode)parentNode;
			listView.Items.Clear();
			
			foreach (SpanNode rn in node.Nodes) {
				ListViewItem lv = new ListViewItem(rn.Text);
				lv.Tag = rn;
				listView.Items.Add(lv);
			}
		}
		
		void addClick(object sender, EventArgs e)
		{
			using (InputBox box = new InputBox()) {
				box.Label.Text = ResourceService.GetString("Dialog.HighlightingEditor.Spans.EnterName");
				if (box.ShowDialog() == DialogResult.Cancel) return;
				
				if (box.TextBox.Text == "") return;
				
				SpanNode rsn = new SpanNode(box.TextBox.Text);
				ListViewItem lv = new ListViewItem(rsn.Text);
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
