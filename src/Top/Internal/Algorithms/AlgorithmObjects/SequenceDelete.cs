using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Xml;


using NetFocus.DataStructure.Gui.Views;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.TextEditor;
using NetFocus.DataStructure.Internal.Algorithm.Glyphs;
using NetFocus.DataStructure.Gui.Algorithm.Dialogs;


namespace NetFocus.DataStructure.Internal.Algorithm 
{
	/// <summary>
	/// 封装一个算法: "把线性表中第I个位置的元素删除"
	/// </summary>
	public class SequenceDelete : AbstractAlgorithm 
	{
		SequenceDeleteStatus status;
		SquareLine squareLine;
		IIterator arrayIterator;
		ArrayList movedGlyphs = new ArrayList();
		ArrayList statusItemList = new ArrayList();
		int squareSpace = 5;
		int squareSize = 50;
		string l;
		int i;

		public override object Status
		{
			get
			{
				return status;
			}
			set
			{
				status = value as SequenceDeleteStatus;
			}
		}
		
		
		public override void ActiveWorkbenchWindow_CloseEvent(object sender, EventArgs e) 
		{
			arrayIterator = null;

			base.ActiveWorkbenchWindow_CloseEvent(sender,e);

		}
		
		public override void Recover()
		{
			status = new SequenceDeleteStatus(l,i);
			base.Recover();
		}

		Image CreatePreviewImage(string s,int pos)
		{
			int height = 80;
			int width = 530;
			int space = 2;
			int size = 35;
			int leftSpan = 15;
			int topSpan = 20;
			ArrayList squareArray = new ArrayList();
			IGlyph glyph;
			for(int i = 0;i < s.Length;i++)
			{
				if(pos - 1 != i)
				{
					glyph = new Square(leftSpan + i*(size + space),topSpan,size,Color.DarkCyan,GlyphAppearance.Flat,s[i].ToString());
				
				}
				else  //用红色表示要删除的元素
				{
					glyph = new Square(leftSpan + i*(size + space),topSpan,size,Color.Red,GlyphAppearance.Flat,s[i].ToString());				
				}
				squareArray.Add(glyph);
			}
			squareLine = new SquareLine(1,1,1,squareArray);

			IIterator arrayIterator = squareLine.CreateIterator();

			Bitmap bmp = new Bitmap(width,height);
			Graphics g = Graphics.FromImage(bmp);

			for(IIterator iterator = arrayIterator.First();!arrayIterator.IsDone();iterator = arrayIterator.Next())
			{
				iterator.CurrentItem.Draw(g);
			}

			return bmp;

		}

		public override bool GetData()
		{
			statusItemList.Clear();

			StatusItemControl statusItemControl = new StatusItemControl();

			Hashtable table = AlgorithmManager.Algorithms.GetExampleDatas();
			
			if(table != null)
			{
				XmlNode node = table[typeof(SequenceDelete).ToString()] as XmlElement;

				XmlNodeList childNodes  = node.ChildNodes;
		
				StatusItem statusItem = null;

				foreach (XmlElement el in childNodes)
				{
					string s = el.Attributes["OriginalString"].Value;
					int pos = Convert.ToInt32(el.Attributes["DeletePosition"].Value);

					statusItem = new StatusItem(new SequenceDeleteStatus(s,pos));
					statusItem.Height = 80;
					statusItem.Image = CreatePreviewImage(s,pos);
					statusItemList.Add(statusItem);
				}
			}
			DialogType = typeof(SequenceDeleteDialog);

			InitDataForm form = new InitDataForm();

			form.StatusItemList = statusItemList;

			if(form.ShowDialog() != DialogResult.OK)
			{
				return false;
			}
			if(form.SelectedIndex >= 0)  //说明用户是通过选中某个模板来初始化数据的
			{
				StatusItem selectedItem = form.StatusItemList[form.SelectedIndex] as StatusItem;
				if(selectedItem != null)
				{
					SequenceDeleteStatus tempStatus = selectedItem.ItemInfo as SequenceDeleteStatus;
					if(tempStatus != null)
					{
						l = tempStatus.L;
						i = tempStatus.I;
					}
				}
			}
			else  //说明用户选择自定义数据
			{
				l = status.L;
				i = status.I;
			}
			return true;
			
		}

		
		public override void Initialize(bool isOpen)
		{
			base.Initialize(isOpen);

			status = new SequenceDeleteStatus(l,i);

			InitGraph();
			
			WorkbenchSingleton.Workbench.ActiveViewContent.SelectView();

		}
		
		
		public override void InitGraph() 
		{
			ArrayList squareArray = new ArrayList();
			IGlyph glyph;
			for(int i=0;i<status.Length;i++)
			{
				if(status.I-1 != i)
				{
					glyph = new Square(40 + i*(squareSize + squareSpace),40,squareSize,status.图形背景色,status.图形外观,status.L[i].ToString());
				
				}
				else  //用红色表示要删除的元素
				{
					glyph = new Square(40 + i*(squareSize + squareSpace),40,squareSize,status.删除元素背景色,status.图形外观,status.L[i].ToString());
				}
				squareArray.Add(glyph);
			}
			squareLine = new SquareLine(20,80,status.Length*(squareSize+20+10),squareArray);

			arrayIterator = squareLine.CreateIterator();

		}

		
		public override void ExecuteAndUpdateCurrentLine()
		{
			switch (CurrentLine)
			{
				case 0:
					CurrentLine = 3;
					return;
				case 3:
					CurrentLine = 6;
					return;
				case 6:
					IGlyph glyph1 = ((ArrayIterator)arrayIterator).GetGlyphByIndex(status.I-1);
					status.CanEdit = true;
					status.E = ((Square)glyph1).Text;
					break;
				case 7:
					//判断for语句条件是否成立，如果成立，CurrentLine = 8;不成立，CurrentLine = 10;
					if(status.J > status.Length -1)
					{
						CurrentLine = 10;
						return;
					}
					break;
				case 8:
					//移动一个图形元素.
					movedGlyphs.Add(((ArrayIterator)arrayIterator).GetGlyphByIndex(status.J));
					IGlyph tempGlyph = ((ArrayIterator)arrayIterator).GetGlyphByIndex(status.J);
					IGlyph tempGlyph1 = new Square(tempGlyph.Bounds.X - tempGlyph.Bounds.Width - squareSpace,tempGlyph.Bounds.Y,squareSize,status.图形背景色,status.图形外观,((Square)tempGlyph).Text);
					((ArrayIterator)arrayIterator).MoveGlyphHorizon(status.J,tempGlyph1,0);
					string c = status.L[status.J].ToString();
					status.CanEdit = true;
					status.L = status.L.Remove(status.J - 1,1);
					status.CanEdit = true;
					status.L = status.L.Insert(status.J - 1,c);
					status.CanEdit = true;
					status.J++;
					CurrentLine = 7;
					return;
				case 10:
					//线性表长度减1
					status.CanEdit = true;
					status.Length--;
					status.CanEdit = true;
					status.L = status.L.Remove(status.Length,1);
					//删除最后一个结点
					((ArrayIterator)arrayIterator).DeleteGlyph(status.Length);
					break;
				case 11:
					return;
					
			}
			CurrentLine++;

		}
		
		
		public override void UpdateAnimationPad()
		{
			base.UpdateAnimationPad();
			Graphics g = AlgorithmManager.Algorithms.ClearAnimationPad();
			
			
			if(AlgorithmManager.Algorithms.CurrentAlgorithm != null)
			{
				if(arrayIterator != null)
				{
					for(IIterator iterator = arrayIterator.First();!arrayIterator.IsDone();iterator = arrayIterator.Next())
					{
						iterator.CurrentItem.Draw(g);
					}
				}
			}

		}


		public override void UpdateGraphAppearance()
		{
			int i = 0;
			for(IIterator iterator = arrayIterator.First();!arrayIterator.IsDone();iterator = arrayIterator.Next(),i++)
			{
				if(i != status.I - 1)
				{
					iterator.CurrentItem.BackColor = status.图形背景色;
					iterator.CurrentItem.Appearance = status.图形外观;
				}
				else
				{
					iterator.CurrentItem.BackColor = status.删除元素背景色;
					iterator.CurrentItem.Appearance = status.图形外观;
				}
			}
		}
		

	}

}
