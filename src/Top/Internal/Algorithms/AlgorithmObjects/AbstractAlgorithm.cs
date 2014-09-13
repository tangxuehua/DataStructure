using System;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;

using NetFocus.DataStructure.TextEditor;
using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.Gui.Pads;
using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.Services;


namespace NetFocus.DataStructure.Internal.Algorithm
{
	public abstract class AbstractAlgorithm : IAlgorithm
	{
		object obj = null;
		int currentLine = 0;
		int[] lastLines;
		string[] codeFiles;
		int[] breakPoints;
		Type dialogType = null;

		public int[] BreakPoints
		{
			get
			{
				return breakPoints;
			}
			set
			{
				breakPoints = value;
			}
		}
		public Type DialogType
		{
			get
			{
				return dialogType;
			}
			set
			{
				dialogType = value;
			}
		}
		public int CurrentLine{
			get{
				return currentLine;
			}
			set{
				currentLine = value;
			}
		}
		public int[] LastLines
		{
			get
			{
				return lastLines;
			}
			set
			{
				lastLines = value;
			}
		}

		public string[] CodeFiles {
			get{
				return codeFiles;
			}
			set{
				codeFiles = value;
			}
		}

		public virtual object Status
		{
			get
			{
				return obj;
			}
			set
			{
				obj = value;
			}
		}
		

		public virtual bool ShowCustomizeDialog()
		{
			if(dialogType == null)
			{
				return false;
			}
			Form dialog = dialogType.Assembly.CreateInstance(dialogType.FullName) as Form;

			if(dialog != null && dialog.ShowDialog() == DialogResult.OK)
			{
				return true;
			}
			return false;
		}
		
		
		public virtual void Recover()
		{
			if(WorkbenchSingleton.Workbench.ActiveViewContent != null)
			{
				if(AlgorithmManager.Algorithms.CurrentAlgorithm != null)
				{					
					CurrentLine = 0;
					
					InitGraph();
					UpdateCurrentView();
					UpdatePropertyPad();
					UpdateAnimationPad();

					AlgorithmManager.Algorithms.Timer.Stop();
				}
			}
		}


		public virtual void ActiveWorkbenchWindow_CloseEvent(object sender, EventArgs e) 
		{
			AlgorithmManager.Algorithms.Timer.Tick -= AlgorithmManager.Algorithms.UpdateAlgorithmStatusEventHandler;

			IViewContent content = sender as IViewContent;

			if(content != null && content.AlgorithmType != null && AlgorithmManager.Algorithms.OpeningAlgorithms.Contains(content.AlgorithmType))
			{
				AlgorithmManager.Algorithms.OpeningAlgorithms.Remove(content.AlgorithmType);
			}
			if(AlgorithmManager.Algorithms.OpeningAlgorithms.Count == 0 || (WorkbenchSingleton.Workbench.ActiveViewContent != null && WorkbenchSingleton.Workbench.ActiveViewContent.AlgorithmType == null))
			{
				AlgorithmManager.Algorithms.CurrentAlgorithm = null;
				UpdatePropertyPad();
				UpdateAnimationPad();
			}
			
		}
		
		
		void ActiveCurrentAlgorithm(object sender,EventArgs e)
		{
			AlgorithmManager.Algorithms.Timer.Stop();
			AlgorithmManager.Algorithms.Timer.Tick -= AlgorithmManager.Algorithms.UpdateAlgorithmStatusEventHandler;
			
			Type algorithmType = WorkbenchSingleton.Workbench.ActiveViewContent.AlgorithmType;

			if(algorithmType != null)
			{
				AlgorithmManager.Algorithms.CurrentAlgorithm = (IAlgorithm)AlgorithmManager.Algorithms.GetAlgorithm(algorithmType);
			}
			UpdateCurrentView();

			UpdatePropertyPad();

			UpdateAnimationPad();

		}

		
		public virtual bool GetData()
		{
			return false;
		}
		
		
		public virtual void Initialize(bool isOpen)
		{
			if(isOpen == true)
			{
				Recover();
				return;
			}
			//导入算法文件.
			IFileService fileService = (IFileService)ServiceManager.Services.GetService(typeof(IFileService));
			for(int i = 0; i < AlgorithmManager.Algorithms.CurrentAlgorithm.CodeFiles.Length; i++) 
			{
				fileService.OpenFile(AlgorithmManager.Algorithms.AlgorithmFilesPath + AlgorithmManager.Algorithms.CurrentAlgorithm.CodeFiles[i]);
				fileService.RecentOpenMemeto.RemoveLastFile();
			}

			//将当前行设置为0.
			CurrentLine = 0;
			
			//将算法设为只读.
			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			((TextEditorControl)content.Control).IsReadOnly = true;
			//将当前打开的文件标记为算法,以区别一般打开的文件
			content.AlgorithmType = AlgorithmManager.Algorithms.CurrentAlgorithm.GetType();
			
			content.ViewSelected += new EventHandler(ActiveCurrentAlgorithm);
			
			content.CloseEvent += new EventHandler(ActiveWorkbenchWindow_CloseEvent);

		
		}
		
		
		public abstract void InitGraph();
		
		public abstract void UpdateGraphAppearance();

		public virtual void UpdateAnimationPad()
		{
			AlgorithmManager.Algorithms.ClearStackPad();
		}
		
		
		public abstract void ExecuteAndUpdateCurrentLine();
		
		public void UpdateCurrentView()
		{
			if(WorkbenchSingleton.Workbench.WorkbenchLayout.ActiveViewContent != null && WorkbenchSingleton.Workbench.WorkbenchLayout.ActiveViewContent.AlgorithmType != null)
			{
				TextEditorControl textEditorControl = WorkbenchSingleton.Workbench.WorkbenchLayout.ActiveViewContent.Control as TextEditorControl;
				TextArea textArea = textEditorControl.ActiveTextAreaControl.TextArea;
				if(textArea != null)
				{
					int line = AlgorithmManager.Algorithms.CurrentAlgorithm.CurrentLine;

					if (line >= 0 && line < textArea.Document.TotalNumberOfLines) 
					{
						Point selectionStartPos = new Point(0, line);
						Point selectionEndPos = new Point(textArea.Document.GetLineSegment(line).Length + 1, line);
						textArea.SelectionManager.ClearSelection();
						textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, selectionStartPos, selectionEndPos));

						textArea.Caret.Position = selectionStartPos;//选中相应的行之后设置光标的位置
					}
				}
			}

		}

		
		public void UpdatePropertyPad()
		{
			IPadContent propertyPad = WorkbenchSingleton.Workbench.GetPad(typeof(PropertyPad));
			if(propertyPad != null) 
			{
				((PropertyGrid)propertyPad.Control).SelectedObject = null;
				if(AlgorithmManager.Algorithms.CurrentAlgorithm != null)
				{
					((PropertyGrid)propertyPad.Control).SelectedObject = AlgorithmManager.Algorithms.CurrentAlgorithm.Status;
				}
			}
		}

		//获取设置的断点
		void SetBreakPoints()
		{
			TextEditorControl textEditorControl = WorkbenchSingleton.Workbench.WorkbenchLayout.ActiveViewContent.Control as TextEditorControl;
			TextArea textArea = textEditorControl.ActiveTextAreaControl.TextArea;
			if(textEditorControl != null && textArea != null)
			{
				BreakPoints = (int[])textArea.Document.BookmarkManager.Marks.ToArray(typeof(int));
			}
		}
		//判断是否算法已经运行结束
		bool HaveFinished()
		{
			for(int i = 0;i < AlgorithmManager.Algorithms.CurrentAlgorithm.LastLines.Length;i++)
			{
				if(CurrentLine == AlgorithmManager.Algorithms.CurrentAlgorithm.LastLines[i])//判断是否运行到最后一行
				{	
					return true;
				}
			}
			return false;
		}
		//判断算法是否运行到了设置了断点的行
		bool HaveRunToBreakPoints()
		{
			foreach(int runtoLine in AlgorithmManager.Algorithms.CurrentAlgorithm.BreakPoints)
			{
				if(CurrentLine == runtoLine)//如果当前行到达了任意一个设置断点的行
				{
					return true;
				}
			}
			return false;
		}
		//一个辅助的内部方法，用于判断是否运行到断点行或结束行
		int RunningExceptFinish()
		{
			if(HaveRunToBreakPoints() == true)
			{
				AlgorithmManager.Algorithms.Timer.Stop();

				bool finished = false;
				finished = HaveFinished();
					
				UpdateCurrentView();
				ExecuteAndUpdateCurrentLine();
				UpdatePropertyPad();
				UpdateAnimationPad();
					
				if(finished == true)
				{
					return 2;  //返回2表示算法不仅已经运行到了设置了断点的行,而且已经完成
				}
				return 1;  //返回1表示算法运行到了设置了断点的行
			}
			return 0;  //返回0表示算法没有运行到任何设置了断点的行
		}
		/// <summary>
		/// 算法运行的骨架，一个模板方法
		/// </summary>
		public virtual void UpdateAlgorithmStatus(object sender,EventArgs e)
		{
			if(AlgorithmManager.Algorithms.CurrentAlgorithm != null)
			{
				if(AlgorithmManager.Algorithms.IsRunto == false)  //如果是运行或者单步调试
				{
					SetBreakPoints();

					bool finished = false;
					finished = HaveFinished();
					
					if(HaveRunToBreakPoints() == true)
					{
						AlgorithmManager.Algorithms.Timer.Stop();  //如果运行到了设置断点的行,则停止计时器
					}
					
					UpdateCurrentView();  //以下四个是钩子方法
					ExecuteAndUpdateCurrentLine();
					UpdatePropertyPad();
					UpdateAnimationPad();
					
					if(AlgorithmManager.Algorithms.IsByStep == true)
					{
						AlgorithmManager.Algorithms.Timer.Stop();  //如果是单步调试,则执行一次后让计时器停止
					}

					if(finished == true)
					{
						AlgorithmManager.Algorithms.Timer.Stop();
						MessageBox.Show("算法运行结束！","消息",MessageBoxButtons.OK,MessageBoxIcon.Information);
					}
				}
				else  //如果是直接执行到某行
				{
					int flag1 = RunningExceptFinish();
					if(flag1 >= 1)
					{
						if(flag1 == 2)
						{
							MessageBox.Show("算法运行结束！","消息",MessageBoxButtons.OK,MessageBoxIcon.Information);
						}
						return;
					}
					//注意,要在当前行的前面和后面都要判断是否运行到了设置了断点的行,因为算法运行时并不总是从上往下.
					ExecuteAndUpdateCurrentLine();   

					int flag2 = RunningExceptFinish();
					if(flag2 >= 1)
					{
						if(flag2 == 2)
						{
							MessageBox.Show("算法运行结束！","消息",MessageBoxButtons.OK,MessageBoxIcon.Information);
						}
						return;
					}
					
					//如果当前行到达了任意一个退出算法的行
					if(HaveFinished() == true)
					{
						UpdateCurrentView();
						ExecuteAndUpdateCurrentLine();
						UpdatePropertyPad();
						UpdateAnimationPad();

						AlgorithmManager.Algorithms.Timer.Stop();

						MessageBox.Show("算法运行结束！","消息",MessageBoxButtons.OK,MessageBoxIcon.Information);
					}
					
				}
			}

		}


	}
}
