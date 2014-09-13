using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Reflection;
using System.Globalization;

using NetFocus.DataStructure.Internal.ExternalTool;
using NetFocus.Components.AddIns.Codons;
using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Gui.Components;
using NetFocus.DataStructure.Services;
using NetFocus.Components.AddIns;
using NetFocus.DataStructure.Internal.Algorithm;

namespace NetFocus.DataStructure.Gui.Dialogs.OptionPanels
{

	public class AnimationOptionsPanel : AbstractOptionPanel
	{
		Color backColor;

		Color ParseColor(string c)
		{
			int a = 255;
			int offset = 0;
			Color color;
			try
			{
				offset = 2;
				a = Int32.Parse(c.Substring(1,2), NumberStyles.HexNumber);
				int r = Int32.Parse(c.Substring(1 + offset,2), NumberStyles.HexNumber);
				int g = Int32.Parse(c.Substring(3 + offset,2), NumberStyles.HexNumber);
				int b = Int32.Parse(c.Substring(5 + offset,2), NumberStyles.HexNumber);

				color = Color.FromArgb(a, r, g, b);
			}
			catch
			{
				color = Color.FromName(c);
			}
			return color;
		}
		
		
		void ModifyBackColorButton_Click(object sender,EventArgs e)
		{
			using (ColorDialog colDialog = new ColorDialog()) 
			{
				colDialog.FullOpen = true;
				if (colDialog.ShowDialog() == DialogResult.OK) 
				{
					backColor = colDialog.Color;
				}
			}
			((Label)ControlDictionary["BackColorPreviewLabel"]).BackColor = backColor;
		}

		public override void LoadPanelContents()
		{
			SetupFromXmlFile(Path.Combine(PropertyService.DataDirectory, @"resources\panels\AnimationPadOptions.xfrm"));

			backColor = ParseColor(PropertyService.GetProperty("NetFocus.DataStructure.AnimationPadPanel.BackColor", "WhiteSmoke"));
			
			((Label)ControlDictionary["BackColorPreviewLabel"]).BackColor = backColor;
			((Button)ControlDictionary["ModifyBackColorButton"]).Click +=new EventHandler(ModifyBackColorButton_Click);
			((ComboBox)ControlDictionary["TimerSpeedComboBox"]).Items.AddRange(new object[]{50,100,200,300,400,500,
			600,700,800,900,1000,1100,1200,1300,1400,1500,1600,1700,1800,1900,2000});

			((ComboBox)ControlDictionary["TimerSpeedComboBox"]).SelectedItem = AlgorithmManager.Algorithms.ExecuteSpeed;
		}
		
		
		string GetColorName(Color c)
		{
			if(c.IsNamedColor)
			{
				return c.Name;
			}
			return "#" + c.Name;
		}
		
		
		public override bool StorePanelContents()
		{
			object o = ((ComboBox)ControlDictionary["TimerSpeedComboBox"]).SelectedItem;

			AlgorithmManager.Algorithms.ExecuteSpeed = Int32.Parse(o.ToString());
			
			if(((CheckBox)ControlDictionary["SavedCheckBox"]).Checked == true)
			{
				PropertyService.SetProperty("NetFocus.DataStructure.AlgorithmExecuteSpeed",Int32.Parse(o.ToString()));
			}
			PropertyService.SetProperty("NetFocus.DataStructure.AnimationPadPanel.BackColor",GetColorName(backColor));
			
			if(AlgorithmManager.Algorithms.CurrentAlgorithm == null)
			{
				AlgorithmManager.Algorithms.ClearAnimationPad();
			}
			else
			{
				AlgorithmManager.Algorithms.CurrentAlgorithm.UpdateAnimationPad();
			}
			return true;
		}
	}
}
