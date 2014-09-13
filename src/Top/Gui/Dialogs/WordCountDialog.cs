
using System;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.Properties;

using NetFocus.DataStructure.Services;

namespace NetFocus.DataStructure.Gui.Dialogs
{
	/// <summary>
	///    Summary description for Win32Form1.
	/// </summary>
	public class WordCountDialog : System.Windows.Forms.Form
	{
		Container components;
		
		ColumnHeader lineHeader;
		ColumnHeader wordHeader;
		ColumnHeader charHeader;
		ColumnHeader fileHeader;
		
		Button helpButton;
		Button cancelButton;
		Button startButton;
		Label  locationLabel;
		ComboBox locationComboBox;
		ListView resultListView;
		ResourceService ResourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		
		internal class Report
		{
			string name;
			long chars;
			long words;
			long lines;
			
			public Report(string name, long chars, long words, long lines)
			{
				this.name  = name;
				this.chars = chars;
				this.words = words;
				this.lines = lines;
			}
			
			public ListViewItem ToListItem()
			{
				return new ListViewItem(new string[] { Path.GetFileName(name), chars.ToString(), words.ToString(), lines.ToString()});
			}
			
			public static Report operator+(Report r, Report s)
			{
				ResourceService ResourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
				Report tmp = new Report(ResourceService.GetString("Dialog.WordCountDialog.TotalText"), s.chars, s.words, s.lines);
				tmp.chars += r.chars;
				tmp.words += r.words;
				tmp.lines += r.lines;
				return tmp;
			}
		}
		
		Report GetReport(string filename)
		{
			long numLines = 0;
			long numWords = 0;
			long numChars = 0;
			
			FileStream istream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			StreamReader sr = new StreamReader(istream);
			string line = sr.ReadLine();
			while (line != null) {
				++numLines;
				numChars += line.Length;
				string[] words = line.Split(null);
				numWords += words.Length;
				line = sr.ReadLine();
			}
			
			sr.Close();
			return new Report(filename, numChars, numWords, numLines);
		}
		
		void startEvent(object sender, System.EventArgs e)
		{
			resultListView.BeginUpdate();
			resultListView.Items.Clear();
			switch (locationComboBox.SelectedIndex) {
				case 0: {// current file
				IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
					if (content != null) {
						if (content.ContentName == null) {
							MessageBox.Show(ResourceService.GetString("Dialog.WordCountDialog.SaveTheFileWarning"), ResourceService.GetString("Global.WarningText"),
							                MessageBoxButtons.OK, MessageBoxIcon.Warning);
						} else {
							FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
							if (fileUtilityService.TestFileExists(content.ContentName)) {
								resultListView.Items.Add(GetReport(content.ContentName).ToListItem());
							}
						}
					}
					break;
				}
				case 1: {// all open files
				if (WorkbenchSingleton.Workbench.ViewContentCollection.Count > 0) {
					string tmp = "";
					
					Report all = new Report(ResourceService.GetString("Dialog.WordCountDialog.TotalText"), 0, 0, 0);
					foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
						if (content.ContentName == null) {
							MessageBox.Show(ResourceService.GetString("Dialog.WordCountDialog.SaveAllFileWarning"), ResourceService.GetString("Global.WarningText"),
							                MessageBoxButtons.OK, MessageBoxIcon.Warning);
							continue;
						} else {
							FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
							if (fileUtilityService.TestFileExists(content.ContentName)) {
								Report r = GetReport(content.ContentName);
								tmp += r.ToString();
								all += r;
								resultListView.Items.Add(r.ToListItem());
							}
						}
					}
					resultListView.Items.Add(new ListViewItem(""));
					resultListView.Items.Add(all.ToListItem());
				}
				break;
				}
			}
			resultListView.EndUpdate();
		}
		
		public WordCountDialog()
		{
			InitializeComponent();
			AcceptButton = startButton;
			CancelButton = cancelButton;
			startButton.Click += new System.EventHandler(startEvent);
			ShowInTaskbar = false;
			Icon = null;
			StartPosition = FormStartPosition.CenterParent;
		}
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null){
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		private void InitializeComponent()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			bool flat = Crownwood.Magic.Common.VisualStyle.IDE == (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("NetFocus.DataStructure.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
			
			this.components = new System.ComponentModel.Container();
			this.resultListView = new System.Windows.Forms.ListView();
			this.charHeader = new System.Windows.Forms.ColumnHeader();
			this.lineHeader = new System.Windows.Forms.ColumnHeader();
			this.locationLabel = new System.Windows.Forms.Label();
			this.wordHeader = new System.Windows.Forms.ColumnHeader();
			this.cancelButton = new System.Windows.Forms.Button();
			this.locationComboBox = new System.Windows.Forms.ComboBox();
			this.startButton = new System.Windows.Forms.Button();
			this.fileHeader = new System.Windows.Forms.ColumnHeader();
			this.helpButton = new System.Windows.Forms.Button();
			
			resultListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
			resultListView.MultiSelect = false;
			resultListView.Size = new System.Drawing.Size(368, 256);
			resultListView.FullRowSelect = true;
			resultListView.View = System.Windows.Forms.View.Details;
			resultListView.ForeColor = System.Drawing.SystemColors.WindowText;
			resultListView.GridLines = true;
			resultListView.TabIndex = 0;
			resultListView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left;
			resultListView.Location = new System.Drawing.Point(8, 40);
			resultListView.Columns.Add(fileHeader);
			resultListView.Columns.Add(charHeader);
			resultListView.Columns.Add(wordHeader);
			resultListView.Columns.Add(lineHeader);
			resultListView.BorderStyle  = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			
			charHeader.Text = ResourceService.GetString("Dialog.WordCountDialog.CharsText");
			charHeader.Width = 60;
			charHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			
			lineHeader.Text = ResourceService.GetString("Dialog.WordCountDialog.LinesText");
			lineHeader.Width = 60;
			lineHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			
			locationLabel.Location = new System.Drawing.Point(8, 8);
			locationLabel.Text = ResourceService.GetString("Dialog.WordCountDialog.Label1Text");
			
			locationLabel.Size = new System.Drawing.Size(80, 16);
			locationLabel.TabIndex = 1;
			
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.Text = ResourceService.GetString("Dialog.WordCountDialog.DialogName");
			this.MaximizeBox = false;
			//@design this.TrayLargeIcon = true;
			this.ShowInTaskbar = false;
			this.FormBorderStyle = FormBorderStyle.Sizable;
			//@design this.TrayHeight = 0;
			//			this.TopMost = true;
			this.MinimizeBox = false;
			this.ClientSize = new System.Drawing.Size(456 + 10, 301);
			
			wordHeader.Text = ResourceService.GetString("Dialog.WordCountDialog.WordsText");
			wordHeader.Width = 60;
			wordHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			
			cancelButton.Location = new System.Drawing.Point(384, 40);
			cancelButton.Size = new System.Drawing.Size(74, 24);
			cancelButton.TabIndex = 4;
			cancelButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			cancelButton.Text = ResourceService.GetString("Global.CancelButtonText");
			cancelButton.DialogResult = DialogResult.Cancel;
			cancelButton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			locationComboBox.Location = new System.Drawing.Point(96, 8);
			locationComboBox.Text = "";
			locationComboBox.Size = new System.Drawing.Size(176, 21);
			locationComboBox.TabIndex = 1;
			locationComboBox.Items.Add(ResourceService.GetString("Global.Location.currentfile"));
			locationComboBox.Items.Add(ResourceService.GetString("Global.Location.allopenfiles"));
			locationComboBox.ValueMember = locationComboBox.Items[0].ToString();
			
			locationComboBox.SelectedIndex = 0;
			startButton.Location = new System.Drawing.Point(384, 8);
			startButton.Size = new System.Drawing.Size(74, 24);
			startButton.TabIndex = 3;
			startButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			startButton.Text = ResourceService.GetString("Dialog.WordCountDialog.StartButton");
			startButton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			fileHeader.Text = ResourceService.GetString("Dialog.WordCountDialog.FileText");
			fileHeader.Width = 177;
			fileHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
			
			helpButton.Location = new System.Drawing.Point(384, 80);
			helpButton.Size = new System.Drawing.Size(74, 24);
			helpButton.TabIndex = 5;
			helpButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			helpButton.Text = ResourceService.GetString("Global.HelpButtonText");
			helpButton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			this.Controls.Add(helpButton);
			this.Controls.Add(cancelButton);
			this.Controls.Add(startButton);
			this.Controls.Add(locationLabel);
			this.Controls.Add(locationComboBox);
			this.Controls.Add(resultListView);
			
		}
		
	}
}
