
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Internal.Algorithm;

namespace NetFocus.DataStructure.Gui.Pads
{
	public class PropertyPad : AbstractPadContent
	{
		static PropertyGrid        grid = null;
		static string title = ((ResourceService)ServiceManager.Services.GetService(typeof(ResourceService))).GetString("MainWindow.Windows.PropertiesScoutLabel");
		
		public override Control Control 
		{
			get {
				return grid;
			}
		}
		

		void PropertyChanged(object sender, PropertyValueChangedEventArgs e)
		{
			AlgorithmManager.Algorithms.CurrentAlgorithm.UpdateGraphAppearance();
			IPadContent animationPad = WorkbenchSingleton.Workbench.GetPad(typeof(AnimationPad));
			AlgorithmManager.Algorithms.CurrentAlgorithm.UpdateAnimationPad();
		}
		
		
		public PropertyPad() : base(title,"Icons.16x16.PropertiesIcon")
		{			
			grid = new PropertyGrid();
			grid.PropertyValueChanged += new PropertyValueChangedEventHandler(PropertyChanged);
			grid.ToolbarVisible = false;
			grid.PropertySort = PropertySort.Alphabetical;
			grid.ViewForeColor = Color.Navy;
			grid.HelpForeColor = Color.Navy;
		}
		
		
	}
}
