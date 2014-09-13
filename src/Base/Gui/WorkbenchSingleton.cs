using System;
using System.CodeDom.Compiler;
using System.Windows.Forms;

namespace NetFocus.DataStructure.Gui
{
	public class WorkbenchSingleton : DefaultWorkbench
	{
		
		static IWorkbench workbench    = null;
		

		static void CreateWorkspace()//创建整个工作空间.
		{
			DefaultWorkbench w = new DefaultWorkbench();//新建一个空的工作台实例.	
			workbench = w;				
			w.InitializeWorkspace();//初始化菜单,工具栏,状态栏之类的东西.

		}
		

		public static IWorkbench Workbench 
		{
			get {
				if (workbench == null) {  //惰性初始化
					CreateWorkspace();
				}
				return workbench;
			}
		}
		
	}
}
