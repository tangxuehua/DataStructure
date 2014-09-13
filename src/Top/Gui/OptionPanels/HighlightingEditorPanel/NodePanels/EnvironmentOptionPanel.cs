
using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;

using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Nodes;
using NetFocus.DataStructure.Gui.Dialogs;

namespace NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Panels
{
	class EnvironmentOptionPanel : NodeOptionPanel
	{
		private System.Windows.Forms.Button button;
		private System.Windows.Forms.ListView listView;

		public EnvironmentOptionPanel(EnvironmentNode parent) : base(parent)
		{
			SetupFromXmlFile(System.IO.Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\HighlightingEditor\Environment.xfrm"));
			
			button = (Button)ControlDictionary["button"];
			button.Click += new EventHandler(EditButtonClicked);
			listView  = (ListView)ControlDictionary["listView"];
			
			listView.Font = new Font(listView.Font.FontFamily, 10);
		}
		
		public override void StoreSettings()
		{
			EnvironmentNode node = (EnvironmentNode)parentNode;
			
			foreach (EnvironmentItem item in listView.Items) {
				node.Colors[item.arrayIndex] = item.Color;
			}
		}
		
		public override void LoadSettings()
		{
			EnvironmentNode node = (EnvironmentNode)parentNode;
			listView.Items.Clear();
			
			for (int i = 0; i <= EnvironmentNode.ColorNames.GetUpperBound(0); ++i) {
				listView.Items.Add(new EnvironmentItem(i, node.ColorDescs[i], node.Colors[i], listView.Font));
			}
		}
		
		void EditButtonClicked(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count != 1) return;
			
			EnvironmentItem item = (EnvironmentItem)listView.SelectedItems[0];
			using (EditHighlightingColorDialog dlg = new EditHighlightingColorDialog(item.Color)) {
				if (dlg.ShowDialog(this) == DialogResult.OK) {
					item.Color = dlg.Color;
					item.ColorUpdate();
				}
			}
		}
		
		private class EnvironmentItem : ListViewItem
		{
			public string Name;
			public EditorHighlightColor Color;
			public int arrayIndex;
			
			Font basefont;
			Font listfont;
			
			static Font ParseFont(string font)
			{
				string[] descr = font.Split(new char[]{',', '='});
				return new Font(descr[1], Single.Parse(descr[3]));
			}
			
			public EnvironmentItem(int index, string name, EditorHighlightColor color, Font listFont) : base(new string[] {name, "Sample"})
			{
				Name = name;
				Color = color;
				arrayIndex = index;
				
				this.UseItemStyleForSubItems = false;
				
				IProperties properties = ((IProperties)PropertyService.GetProperty("NetFocus.DataStructure.TextEditor.Document.DefaultDocumentProperties", new DefaultProperties()));
				basefont = ParseFont(properties.GetProperty("DefaultFont", new Font("Courier New", 10).ToString()));
				listfont = listFont;
				
				ColorUpdate();
			}
			
			public void ColorUpdate()
			{
				FontStyle fs = FontStyle.Regular;
				if (Color.Bold)   fs |= FontStyle.Bold;
				if (Color.Italic) fs |= FontStyle.Italic;
				
				this.Font = new Font(listfont.FontFamily, 8);
				
				Font font = new Font(basefont, fs);
				
				this.SubItems[1].Font = font;
				
				this.SubItems[1].ForeColor = Color.GetForeColor();
				this.SubItems[1].BackColor = Color.GetBackColor();
			}
		}
	}
}
