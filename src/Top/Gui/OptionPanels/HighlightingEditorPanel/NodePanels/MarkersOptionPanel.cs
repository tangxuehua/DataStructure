
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.Gui.Dialogs;
using NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Nodes;

namespace NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Panels
{
	class MarkersOptionPanel : NodeOptionPanel
	{
		private System.Windows.Forms.Button addBtn;
		private System.Windows.Forms.Button removeBtn;
		private System.Windows.Forms.ListView listView;
		
		bool previous = false;
		
		public MarkersOptionPanel(MarkersNode parent, bool prev) : base(parent)
		{
			SetupFromXmlFile(System.IO.Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\HighlightingEditor\Markers.xfrm"));
			
			addBtn = (Button)ControlDictionary["addBtn"];
			addBtn.Click += new EventHandler(addClick);
			removeBtn = (Button)ControlDictionary["removeBtn"];
			removeBtn.Click += new EventHandler(removeClick);
			
			listView  = (ListView)ControlDictionary["listView"];
			
			previous = prev;
			ControlDictionary["label"].Text = ResourceService.GetString(previous ? "Dialog.HighlightingEditor.Markers.Previous" : "Dialog.HighlightingEditor.Markers.Next");
		}
		
		public override void StoreSettings()
		{
		}
		
		public override void LoadSettings()
		{
			MarkersNode node = (MarkersNode)parentNode;
			listView.Items.Clear();
			
			foreach (MarkerNode rn in node.Nodes) {
				ListViewItem lv = new ListViewItem(rn.What);
				lv.Tag = rn;
				listView.Items.Add(lv);
			}
		}
		
		void addClick(object sender, EventArgs e)
		{
			using (InputBox box = new InputBox()) {
				box.Label.Text = ResourceService.GetString("Dialog.HighlightingEditor.Markers.EnterName");
				if (box.ShowDialog() == DialogResult.Cancel) return;
				
				if (box.TextBox.Text == "") return;
				foreach (ListViewItem item in listView.Items) {
					if (item.Text == box.TextBox.Text)
						return;
				}
				
				MarkerNode rsn = new MarkerNode(box.TextBox.Text, previous);
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
