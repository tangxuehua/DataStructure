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

	public class SequenceMerge : AbstractAlgorithm 
	{
		ArrayList statusItemList = new ArrayList();
		IIterator arrayIteratorLa;
		IIterator arrayIteratorLb;
		IIterator arrayIteratorLc;
		IGlyph currentGlyph;
		int squareSpace = 5;
		int squareSize = 50;
		string la,lb;
		int k = 0;
		
		SequenceMergeStatus status = null;

		public override object Status
		{
			get
			{
				return status;
			}
			set
			{
				status = value as SequenceMergeStatus;
			}
		}

		
		public override void ActiveWorkbenchWindow_CloseEvent(object sender, EventArgs e) 
		{
			arrayIteratorLa = null;
			arrayIteratorLb = null;
			arrayIteratorLc = null;
			k = 0;
			
			base.ActiveWorkbenchWindow_CloseEvent(sender,e);
			
		}


		public override void Recover()
		{
			k = 0;
			status = new SequenceMergeStatus(la,lb);
			base.Recover();
		}

		Image CreatePreviewImage(string s1,string s2)
		{
			int height = 80;
			int width = 530;
			int space = 2;
			int size = 30;
			int leftSpan = 15;
			int topSpan = 5;
			SquareLine squareLineLa;
			SquareLine squareLineLb;

			ArrayList squareArrayLa = new ArrayList();
			ArrayList squareArrayLb = new ArrayList();
			IGlyph glyph;
			for(int i=0;i<s1.Length;i++)
			{
				glyph = new Square(leftSpan + i*(size + space),topSpan,size,Color.DarkCyan,GlyphAppearance.Flat,s1[i].ToString());
				squareArrayLa.Add(glyph);
			}
			for(int i=0;i<s2.Length;i++)
			{
				glyph = new Square(leftSpan + i*(size + space),topSpan + size + 2,size,Color.DarkCyan,GlyphAppearance.Flat,s2[i].ToString());
				squareArrayLb.Add(glyph);
			}

			squareLineLa = new SquareLine(1,1,1,squareArrayLa);
			squareLineLb = new SquareLine(1,1,1,squareArrayLb);

			IIterator arrayIteratorLa = squareLineLa.CreateIterator();
			IIterator arrayIteratorLb = squareLineLb.CreateIterator();

			Bitmap bmp = new Bitmap(width,height);
			Graphics g = Graphics.FromImage(bmp);

			if(arrayIteratorLa != null)
			{
				for(IIterator iterator = arrayIteratorLa.First();!arrayIteratorLa.IsDone();iterator = arrayIteratorLa.Next())
				{
					iterator.CurrentItem.Draw(g);
				}
			}

			if(arrayIteratorLb != null)
			{
				for(IIterator iterator = arrayIteratorLb.First();!arrayIteratorLb.IsDone();iterator = arrayIteratorLb.Next())
				{
					iterator.CurrentItem.Draw(g);
				}
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
				XmlNode node = table[typeof(SequenceMerge).ToString()] as XmlElement;

				XmlNodeList childNodes  = node.ChildNodes;
		
				StatusItem statusItem = null;

				foreach (XmlElement el in childNodes)
				{
					string s1 = el.Attributes["String1"].Value;
					string s2 = el.Attributes["String2"].Value;

					statusItem = new StatusItem(new SequenceMergeStatus(s1,s2));
					statusItem.Height = 80;
					statusItem.Image = CreatePreviewImage(s1,s2);
					statusItemList.Add(statusItem);
				}
			}

			DialogType = typeof(SequenceMergeDialog);

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
					SequenceMergeStatus tempStatus = selectedItem.ItemInfo as SequenceMergeStatus;
					if(tempStatus != null)
					{
						la = tempStatus.La;
						lb = tempStatus.Lb;
					}
				}
			}
			else  //说明用户选择自定义数据
			{
				la = status.La;
				lb = status.Lb;

			}
			return true;
			
		}


		public override void Initialize(bool isOpen)
		{
			base.Initialize(isOpen);

			status = new SequenceMergeStatus(la,lb);

			InitGraph();
			
			WorkbenchSingleton.Workbench.ActiveViewContent.SelectView();


		}
		
		
		public override void InitGraph() 
		{
			SquareLine squareLineLa;
			SquareLine squareLineLb;
			SquareLine squareLineLc;

			ArrayList squareArrayLa = new ArrayList();
			ArrayList squareArrayLb = new ArrayList();
			ArrayList squareArrayLc = new ArrayList();
			IGlyph glyph;
			for(int i=0;i<status.LaLength;i++)
			{
				glyph = new Square(40 + i*(squareSize + squareSpace),10 + 5,squareSize,status.图形背景色,status.图形外观,status.La[i].ToString());
				squareArrayLa.Add(glyph);
			}
			for(int i=0;i<status.LbLength;i++)
			{
				glyph = new Square(40 + i*(squareSize + squareSpace),10 + squareSize + 2 * 5,squareSize,status.图形背景色,status.图形外观,status.Lb[i].ToString());
				squareArrayLb.Add(glyph);
			}

			squareLineLa = new SquareLine(20,30,status.LaLength*(squareSize+20+10),squareArrayLa);
			squareLineLb = new SquareLine(20,110,status.LbLength*(squareSize+20+10),squareArrayLb);
			squareLineLc = new SquareLine(20,190,status.LaLength*(squareSize+20+10) + status.LbLength*(squareSize+20+10),squareArrayLc);
			//最后初始化所有的迭代器
			arrayIteratorLa = squareLineLa.CreateIterator();
			arrayIteratorLb = squareLineLb.CreateIterator();
			arrayIteratorLc = squareLineLc.CreateIterator();

		}


		public override void ExecuteAndUpdateCurrentLine()
		{
			IGlyph glyphI = null;
			IGlyph glyphJ = null;
			IGlyph insertingGlyph = null;

			switch (CurrentLine)
			{
				case 0:
					//i=j=k=0;
					status.CanEdit = true;
					status.I = 0;
					status.CanEdit = true;
					status.J = 0;
					status.CanEdit = true;
					status.K = 0;
					CurrentLine = 4;
					return;
				case 5:
					//判断while（i<La.length && j<Lb.length）是否成立，如果成立，CurrentLine = 6;不成立，CurrentLine = 12;
					if(status.I >=status.LaLength || status.J >= status.LbLength)
					{
						CurrentLine = 12;
						return;
					}
					CurrentLine = 7;
					return;
				case 7:
					//判断If(La.elem[i]<=Lb.elem[j])是否成立，如果成立，CurrentLine = 8;不成立，CurrentLine = 10;
					glyphI = ((ArrayIterator)arrayIteratorLa).GetGlyphByIndex(status.I);
					glyphJ = ((ArrayIterator)arrayIteratorLb).GetGlyphByIndex(status.J);					
					if(((Square)glyphI).Text[0] > ((Square)glyphJ).Text[0])
					{
						CurrentLine = 10;
						return;
					}
					break;
				case 8:
					//Lc.elem[k++]=La.elem[i++];
					glyphI = ((ArrayIterator)arrayIteratorLa).GetGlyphByIndex(status.I);
					currentGlyph = glyphI;
					((ArrayIterator)arrayIteratorLa).SetBackColor(status.I ,status.当前元素背景色,status.图形背景色,true);
					((ArrayIterator)arrayIteratorLb).SetBackColor(-1,status.图形背景色 ,status.图形背景色,true);
					insertingGlyph = new Square(40 + k*(squareSize + squareSpace),10 + 2 * squareSize + 3 * 5,squareSize,status.当前元素背景色,glyphI.Appearance,((Square)glyphI).Text);
					((ArrayIterator)arrayIteratorLc).InsertGlyph(insertingGlyph);
					status.CanEdit = true;
					status.Lc = status.Lc.Insert(status.Lc.Length,((Square)glyphI).Text);
					status.CanEdit = true;
					status.LcLength += 1;
					k += 1;
					status.CanEdit = true;
					status.I += 1;
					status.CanEdit = true;
					status.K += 1;
					CurrentLine = 5;
					return;
				case 10:
					//Lc.elem[k++]=Lb.elem[j++];
					glyphJ = ((ArrayIterator)arrayIteratorLb).GetGlyphByIndex(status.J);
					currentGlyph = glyphJ;
					((ArrayIterator)arrayIteratorLb).SetBackColor(status.J,status.当前元素背景色 ,status.图形背景色,true);
					((ArrayIterator)arrayIteratorLa).SetBackColor(-1,status.图形背景色 ,status.图形背景色,true);
					insertingGlyph = new Square(40 + k*(squareSize + squareSpace),10 + 2 * squareSize + 3 * 5,squareSize,status.当前元素背景色,glyphJ.Appearance,((Square)glyphJ).Text);
					((ArrayIterator)arrayIteratorLc).InsertGlyph(insertingGlyph);
					status.CanEdit = true;
					status.Lc = status.Lc.Insert(status.Lc.Length,((Square)glyphJ).Text);
					status.CanEdit = true;
					status.LcLength += 1;
					k += 1;
					status.CanEdit = true;
					status.J += 1;
					status.CanEdit = true;
					status.K += 1;
					CurrentLine = 5;
					return;
				case 12:
					//判断While(i<La.length)是否成立
					if(status.I >=status.LaLength)
					{
						CurrentLine = 14;
						return;
					}
					break;
				case 13:
					//Lc.elem[k++]=La.elem[i++];
					glyphI = ((ArrayIterator)arrayIteratorLa).GetGlyphByIndex(status.I);
					currentGlyph = glyphI;
					((ArrayIterator)arrayIteratorLa).SetBackColor(status.I,status.当前元素背景色 ,status.图形背景色,true);
					((ArrayIterator)arrayIteratorLb).SetBackColor(-1,status.图形背景色 ,status.图形背景色,true);
					insertingGlyph = new Square(40 + k*(squareSize + squareSpace),10 + 2 * squareSize + 3 * 5,squareSize,status.当前元素背景色,glyphI.Appearance,((Square)glyphI).Text);
					((ArrayIterator)arrayIteratorLc).InsertGlyph(insertingGlyph);
					status.CanEdit = true;
					status.Lc = status.Lc.Insert(status.Lc.Length,((Square)glyphI).Text);
					status.CanEdit = true;
					status.LcLength += 1;
					k += 1;
					status.CanEdit = true;
					status.I += 1;
					status.CanEdit = true;
					status.K += 1;
					CurrentLine = 12;
					return;
				case 14:
					//判断While(j<Lb.length)是否成立
					if(status.J >=status.LbLength)
					{
						CurrentLine = 16;
						return;
					}
					break;
				case 15:
					//Lc.elem[k++]=Lb.elem[j++];
					glyphJ = ((ArrayIterator)arrayIteratorLb).GetGlyphByIndex(status.J);
					currentGlyph = glyphJ;
					((ArrayIterator)arrayIteratorLb).SetBackColor(status.J ,status.当前元素背景色,status.图形背景色,true);
					((ArrayIterator)arrayIteratorLa).SetBackColor(-1,status.图形背景色 ,status.图形背景色,true);
					insertingGlyph = new Square(40 + k*(squareSize + squareSpace),10 + 2 * squareSize + 3 * 5,squareSize,status.当前元素背景色,glyphJ.Appearance,((Square)glyphJ).Text);
					((ArrayIterator)arrayIteratorLc).InsertGlyph(insertingGlyph);
					status.CanEdit = true;
					status.Lc = status.Lc.Insert(status.Lc.Length,((Square)glyphJ).Text);
					status.CanEdit = true;
					status.LcLength += 1;
					k += 1;
					status.CanEdit = true;
					status.J += 1;
					status.CanEdit = true;
					status.K += 1;
					CurrentLine = 14;
					return;
				case 16:
					//Lc.length=k;
					status.LcLength = status.K;
					break;
				case 17:
					return;
			}
			CurrentLine++;
		}

		
		public override void UpdateGraphAppearance()
		{
			for(IIterator iterator = arrayIteratorLa.First();!arrayIteratorLa.IsDone();iterator = arrayIteratorLa.Next())
			{
				iterator.CurrentItem.BackColor = status.图形背景色;
				iterator.CurrentItem.Appearance = status.图形外观;
			}
			for(IIterator iterator = arrayIteratorLb.First();!arrayIteratorLb.IsDone();iterator = arrayIteratorLb.Next())
			{
				iterator.CurrentItem.BackColor = status.图形背景色;
				iterator.CurrentItem.Appearance = status.图形外观;
			}
			for(IIterator iterator = arrayIteratorLc.First();!arrayIteratorLc.IsDone();iterator = arrayIteratorLc.Next())
			{
				iterator.CurrentItem.BackColor = status.当前元素背景色;
				iterator.CurrentItem.Appearance = status.图形外观;
			}
			if(currentGlyph != null)
			{
				currentGlyph.BackColor = status.当前元素背景色;
			}
		}
		
		
		public override void UpdateAnimationPad() 
		{
			base.UpdateAnimationPad();
			Graphics g = AlgorithmManager.Algorithms.ClearAnimationPad();
			
			if(AlgorithmManager.Algorithms.CurrentAlgorithm != null)
			{
				if(arrayIteratorLa != null)
				{
					for(IIterator iterator = arrayIteratorLa.First();!arrayIteratorLa.IsDone();iterator = arrayIteratorLa.Next())
					{
						iterator.CurrentItem.Draw(g);
					}
				}
				if(arrayIteratorLb != null)
				{
					for(IIterator iterator = arrayIteratorLb.First();!arrayIteratorLb.IsDone();iterator = arrayIteratorLb.Next())
					{
						iterator.CurrentItem.Draw(g);
					}
				}
				if(arrayIteratorLc != null)
				{
					for(IIterator iterator = arrayIteratorLc.First();!arrayIteratorLc.IsDone();iterator = arrayIteratorLc.Next())
					{
						iterator.CurrentItem.Draw(g);
					}
				}
			}
		}


	}
}
