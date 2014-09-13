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
	public class StackPad : AbstractPadContent
	{
		PictureBox userControl = new PictureBox();

		public override Control Control 
		{
			get 
			{
				return userControl;
			}
		}

		public override void RedrawContent()
		{
			OnTitleChanged(null);
			OnIconChanged(null);
		}
		
		void userControl_Resize(object sender,EventArgs e)
		{
			if(AlgorithmManager.Algorithms.CurrentAlgorithm != null)
			{
				AlgorithmManager.Algorithms.CurrentAlgorithm.UpdateAnimationPad();
			}
			else
			{
				AlgorithmManager.Algorithms.ClearStackPad();
			}
		}

		public StackPad() : base("${res:MainWindow.Windows.StackPadLabel}", "Icons.16x16.OpenFolderBitmap")
		{
			userControl.BackColor = SystemColors.Control;
            userControl.Resize += new EventHandler(userControl_Resize);
		}
	
	}
}
