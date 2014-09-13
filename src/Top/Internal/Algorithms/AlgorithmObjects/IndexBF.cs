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

	public class IndexBF : AbstractAlgorithm
	{
		ArrayList statusItemList = new ArrayList();
		int squareSpace = 5;
		int squareSize = 40;
		IndexBFStatus status = null;
		string s,t;
		int pos;
		IIterator arrayIteratorS;
		IIterator arrayIteratorT;

		public override object Status
		{
			get
			{
				return status;
			}
			set
			{
				status = value as IndexBFStatus;
			}
		}
		

		public override void ActiveWorkbenchWindow_CloseEvent(object sender, EventArgs e) 
		{
			arrayIteratorS = null;
			arrayIteratorT = null;

			base.ActiveWorkbenchWindow_CloseEvent(sender,e);

		}
		
		public override void Recover()
		{
			status = new IndexBFStatus(s,t,pos);
			base.Recover();
		}


		Image CreatePreviewImage(string s,string t,int pos)
		{
			int height = 80;
			int width = 530;
			int space = 2;
			int size = 30;
			int leftSpan = 15;
			int topSpan = 3;

			SquareLine squareLineS;
			SquareLine squareLineT;

			ArrayList squareArrayS = new ArrayList();
			ArrayList squareArrayT = new ArrayList();
			IGlyph glyph;
			for(int i=0;i<s.Length;i++)
			{
				glyph = new Square(leftSpan + i*(size + space),topSpan,size,SystemColors.InactiveBorder,GlyphAppearance.Popup,s[i].ToString());
				squareArrayS.Add(glyph);
			}
			int startX = ((IGlyph)squareArrayS[pos - 1]).Bounds.X;
			for(int i=0;i<t.Length;i++)
			{
				glyph = new Square(startX + i*(size + space),topSpan + size + 2,size,SystemColors.InactiveBorder,GlyphAppearance.Popup,t[i].ToString());
				squareArrayT.Add(glyph);
			}
			squareLineS = new SquareLine(0,0,1,squareArrayS);
			squareLineT = new SquareLine(0,0,1,squareArrayT);
			//最后初始化所有的迭代器
			IIterator arrayIteratorS = squareLineS.CreateIterator();
			IIterator arrayIteratorT = squareLineT.CreateIterator();

			Bitmap bmp = new Bitmap(width,height);
			Graphics g = Graphics.FromImage(bmp);

			for(IIterator iterator = arrayIteratorS.First();!arrayIteratorS.IsDone();iterator = arrayIteratorS.Next())
			{
				iterator.CurrentItem.Draw(g);
			}
			for(IIterator iterator = arrayIteratorT.First();!arrayIteratorT.IsDone();iterator = arrayIteratorT.Next())
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
				XmlNode node = table[typeof(IndexBF).ToString()] as XmlElement;

				XmlNodeList childNodes  = node.ChildNodes;
		
				StatusItem statusItem = null;

				foreach (XmlElement el in childNodes)
				{
					string s = el.Attributes["String1"].Value;
					string t = el.Attributes["String2"].Value;
					int pos = Convert.ToInt32(el.Attributes["StartPosition"].Value);

					statusItem = new StatusItem(new IndexBFStatus(s,t,pos));
					statusItem.Height = 80;
					statusItem.Image = CreatePreviewImage(s,t,pos);
					statusItemList.Add(statusItem);
				}
			}
			DialogType = typeof(IndexBFDialog);
			
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
					IndexBFStatus tempStatus = selectedItem.ItemInfo as IndexBFStatus;
					if(tempStatus != null)
					{
						s = tempStatus.S;
						t = tempStatus.T;
						pos = tempStatus.Pos;
					}
				}
			}
			else  //说明用户选择自定义数据
			{
				s = status.S;
				t = status.T;
				pos = status.Pos;
			}
			return true;
			
		}


		public override void Initialize(bool isOpen)
		{
			base.Initialize(isOpen);

			status = new IndexBFStatus(s,t,pos);

			InitGraph();
			
			WorkbenchSingleton.Workbench.ActiveViewContent.SelectView();
	
		}

		
		public override void InitGraph() 
		{
			SquareLine squareLineS;
			SquareLine squareLineT;

			ArrayList squareArrayS = new ArrayList();
			ArrayList squareArrayT = new ArrayList();
			IGlyph glyph;
			for(int i=0;i<status.SLength;i++)
			{
				glyph = new Square(40 + i*(squareSize + squareSpace),40 + 5,squareSize,status.串S元素颜色,status.图形外观,status.S[i].ToString());
				squareArrayS.Add(glyph);
			}
			int startX = ((IGlyph)squareArrayS[status.Pos - 1]).Bounds.X;
			for(int i=0;i<status.TLength;i++)
			{
				glyph = new Square(startX + i*(squareSize + squareSpace),40 + squareSize + 2 * 5,squareSize,status.串T元素颜色,status.图形外观,status.T[i].ToString());
				squareArrayT.Add(glyph);
			}
			squareLineS = new SquareLine(20,30,status.SLength*(squareSize+20+10),squareArrayS);
			squareLineT = new SquareLine(20,110,status.TLength*(squareSize+20+10),squareArrayT);
			//最后初始化所有的迭代器
			arrayIteratorS = squareLineS.CreateIterator();
			arrayIteratorT = squareLineT.CreateIterator();
		}

		
		public override void ExecuteAndUpdateCurrentLine()
		{
			switch (CurrentLine)
			{
				case 0:
					CurrentLine = 3;
					return;
				case 3:  //i=pos;  j=1;
					status.CanEdit = true;
					status.I = status.Pos;
					status.CanEdit = true;
					status.J = 1;
					break;
				case 4:  //While(i<=s[0] && j<=T[0]){
					if(status.I > status.SLength || status.J > status.TLength)
					{
						CurrentLine = 10;
						return;
					}
					break;
				case 5:  //if(S[i]==T[j])
					((ArrayIterator)arrayIteratorS).SetBackColor(status.I - 1,status.当前元素颜色,status.串S元素颜色,true);
					((ArrayIterator)arrayIteratorT).SetBackColor(status.J - 1,status.当前元素颜色,status.串T元素颜色,true);
					if(status.S[status.I - 1] != status.T[status.J - 1])
					{
						CurrentLine = 8;
						return;
					}
					break;
				case 6:  //{i++;  j++}
					status.CanEdit = true;
					status.I++;
					status.CanEdit = true;
					status.J++;
					CurrentLine = 4;
					return;
				case 8:  //{i=i-j+2;  j=1}
					((ArrayIterator)arrayIteratorT).MoveGlyphsHorizon(squareSize + squareSpace);
					((ArrayIterator)arrayIteratorS).SetBackColor(0,status.串S元素颜色,status.串S元素颜色,true);
					((ArrayIterator)arrayIteratorT).SetBackColor(0,status.串T元素颜色,status.串T元素颜色,true);
					status.CanEdit = true;
					status.I = status.I - status.J + 2;
					status.CanEdit = true;
					status.J = 1;
					CurrentLine = 4;
					return;
				case 10: //if(j>T[0])
					if(status.J <= status.TLength)
					{
						CurrentLine = 13;
						return;
					}
					break;
				case 11: //return   i-T[0];
					status.CanEdit = true;
					status.找到位置 = status.I - status.TLength;
					return;
				case 13: //return 0;
					status.CanEdit = true;
					status.找到位置 = 0;
					return;
			}
			CurrentLine++;
		}
		

		public override void UpdateGraphAppearance()
		{
			int indexI = status.I > status.SLength ? status.I - 2 : status.I - 1;
			((ArrayIterator)arrayIteratorS).SetBackColor(indexI,status.当前元素颜色,status.串S元素颜色,true);
			int indexJ = status.J > status.TLength ? status.J - 2 : status.J - 1;
			((ArrayIterator)arrayIteratorT).SetBackColor(indexJ,status.当前元素颜色,status.串T元素颜色,true);

			for(IIterator iterator = arrayIteratorS.First();!arrayIteratorS.IsDone();iterator = arrayIteratorS.Next())
			{
				iterator.CurrentItem.Appearance = status.图形外观;
			}
			for(IIterator iterator = arrayIteratorT.First();!arrayIteratorT.IsDone();iterator = arrayIteratorT.Next())
			{
				iterator.CurrentItem.Appearance = status.图形外观;
			}
		}
		
		
		public override void UpdateAnimationPad() 
		{
			base.UpdateAnimationPad();
			Graphics g = AlgorithmManager.Algorithms.ClearAnimationPad();
			
			if(AlgorithmManager.Algorithms.CurrentAlgorithm != null)
			{
				if(arrayIteratorS != null)
				{
					for(IIterator iterator = arrayIteratorS.First();!arrayIteratorS.IsDone();iterator = arrayIteratorS.Next())
					{
						iterator.CurrentItem.Draw(g);
					}
				}
				if(arrayIteratorT != null)
				{
					for(IIterator iterator = arrayIteratorT.First();!arrayIteratorT.IsDone();iterator = arrayIteratorT.Next())
					{
						iterator.CurrentItem.Draw(g);
					}
				}
			}
		}


	}
}
