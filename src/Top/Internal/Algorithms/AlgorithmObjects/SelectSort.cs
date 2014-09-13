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

	public class SelectSort : AbstractAlgorithm
	{
		ArrayList statusItemList = new ArrayList();
		SelectSortStatus status = null;
		string r;
		SquareLine squareLine;
		IIterator arrayIterator;
		int startX = 100;
		int maxHeight = 200;
		int heightUnit;
		int histogramWidth = 30;
		int histogramSpace = 30;
		int bottomY = 30;
		ArrayList indexArray = new ArrayList();

		public override object Status
		{
			get
			{
				return status;
			}
			set
			{
				status = value as SelectSortStatus;
			}
		}

		
		public override void ActiveWorkbenchWindow_CloseEvent(object sender, EventArgs e) 
		{
			arrayIterator = null;
			indexArray = new ArrayList();

			base.ActiveWorkbenchWindow_CloseEvent(sender,e);

		}

		
		public override void Recover()
		{
			arrayIterator = null;
			indexArray = new ArrayList();
			status = new SelectSortStatus(this.r);
			base.Recover();
		}

		
		Image CreatePreviewImage(string r)
		{
			int width = 530;
			int startX = 10;
			int maxHeight = 145;
			int heightUnit;
			int histogramWidth = 20;
			int histogramSpace = 15;
			int bottomY = 30;

			heightUnit = (maxHeight - bottomY - 15) / r.Length;
			int height = 0;
			ArrayList squareArray = new ArrayList();
			IGlyph glyph;
			glyph = new MyRectangle(startX,maxHeight - bottomY - 45,histogramWidth,45,Color.Gold,GlyphAppearance.Popup,"?");
			squareArray.Add(glyph);
			for(int i=0;i<r.Length;i++)
			{
				height = GetHeight(r,i,heightUnit);
				glyph = new MyRectangle(startX + (i + 1)*(histogramWidth + histogramSpace),maxHeight - bottomY - height,histogramWidth,height,Color.HotPink,GlyphAppearance.Popup,r[i].ToString());
				squareArray.Add(glyph);
			}
			squareLine = new SquareLine(1,1,1,squareArray);

			//最后初始化所有的迭代器
			IIterator arrayIterator = squareLine.CreateIterator();

			Bitmap bmp = new Bitmap(width,maxHeight);
			Graphics g = Graphics.FromImage(bmp);

			if(arrayIterator != null)
			{
				for(IIterator iterator = arrayIterator.First();!arrayIterator.IsDone();iterator = arrayIterator.Next())
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
				XmlNode node = table[typeof(SelectSort).ToString()] as XmlElement;

				XmlNodeList childNodes  = node.ChildNodes;
		
				StatusItem statusItem = null;

				foreach (XmlElement el in childNodes)
				{
					string r = el.Attributes["OriginalString"].Value;

					statusItem = new StatusItem(new SelectSortStatus(r));
					statusItem.Height = 145;
					statusItem.Image = CreatePreviewImage(r);
					statusItemList.Add(statusItem);
				}
			}
			DialogType = typeof(SelectSortDialog);

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
					SelectSortStatus tempStatus = selectedItem.ItemInfo as SelectSortStatus;
					if(tempStatus != null)
					{
						r = tempStatus.R;
					}
				}
			}
			else  //说明用户选择自定义数据
			{
				r = status.R;
			}
			return true;
			
		}


		public override void Initialize(bool isOpen)
		{
			base.Initialize(isOpen);
			
			status = new SelectSortStatus(r);

			InitGraph();
			
			WorkbenchSingleton.Workbench.ActiveViewContent.SelectView();
		}

		
		int GetHeight(string l,int index,int heightUnit)
		{
			int height = heightUnit;

			if(index >= l.Length || index < 0)
			{
				return 0;
			}
			char c = l[index];
			for(int i = 0;i < l.Length;i++)
			{
				if(c > l[i])
				{
					height += heightUnit;
				}
			}
			return height;

		}
		public override void InitGraph() 
		{
			heightUnit = (maxHeight - bottomY - 15) / status.N;
			int height = 0;
			ArrayList squareArray = new ArrayList();
			IGlyph glyph;
			glyph = new MyRectangle(startX,maxHeight - bottomY - 100,histogramWidth,100,status.头元素背景色,status.图形外观,"?");
			squareArray.Add(glyph);
			for(int i=0;i<status.N;i++)
			{
				height = GetHeight(status.R,i,heightUnit);
				glyph = new MyRectangle(startX + (i + 1)*(histogramWidth + histogramSpace),maxHeight - bottomY - height,histogramWidth,height,status.图形背景色,status.图形外观,status.R[i].ToString());
				squareArray.Add(glyph);
			}
			squareLine = new SquareLine(1,1,1,squareArray);

			//最后初始化所有的迭代器
			arrayIterator = squareLine.CreateIterator();

		}

		string ch;
		int currentIndex;
		int currentIndexI;
		void AssignValue(int index1,int index2)
		{
			int x,y,width,height;
			string text;
			IGlyph glyph2 = ((ArrayIterator)arrayIterator).GetGlyphByIndex(index2);
			IGlyph glyph1 = ((ArrayIterator)arrayIterator).GetGlyphByIndex(index1);
			if(glyph2 != null && glyph1 != null)
			{
				x = glyph1.Bounds.X;  //x坐标不变
				y = glyph2.Bounds.Y;
				width = glyph2.Bounds.Width;
				height = glyph2.Bounds.Height;
				text = ((MyRectangle)glyph2).Text;

				glyph1.Bounds = new Rectangle(x,y,width,height);
				((MyRectangle)glyph1).Text = text;
			}

		}
		void AddIndexToIndexArray(int index)
		{
			if(indexArray.Contains(index) == true)
			{
				return;
			}
			indexArray.Add(index);

		}
		public override void ExecuteAndUpdateCurrentLine()
		{
			switch (CurrentLine)
			{
				case 0:
					CurrentLine = 2;
					return;
				case 2:  //for(i = 1;i < n;i++){
					if(status.I >= status.N)
					{
						CurrentLine = 15;
						return;
					}
					break;
				case 3:  //k = i;
					status.CanEdit = true;
					status.K = status.I;
					((ArrayIterator)arrayIterator).SetBackColor(0,status.头元素背景色,status.图形背景色,true);

					break;
				case 4:  //for(j = i + 1;j <= n;j++){
					if(status.J > status.N)
					{
						CurrentLine = 9;
						return;
					}
					break;
				case 5:  //if(R[j].key < R[k].key){
					//以下4句话设置当前要被交换的那个元素的颜色
					indexArray.Clear();
					currentIndexI = status.I;
					currentIndex = status.K;
					AddIndexToIndexArray(currentIndexI);
					AddIndexToIndexArray(currentIndex);
					((ArrayIterator)arrayIterator).SetElementsBackColor(indexArray,status.当前背景色,status.图形背景色);

					if(status.R[status.J - 1] >= status.R[status.K -1])
					{
						CurrentLine = 7;
						return;
					}

					//以下6句话设置当前最小的那个元素及当前要被交换的那个元素的颜色
					indexArray.Clear();
					currentIndexI = status.I;
					currentIndex = status.J;
					AddIndexToIndexArray(currentIndexI);
					AddIndexToIndexArray(currentIndex);
					((ArrayIterator)arrayIterator).SetElementsBackColor(indexArray,status.当前背景色,status.图形背景色);

					break;
				case 6:  //k = j;
					status.CanEdit = true;
					status.K = status.J;
					break;
				case 7:  //}
					status.CanEdit = true;
					status.J++;
					CurrentLine = 4;

					return;
				case 9:  //if(i != k){
					if(status.I == status.K)
					{
							CurrentLine = 13;
							return;
					}
					break;
				case 10:  //R[0] = R[k];
					AssignValue(0,status.K);
					ch = status.R[status.K - 1].ToString();
					break;
				case 11: //R[k] = R[i];
					AssignValue(status.K,status.I);

					string c = status.R[status.I - 1].ToString();
					status.CanEdit = true;
					status.R = status.R.Remove(status.K - 1,1);
					status.CanEdit = true;
					status.R = status.R.Insert(status.K - 1,c);
					break;
				case 12: //R[i] = R[0];
					AssignValue(status.I,0);
					status.CanEdit = true;
					status.R = status.R.Remove(status.I - 1,1);
					status.CanEdit = true;
					status.R = status.R.Insert(status.I - 1,ch);
					break;
				case 13: //}
					status.CanEdit = true;
					status.I++;
					status.CanEdit = true;
					status.J = status.I + 1;
					CurrentLine = 2;
					return;
				case 15: //return;
					return;

			}
			CurrentLine++;
		}
		

		public override void UpdateGraphAppearance()
		{
			for(IIterator iterator = arrayIterator.First();!arrayIterator.IsDone();iterator = arrayIterator.Next())
			{
				iterator.CurrentItem.BackColor = status.图形背景色;
				iterator.CurrentItem.Appearance = status.图形外观;
			}
			arrayIterator.First().CurrentItem.BackColor = status.头元素背景色;
			arrayIterator.First().CurrentItem.Appearance = status.图形外观;

			IGlyph glyph;
			glyph = ((ArrayIterator)arrayIterator).GetGlyphByIndex(currentIndex);
			if(glyph != null)
			{
				glyph.BackColor = status.当前背景色;
				glyph.Appearance = status.图形外观;
			}
			glyph = ((ArrayIterator)arrayIterator).GetGlyphByIndex(currentIndexI);
			if(glyph != null)
			{
				glyph.BackColor = status.当前背景色;
				glyph.Appearance = status.图形外观;
			}
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

	}
}
