using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Threading;
using System.Xml;

using NetFocus.DataStructure.Gui.Views;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.TextEditor;
using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.Gui.Pads;
using NetFocus.DataStructure.Gui.Algorithm.Dialogs;
using NetFocus.DataStructure.Internal.Algorithm.Glyphs;


namespace NetFocus.DataStructure.Internal.Algorithm
{
	public class BSTSearch : AbstractAlgorithm
	{
		object status = null;

		public override object Status
		{
			get
			{
				return status;
			}
		}

		
		public override void ActiveWorkbenchWindow_CloseEvent(object sender, EventArgs e) 
		{
			base.ActiveWorkbenchWindow_CloseEvent(sender,e);
		}
		

		public override void Recover()
		{

		}

		
		public override void Initialize(bool isOpen)
		{
			base.Initialize(isOpen);
			
			//获取算法初始化数据,TODO

			//实例化一个算法状态对象,现在先自己设定一组样本数据,以后用参数获得.
			//status = new CreateListStatus("12345678",8);

			//初始化图形元素.
			InitGraph();
			
			WorkbenchSingleton.Workbench.ActiveViewContent.SelectView();
		}

		
		public override void InitGraph() 
		{

		}

		
		public override void ExecuteAndUpdateCurrentLine()
		{

		}
		

		public override void UpdateGraphAppearance()
		{

		}
		
		
		public override void UpdateAnimationPad() 
		{

		}

	}
}
