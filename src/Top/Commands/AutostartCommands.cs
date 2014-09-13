using System;
using System.Collections;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Properties;
using NetFocus.Components.AddIns.Codons;
using NetFocus.DataStructure.Gui.Dialogs;
using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.Internal.Algorithm;

using NetFocus.DataStructure.Gui.Dialogs.OptionPanels;

namespace NetFocus.DataStructure.Commands
{
	/// <summary>
	/// 自动启动控制台的一个命令.
	/// </summary>
	public class StartWorkbenchCommand : AbstractCommand
	{
		const string workbenchMemento = "DataStructure.Workbench.WorkbenchMemento";
		
		public override void Run()
		{
			Form f = (Form)WorkbenchSingleton.Workbench;

			foreach (string file in SplashScreenForm.GetRequestedFileList()) {
				try 
				{
					IFileService fileService = (IFileService)ServiceManager.Services.GetService(typeof(IFileService));
					fileService.OpenFile(file);
					IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
					if(viewContent != null)
					{
						viewContent.ViewSelected -= AlgorithmManager.Algorithms.ClearPadsHandler;
						viewContent.ViewSelected += AlgorithmManager.Algorithms.ClearPadsHandler;
						viewContent.SelectView();	
						AlgorithmManager.Algorithms.Timer.Enabled = false;
					}
				} 
				catch (Exception e) 
				{
					Console.WriteLine("不能打开文件： {0} 出现错误 :\n{1}", file, e.ToString());
				}	
			
			}
			Application.Run(f);
			
			// 退出程序后,最后保存工作台的状态.	
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			if(WorkbenchSingleton.Workbench is IMementoCapable)
			{
				IXmlConvertable workbenchStatus = ((IMementoCapable)WorkbenchSingleton.Workbench).CreateMemento();
				propertyService.SetProperty(workbenchMemento, workbenchStatus);
			}
			
		}

	}
}
