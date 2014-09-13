using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Globalization;

using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Internal.Algorithm;


namespace NetFocus.DataStructure.Gui.Pads
{
	public class AnimationPad : AbstractPadContent
	{
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		PictureBox pictureBox1 = new PictureBox();

		public override Control Control 
		{
			get 
			{
                return pictureBox1;
			}
		}

		public override void RedrawContent()
		{
			OnTitleChanged(null);
			OnIconChanged(null);
		}
		
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
		void userControl_Resize(object sender,EventArgs e)
		{
			if(AlgorithmManager.Algorithms.CurrentAlgorithm != null)
			{
				AlgorithmManager.Algorithms.CurrentAlgorithm.UpdateAnimationPad();
			}
			else
			{
				AlgorithmManager.Algorithms.ClearAnimationPad();
			}
		}
		
		public AnimationPad() : base("${res:MainWindow.Windows.AnimationPadLabel}", "Icons.16x16.OpenFolderBitmap")
		{
			Color c = ParseColor(propertyService.GetProperty("NetFocus.DataStructure.AnimationPadPanel.BackColor","White"));
			pictureBox1.BackColor = c;
            pictureBox1.Resize += new EventHandler(userControl_Resize);

		}

	
	}
}
