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
	public class ListInsert : AbstractAlgorithm
	{
		IIterator circleNodeIterator;//用于输出结点链表的指针
		IIterator lineNodeIterator;//用于输出连接线链表的指针
		IIterator nullIteratorP;//箭头p
		IIterator nullIteratorS;//箭头s
		IIterator circleNodeIterator1;//指针p
		IIterator nullIteratorBezierLine;//用于画一条贝赛尔曲线
		IIterator iteratorInsertNode;//表示要插入到链表中的结点
		ArrayList statusItemList = new ArrayList();
		ListInsertStatus status = null;
		int diameter = 50;
		int lineLength = 40;
		int lineWidth = 2;
		string l;
		int i;
		char e;

		int x = 0,y = 0;

		public override object Status
		{
			get
			{
				return status;
			}
			set
			{
				status = value as ListInsertStatus;	
			}
		}
		
		
		public override void ActiveWorkbenchWindow_CloseEvent(object sender, EventArgs e) 
		{
			
			circleNodeIterator = null;
			lineNodeIterator   = null;
			nullIteratorP      = null;
			nullIteratorS      = null;
			circleNodeIterator1= null;
			iteratorInsertNode = null;
			nullIteratorBezierLine = null;

			base.ActiveWorkbenchWindow_CloseEvent(sender,e);

		}
	
		public override void Recover()
		{
			circleNodeIterator = null;
			lineNodeIterator   = null;
			nullIteratorP      = null;
			nullIteratorS      = null;
			circleNodeIterator1= null;
			iteratorInsertNode = null;
			nullIteratorBezierLine = null;

			status = new ListInsertStatus(l,i,e);
			base.Recover();
		}


		Image CreatePreviewImage(string s,int pos,char e)
		{
			int height = 80;
			int width = 530;
			int lineLength = 18;
			int diameter = 30;
			int leftSpan = 3;
			int topSpan = 20;


			IIterator circleNodeIterator;//用于输出结点链表的指针
			IIterator lineNodeIterator;//用于输出连接线链表的指针

			LinkCircleNode headNode = null;
			LinkLineNode   headLineNode = null;
			LinkCircleNode previousNode = null;
			LinkLineNode   previousLineNode = null;
			LinkCircleNode currentNode = null;
			LinkLineNode   currentLineNode = null;
			previousNode = headNode = new LinkCircleNode(leftSpan,topSpan,diameter,Color.Red,"H");
			previousLineNode = headLineNode = new LinkLineNode(leftSpan + diameter,topSpan + diameter/2,leftSpan + diameter + lineLength,topSpan + diameter/2,lineWidth,Color.Red);

			s = s.Insert(pos - 1,e.ToString());

			for(int i = 0;i < s.Length;i++)
			{
				if(i != pos - 1)
				{
					currentNode = new LinkCircleNode(leftSpan + (i + 1) * (lineLength + diameter),topSpan,diameter,Color.DarkTurquoise,s[i].ToString());
				}
				else
				{
					currentNode = new LinkCircleNode(leftSpan + (i + 1) * (lineLength + diameter),topSpan,diameter,Color.DarkOrange,s[i].ToString());
				}
				currentLineNode = new LinkLineNode(leftSpan + diameter + (i + 1) * (lineLength + diameter),topSpan + diameter/2,leftSpan + diameter + (i + 1) * (lineLength + diameter) + lineLength,topSpan + diameter/2,lineWidth,Color.Red);
				previousNode.Next = currentNode;
				previousLineNode.Next = currentLineNode;

				previousNode = currentNode;
				previousLineNode = currentLineNode;

			}

			currentNode.Next = null;
			currentLineNode.Next = null;

			circleNodeIterator = headNode.CreateIterator(); 
			lineNodeIterator = headLineNode.CreateIterator();

			Bitmap bmp = new Bitmap(width,height);
			Graphics g = Graphics.FromImage(bmp);

			if(circleNodeIterator != null)
			{
				for(IIterator iterator = circleNodeIterator.First();!circleNodeIterator.IsDone();iterator = circleNodeIterator.Next())
				{
					iterator.CurrentItem.Draw(g);
				}
			}
			if(lineNodeIterator != null)
			{
				for(IIterator iterator = lineNodeIterator.First();!lineNodeIterator.IsDone();iterator = lineNodeIterator.Next())
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
				XmlNode node = table[typeof(ListInsert).ToString()] as XmlElement;

				XmlNodeList childNodes  = node.ChildNodes;
		
				StatusItem statusItem = null;

				foreach (XmlElement el in childNodes)
				{
					string l = el.Attributes["OriginalString"].Value;
					int i = Convert.ToInt32(el.Attributes["InsertPosition"].Value);
					char e = Convert.ToChar(el.Attributes["InsertData"].Value);

					statusItem = new StatusItem(new ListInsertStatus(l,i,e));
					statusItem.Height = 80;
					statusItem.Image = CreatePreviewImage(l,i,e);
					statusItemList.Add(statusItem);
				}
			}
			DialogType = typeof(ListInsertDialog);

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
					ListInsertStatus tempStatus = selectedItem.ItemInfo as ListInsertStatus;
					if(tempStatus != null)
					{
						l = tempStatus.L;
						i = tempStatus.I;
						e = tempStatus.E;
					}
				}
			}
			else  //说明用户选择自定义数据
			{
				l = status.L;
				i = status.I;
				e = status.E;
			}
			return true;
			
		}


		public override void Initialize(bool isOpen)
		{
			base.Initialize(isOpen);

			status = new ListInsertStatus(l,i,e);

			InitGraph();
			
			WorkbenchSingleton.Workbench.ActiveViewContent.SelectView();

		}
		
		
		public override void InitGraph() 
		{
			LinkCircleNode previousNode = null; //当前要创建的节点的前驱节点
			LinkCircleNode currentNode = null;  //当前要创建的节点
			LinkCircleNode headNode = null;     //头节点

			//这里一条连接线也代表一个节点
			LinkLineNode   headLineNode = null;
			LinkLineNode   currentLineNode = null;     
			LinkLineNode   previousLineNode = null;

			headNode = new LinkCircleNode(40,50,diameter,status.头结点颜色,"H");  //先创建一个头节点
			headLineNode = new LinkLineNode(40 + diameter,50 + diameter/2,40 + diameter + lineLength,50 + diameter/2,lineWidth,Color.Red);

			for(int i=0;i<status.Length;i++)
			{
				currentNode = new LinkCircleNode(40 + (i+1)*(diameter + lineLength),50,diameter,status.结点颜色,status.L[i].ToString());
				currentLineNode = new LinkLineNode(40 + diameter + (i+1)*(diameter + lineLength),50 + diameter/2,40 + diameter + (i+1)*(diameter + lineLength) + lineLength,50 + diameter/2,lineWidth,Color.Red);
				
				if(i == 0)  //说明当前创建的节点为第一个节点
				{
					headNode.Next = currentNode;
					headLineNode.Next = currentLineNode;
				}
				else
				{
					previousNode.Next = currentNode;
					previousLineNode.Next = currentLineNode;
				}
				previousNode = currentNode;
				previousLineNode = currentLineNode;
			}

			currentNode.Next = null;  //这里要注意,要把最后一个节点的Next设置为null
			currentLineNode.Next = null;

			circleNodeIterator = headNode.CreateIterator(); 
			circleNodeIterator1 = headNode.CreateIterator(); 
			lineNodeIterator = headLineNode.CreateIterator();

		}


		Point[] GetPoints()
		{
			int x0,y0,x1,y1,x2,y2,x3,y3;
			x0 = iteratorInsertNode.First().CurrentItem.Bounds.X + diameter;
			y0 = iteratorInsertNode.First().CurrentItem.Bounds.Y + diameter / 2;
			x1 = x0 + 30;
			y1 = y0 - diameter / 5;
			x2 = x0 - 30;
			y2 = y0 - 2 * diameter / 5;
			x3 = x0 - 5;
			y3 = y0 - diameter;
			Point point0 = new Point(x0,y0);
			Point point1 = new Point(x1,y1);
			Point point2 = new Point(x2,y2);
			Point point3 = new Point(x3,y3);

			Point[] points = new Point[]{point0,point1,point2,point3};

			return points;
		}
		public override void ExecuteAndUpdateCurrentLine()
		{
			switch (CurrentLine)
			{
				case 0:
					CurrentLine = 2;
					return;
				case 2:  //p = L;j = 0;
					circleNodeIterator1 = circleNodeIterator1.First();//使p指向头结点
					status.CanEdit = true;
					status.J = 0;
					nullIteratorP = new Pointer(circleNodeIterator1.CurrentItem.Bounds.X - 33,circleNodeIterator1.CurrentItem.Bounds.Y - 33,Color.Purple,"p").CreateIterator();
					status.CanEdit = true;
					status.P = "p当前指向结点" + ((LinkCircleNode)circleNodeIterator1.CurrentItem).Text;
					break;
				case 3:  //while(p && j<i-1){
					//判断while语句条件是否成立，如果成立，CurrentLine = 4;不成立，CurrentLine = 6;
					if(circleNodeIterator1 == null || status.J >= status.I - 1)
					{
						CurrentLine = 6;
						return;
					}
					break;
				case 4:  //p = p->next; j++;
					circleNodeIterator1 = circleNodeIterator1.Next();  //p下移一个结点
					((NodeListIterator)circleNodeIterator).SetCurrentItemNewColor(circleNodeIterator1.CurrentItem,status.当前结点颜色,status.结点颜色);
					status.CanEdit = true;
					status.P = "p当前指向结点" + ((LinkCircleNode)circleNodeIterator1.CurrentItem).Text;
					//把指针p设置到新的位置
					((Pointer)nullIteratorP.CurrentItem).SetToNewPosition(circleNodeIterator1.CurrentItem.Bounds.X - 33,circleNodeIterator1.CurrentItem.Bounds.Y - 33);
					status.CanEdit = true;
					status.J++;
					CurrentLine = 3;
					return;
				case 6:  //if(p == NULL)
					CurrentLine = 8;
					return;
				case 8:  //s = (LinkList)malloc(sizeof(LNode));
					x = this.circleNodeIterator1.CurrentItem.Bounds.X + diameter - 5;
					y = this.circleNodeIterator1.CurrentItem.Bounds.Y + diameter;
					iteratorInsertNode = new LinkCircleNode(x,y,diameter,status.插入结点颜色,"").CreateIterator();
					break;
				case 9:  //s->data = e;
					iteratorInsertNode = new LinkCircleNode(x,y,diameter,status.插入结点颜色,status.E.ToString()).CreateIterator();
					break;
				case 10:  //s->next = p->next;
					nullIteratorBezierLine = new BezierLine(new Rectangle(1,1,1,1),GetPoints(),2,Color.Red).CreateIterator();
					break;
				case 11:  //p->next = s;
					int xx1,yy1;
					xx1 = circleNodeIterator1.CurrentItem.Bounds.X + diameter;
					yy1 = circleNodeIterator1.CurrentItem.Bounds.Y + diameter / 2;
					nullIteratorBezierLine = null;
					iteratorInsertNode.CurrentItem.Bounds = new Rectangle(xx1 + lineLength,yy1 - diameter / 2,diameter,diameter);
					((NodeListIterator)circleNodeIterator).InsertNodeAndRefresh(status.I,(INode)iteratorInsertNode.CurrentItem,diameter,lineLength);

					LinkLineNode insertLineNode = new LinkLineNode(xx1 + diameter + lineLength,yy1,xx1 + diameter + 2 * lineLength,yy1,2,Color.Black);
					((NodeListIterator)lineNodeIterator).InsertNodeAndRefresh(status.I,insertLineNode,diameter,lineLength);
					status.CanEdit = true;
					status.L = status.L.Insert(status.I - 1,status.E.ToString());
					status.CanEdit = true;
					status.Length++;
					break;
				case 12:
					return;
			}
			CurrentLine++;
			
		}
		
		
		public override void UpdateGraphAppearance()
		{
			if(circleNodeIterator != null)
			{
				for(IIterator iterator = circleNodeIterator.First();!circleNodeIterator.IsDone();iterator = circleNodeIterator.Next())
				{
					iterator.CurrentItem.BackColor = status.结点颜色;
				}
				//处理头结点
				IGlyph headNode = circleNodeIterator.First().CurrentItem;
				headNode.BackColor = status.头结点颜色;
				//处理当前结点
				if(circleNodeIterator1 != null && circleNodeIterator1.CurrentItem != null)
				{
					circleNodeIterator1.CurrentItem.BackColor = status.当前结点颜色;
				}
				if(iteratorInsertNode != null)
				{
					iteratorInsertNode.First().CurrentItem.BackColor = status.插入结点颜色;
				}
			}
		}
		
		
		public override void UpdateAnimationPad() 
		{
			base.UpdateAnimationPad();
			Graphics g = AlgorithmManager.Algorithms.ClearAnimationPad();
			
			if(AlgorithmManager.Algorithms.CurrentAlgorithm != null)
			{
				if(circleNodeIterator != null)
				{
					for(IIterator iterator = circleNodeIterator.First();!circleNodeIterator.IsDone();iterator = circleNodeIterator.Next())
					{
						iterator.CurrentItem.Draw(g);
					}
				}
				if(lineNodeIterator != null)
				{
					for(IIterator iterator = lineNodeIterator.First();!lineNodeIterator.IsDone();iterator = lineNodeIterator.Next())
					{
						iterator.CurrentItem.Draw(g);
					}
				}
				if(nullIteratorP != null)
				{
					nullIteratorP.CurrentItem.Draw(g);
				}
				if(nullIteratorS != null)
				{
					nullIteratorS.CurrentItem.Draw(g);
				}
				if(iteratorInsertNode != null)
				{
					iteratorInsertNode.First().CurrentItem.Draw(g);
				}
				if(nullIteratorBezierLine != null)
				{
					nullIteratorBezierLine.CurrentItem.Draw(g);
				}
			}

		}


	}
}
