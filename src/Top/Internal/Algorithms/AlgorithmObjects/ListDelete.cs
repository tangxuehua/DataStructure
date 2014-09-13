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
	public class ListDelete : AbstractAlgorithm
	{
		IIterator circleNodeIterator;//用于输出结点链表的指针
		IIterator lineNodeIterator;//用于输出连接线链表的指针
		IIterator nullIteratorP;//箭头p
		IIterator nullIteratorQ;//箭头q
		IIterator swerveLineIterator;//用来画转弯的线
		IIterator circleNodeIterator1;//指针p
		ArrayList statusItemList = new ArrayList();
		int diameter = 50;
		int lineLength = 40;
		int lineWidth = 2;

		string l;
		int i;
		ListDeleteStatus status = null;

		public override object Status
		{
			get
			{
				return status;
			}
			set
			{
				status = value as ListDeleteStatus;
			}
		}
		
		
		public override void ActiveWorkbenchWindow_CloseEvent(object sender, EventArgs e) 
		{
			
			circleNodeIterator = null;
			lineNodeIterator   = null;
			nullIteratorP      = null;
			nullIteratorQ      = null;
			circleNodeIterator1= null;
			swerveLineIterator = null;

			base.ActiveWorkbenchWindow_CloseEvent(sender,e);


		}

	
		public override void Recover()
		{
			circleNodeIterator = null;
			lineNodeIterator   = null;
			nullIteratorP      = null;
			nullIteratorQ      = null;
			circleNodeIterator1= null;
			swerveLineIterator = null;

			status = new ListDeleteStatus(l,i);
			base.Recover();
		}


		Image CreatePreviewImage(string s,int pos)
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

			for(int i = 0;i < s.Length;i++)
			{
				if(i != pos - 1)
				{
					currentNode = new LinkCircleNode(leftSpan + (i + 1) * (lineLength + diameter),topSpan,diameter,Color.DarkTurquoise,s[i].ToString());
				}
				else
				{
					currentNode = new LinkCircleNode(leftSpan + (i + 1) * (lineLength + diameter),topSpan,diameter,Color.Yellow,s[i].ToString());
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
				XmlNode node = table[typeof(ListDelete).ToString()] as XmlElement;

				XmlNodeList childNodes  = node.ChildNodes;
		
				StatusItem statusItem = null;

				foreach (XmlElement el in childNodes)
				{
					string l = el.Attributes["OriginalString"].Value;
					int i = Convert.ToInt32(el.Attributes["DeletePosition"].Value);

					statusItem = new StatusItem(new ListDeleteStatus(l,i));
					statusItem.Height = 80;
					statusItem.Image = CreatePreviewImage(l,i);
					statusItemList.Add(statusItem);
				}
			}
			DialogType = typeof(ListDeleteDialog);

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
					ListDeleteStatus tempStatus = selectedItem.ItemInfo as ListDeleteStatus;
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
			
			status = new ListDeleteStatus(l,i);

			InitGraph();
			
			WorkbenchSingleton.Workbench.ActiveViewContent.SelectView();

		}
		
		
		public override void InitGraph() 
		{
			LinkCircleNode previousNode = null; //当前要创建的节点的前驱节点
			LinkCircleNode currentNode = null;  //当前要创建的节点
			LinkCircleNode headNode = null;     //头节点

			//这里一条线也代表一个节点
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

		
		Point[] GetPoints(int x,int y,int width,int height)
		{
			Point point0 = new Point(x - lineLength / 2,y);
			Point point1 = new Point(x,y);
			Point point2 = new Point(x,y + height);
			Point point3 = new Point(x + width,y + height);
			Point point4 = new Point(x + width,y);
			Point[] points = new Point[]{point0,point1,point2,point3,point4};

			return points;
		}
		public override void ExecuteAndUpdateCurrentLine()
		{
			switch (CurrentLine)
			{
				case 0:
					CurrentLine = 2;
					return;
				case 2:
					circleNodeIterator1 = circleNodeIterator1.First();//使p指向头结点
					status.CanEdit = true;
					status.J = 0;
					nullIteratorP = new Pointer(circleNodeIterator1.CurrentItem.Bounds.X - 33,circleNodeIterator1.CurrentItem.Bounds.Y - 33,Color.Purple,"p").CreateIterator();
					status.CanEdit = true;
					status.P = "p当前指向结点" + ((LinkCircleNode)circleNodeIterator1.CurrentItem).Text;
					break;
				case 3:  //while(p->next && j<i-1)
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
				case 6:
					CurrentLine = 8;
					return;
				case 8:  //q = p->next;
					IGlyph glyph = ((NodeListIterator)circleNodeIterator1).GetNextGlyph();//注意:这里不仅取出p所指的图元,而且把p指针下移一个
					nullIteratorQ = new Pointer(glyph.Bounds.X - 33,glyph.Bounds.Y - 33,Color.Red,"q").CreateIterator();
					break;
				case 9:  //p->next = q->next;
					int x,y,width,height;
					x = circleNodeIterator1.CurrentItem.Bounds.X + diameter + lineLength / 2;
					y = circleNodeIterator1.CurrentItem.Bounds.Y + diameter / 2;
					width = diameter + lineLength;
					height = diameter;
					swerveLineIterator = new SwerveLine(new Rectangle(x,y,width,height),GetPoints(x,y,width,height),2,Color.Red).CreateIterator();
					((NodeListIterator)lineNodeIterator).SetNodeUnVisible(status.I - 1);
					status.CanEdit = true;
					status.L = status.L.Remove(status.I - 1,1);
					status.CanEdit = true;
					status.Length -= 1; 
					break;
				case 10:  //e = q->data; free(q);
					status.CanEdit = true;
					status.E = ((LinkCircleNode)((NodeListIterator)circleNodeIterator1).GetNextGlyph()).Text[0];
					//删除q所指的结点
					((NodeListIterator)circleNodeIterator).DeleteNodeAndRefresh(status.I,diameter,lineLength);
					//删除一条连接线,这里因为所有的连接线都一样,所以随便指定一个删除的索引即可,这里,我总是指定为status.Length,即删除最后一条线
					((NodeListIterator)lineNodeIterator).DeleteNodeAndRefresh(status.Length,diameter,lineLength);
					//删除q指针
					nullIteratorQ = null;
					//将刚才设置为透明的线重新显示出来
					((NodeListIterator)lineNodeIterator).SetNodeVisible(status.I - 1,Color.Red);
					//删除转弯的线
					swerveLineIterator = null;
					break;
				case 11:
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
				if(nullIteratorQ != null)
				{
					nullIteratorQ.CurrentItem.Draw(g);
				}
				if(swerveLineIterator != null)
				{
					swerveLineIterator.CurrentItem.Draw(g);
				}
			}
		}


	}
}
