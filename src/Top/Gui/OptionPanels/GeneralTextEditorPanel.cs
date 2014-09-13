
using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;

using NetFocus.DataStructure.Internal.ExternalTool;
using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Gui.Views;
using NetFocus.Components.AddIns.Codons;
using NetFocus.DataStructure.Gui.Dialogs;
using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.TextEditor;


namespace NetFocus.DataStructure.Gui.Dialogs.OptionPanels
{
	/// <summary>
	/// General texteditor options panel.
	/// </summary>
	public class GeneralTextEditorPanel : AbstractOptionPanel
	{
		int encoding = Encoding.UTF8.CodePage;
		
		class FontDescriptor
		{
			string name;
			bool   isMonospaced;
			
			public string Name {
				get {
					return name;
				}
				set {
					name = value;
				}
			}
			
			public bool IsMonospaced {
				get {
					return isMonospaced;
				}
				set {
					isMonospaced = value;
				}
			}
			
			public FontDescriptor(string name, bool isMonospaced)
			{
				this.name = name;
				this.isMonospaced = isMonospaced;
			}
			
		}
		
		static StringFormat drawStringFormat = new StringFormat(StringFormatFlags.NoWrap);
		static Font         boldComboBoxFont;
		bool IsMonospaced(FontFamily fontFamily)
		{
			using (Bitmap newBitmap = new Bitmap(1, 1)) {
				using (Graphics g  = Graphics.FromImage(newBitmap)) {
					using (Font f = new Font(fontFamily, 10)) {
						int w1 = (int)g.MeasureString("i", f).Width;
						int w2 = (int)g.MeasureString("m", f).Width;
						return w1 == w2;
					}
				}
			}
		}
		
		
		void MeasureComboBoxItem(object sender, System.Windows.Forms.MeasureItemEventArgs e)
		{
			ComboBox comboBox = (ComboBox)sender;
			if (e.Index >= 0) {
				FontDescriptor fontDescriptor = (FontDescriptor)comboBox.Items[e.Index];
				SizeF size = e.Graphics.MeasureString(fontDescriptor.Name, comboBox.Font);
				e.ItemWidth  = (int)size.Width;
				e.ItemHeight = (int)comboBox.Font.Height;
			}
		}	
		
		void ComboBoxDrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
		{
			ComboBox comboBox = (ComboBox)sender;
			e.DrawBackground();
			if (e.Index >= 0) {
				FontDescriptor fontDescriptor = (FontDescriptor)comboBox.Items[e.Index];
				Rectangle drawingRect = new Rectangle(e.Bounds.X,
				                                      e.Bounds.Y,
				                                      e.Bounds.Width,
				                                      e.Bounds.Height);
				
				Brush drawItemBrush = SystemBrushes.WindowText;
				if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) {
					drawItemBrush = SystemBrushes.HighlightText;
				}
				
				e.Graphics.DrawString(fontDescriptor.Name,
				                      fontDescriptor.IsMonospaced ? boldComboBoxFont : comboBox.Font,
				                      drawItemBrush,
				                      drawingRect,
				                      drawStringFormat);
			}
			e.DrawFocusRectangle();
		}
		
		
		Font CurrentFont {
			get {
				int fontSize = 10;
				try {
					fontSize = Math.Max(6, Int32.Parse(ControlDictionary["fontSizeComboBox"].Text));
				} catch (Exception) {}
				
				FontDescriptor fontDescriptor = (FontDescriptor)((ComboBox)ControlDictionary["fontListComboBox"]).Items[((ComboBox)ControlDictionary["fontListComboBox"]).SelectedIndex];
				
				return new Font(fontDescriptor.Name,
				                fontSize);
			}
		}
		
		static Font ParseFont(string font)
		{
			try 
			{
				string[] descr = font.Split(new char[]{',', '='});
				return new Font(descr[1], Single.Parse(descr[3]));
			} 
			catch (Exception) 
			{
				return new Font("Courier New", 10);
			}
		}
		void UpdateFontPreviewLabel(object sender, EventArgs e)
		{
			ControlDictionary["fontPreviewLabel"].Font = CurrentFont;
		}
		
		
		public override void LoadPanelContents()
		{
			SetupFromXmlFile(Path.Combine(PropertyService.DataDirectory, @"resources\panels\GeneralTextEditorPanel.xfrm"));

			((CheckBox)ControlDictionary["enableDoublebufferingCheckBox"]).Checked = ((IProperties)CustomizationObject).GetProperty("DoubleBuffer", true);
			((CheckBox)ControlDictionary["enableCodeCompletionCheckBox"]).Checked  = ((IProperties)CustomizationObject).GetProperty("EnableCodeCompletion", true);
			((CheckBox)ControlDictionary["enableFoldingCheckBox"]).Checked         = ((IProperties)CustomizationObject).GetProperty("EnableFolding", true);
			((CheckBox)ControlDictionary["showQuickClassBrowserCheckBox"]).Checked = ((IProperties)CustomizationObject).GetProperty("ShowQuickClassBrowserPanel", true);
			
			((CheckBox)ControlDictionary["enableAAFontRenderingCheckBox"]).Checked = ((IProperties)CustomizationObject).GetProperty("UseAntiAliasFont", false);
			((CheckBox)ControlDictionary["mouseWheelZoomCheckBox"]).Checked = ((IProperties)CustomizationObject).GetProperty("MouseWheelTextZoom", true);
			
			foreach (String name in CharacterEncodings.Names) 
			{
				((ComboBox)ControlDictionary["textEncodingComboBox"]).Items.Add(name);
			}
			int encodingIndex = 0;
			try 
			{
				encodingIndex = CharacterEncodings.GetEncodingIndex((Int32)((IProperties)CustomizationObject).GetProperty("Encoding", encoding));
			} 
			catch 
			{
				encodingIndex = CharacterEncodings.GetEncodingIndex(encoding);
			}
			((ComboBox)ControlDictionary["textEncodingComboBox"]).SelectedIndex = encodingIndex;
			encoding = CharacterEncodings.GetEncodingByIndex(encodingIndex).CodePage;
			
			Font currentFont = ParseFont(((IProperties)CustomizationObject).GetProperty("DefaultFont", new Font("Courier New", 10)).ToString());
			
			for (int i = 6; i <= 24; ++i) 
			{
				((ComboBox)ControlDictionary["fontSizeComboBox"]).Items.Add(i);
			}
			((ComboBox)ControlDictionary["fontSizeComboBox"]).Text = currentFont.Size.ToString();
			((ComboBox)ControlDictionary["fontSizeComboBox"]).TextChanged += new EventHandler(UpdateFontPreviewLabel);

			InstalledFontCollection installedFontCollection = new InstalledFontCollection();
			
			int index = 0;
			foreach (FontFamily fontFamily in installedFontCollection.Families) 
			{
				if (fontFamily.IsStyleAvailable(FontStyle.Regular) && fontFamily.IsStyleAvailable(FontStyle.Bold)  && fontFamily.IsStyleAvailable(FontStyle.Italic)) 
				{
					if (fontFamily.Name == currentFont.Name) 
					{
						index = ((ComboBox)ControlDictionary["fontListComboBox"]).Items.Count;
					}
					((ComboBox)ControlDictionary["fontListComboBox"]).Items.Add(new FontDescriptor(fontFamily.Name, IsMonospaced(fontFamily)));
				}
			}
			
			((ComboBox)ControlDictionary["fontListComboBox"]).SelectedIndex = index;
			((ComboBox)ControlDictionary["fontListComboBox"]).TextChanged += new EventHandler(UpdateFontPreviewLabel);
			((ComboBox)ControlDictionary["fontListComboBox"]).SelectedIndexChanged += new EventHandler(UpdateFontPreviewLabel);
			((ComboBox)ControlDictionary["fontListComboBox"]).MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.MeasureComboBoxItem);
			((ComboBox)ControlDictionary["fontListComboBox"]).DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ComboBoxDrawItem);
			
			boldComboBoxFont = new Font(ControlDictionary["fontListComboBox"].Font, FontStyle.Bold);
			
			UpdateFontPreviewLabel(this, EventArgs.Empty);
		}
		

		public override bool StorePanelContents()
		{

			((IProperties)CustomizationObject).SetProperty("DoubleBuffer",         ((CheckBox)ControlDictionary["enableDoublebufferingCheckBox"]).Checked);
			((IProperties)CustomizationObject).SetProperty("UseAntiAliasFont",     ((CheckBox)ControlDictionary["enableAAFontRenderingCheckBox"]).Checked);
			((IProperties)CustomizationObject).SetProperty("MouseWheelTextZoom",   ((CheckBox)ControlDictionary["mouseWheelZoomCheckBox"]).Checked);
			((IProperties)CustomizationObject).SetProperty("EnableCodeCompletion", ((CheckBox)ControlDictionary["enableCodeCompletionCheckBox"]).Checked);
			((IProperties)CustomizationObject).SetProperty("EnableFolding",        ((CheckBox)ControlDictionary["enableFoldingCheckBox"]).Checked);
			((IProperties)CustomizationObject).SetProperty("DefaultFont",          CurrentFont.ToString());
			((IProperties)CustomizationObject).SetProperty("Encoding",             CharacterEncodings.GetCodePageByIndex(((ComboBox)ControlDictionary["textEncodingComboBox"]).SelectedIndex));
			((IProperties)CustomizationObject).SetProperty("ShowQuickClassBrowserPanel", ((CheckBox)ControlDictionary["showQuickClassBrowserCheckBox"]).Checked);
			


			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (content != null && (content is TextEditorView)) {
				TextEditorControl textEditorContorl = content.Control as TextEditorControl;
				textEditorContorl.OptionsChanged();
			}
			return true;
		}
		
		
	}
}
