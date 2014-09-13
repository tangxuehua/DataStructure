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
	public class InOrderTraverse : AbstractAlgorithm
	{
		ArrayList statusItemList = new ArrayList();
		IIterator inOrderTreeIterator = null;
		IIterator inOrderTreeIterator1 = null;
		IIterator currentIterator = null;
		IIterator stackIterator = null;
		IIterator visitedIterator = null;
		BiTreeStatus status = null;
		int diameter = 50;
		bool visiting = false;

		XmlNode dataNode = null;

		BiTreeGenerator biTreeGenerator = new BiTreeGenerator();

		public override object Status
		{
			get
			{
				return status;
			}
			set
			{
				dataNode = value as XmlNode;
			}
		}

		
		public override void ActiveWorkbenchWindow_CloseEvent(object sender, EventArgs e) 
		{
			inOrderTreeIterator = null;
			inOrderTreeIterator1 = null;
			currentIterator = null;
			stackIterator = null;
			visitedIterator = null;
			base.ActiveWorkbenchWindow_CloseEvent(sender,e);

		}
	
		
		public override void Recover()
		{
			inOrderTreeIterator = null;
			inOrderTreeIterator1 = null;
			currentIterator = null;
			stackIterator = null;
			visitedIterator = null;
			status = new BiTreeStatus();
			base.Recover();
		}

		
		Image CreatePreviewImage(XmlNode dataNode)
		{
			int height = 240;
			int width = 530;
			int diameter = 40;
			
			Bitmap bmp = new Bitmap(width,height);
			Graphics g = Graphics.FromImage(bmp);

			BiTreeGenerator biTreeGenerator = new BiTreeGenerator();
			biTreeGenerator.DataNode = dataNode;
			biTreeGenerator.IsPreview = true;
			biTreeGenerator.GenerateTree(diameter,Color.HotPink);
			//注意：这里我还是使用先序遍历，因为用什么遍历方法无很大关系，关键是最终的结果要正确
			IIterator preOrderTreeIterator = new BiTreePreOrderIterator(biTreeGenerator.RootNode);
			IIterator preOrderTreeIterator1 =  new BiTreePreOrderIterator(biTreeGenerator.RootLineNode);

			if(preOrderTreeIterator != null)
			{
				for(IIterator iterator = preOrderTreeIterator.First();!preOrderTreeIterator.IsDone();iterator = preOrderTreeIterator.Next())
				{
					if(iterator.CurrentItem != null)
					{
						iterator.CurrentItem.BackColor = Color.HotPink;
						iterator.CurrentItem.Draw(g);
					}
				}
			}
			if(preOrderTreeIterator1 != null)
			{
				for(IIterator iterator = preOrderTreeIterator1.First();!preOrderTreeIterator1.IsDone();iterator = preOrderTreeIterator1.Next())
				{
					if(iterator.CurrentItem != null)
					{
						iterator.CurrentItem.Draw(g);
					}
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
				XmlNode node = table[typeof(InOrderTraverse).ToString()] as XmlElement;

				XmlNodeList childNodes  = node.ChildNodes;
		
				StatusItem statusItem = null;

				foreach (XmlNode el in childNodes)
				{
					statusItem = new StatusItem(el);
					statusItem.Height = 240;
					statusItem.Image = CreatePreviewImage(el);
					statusItemList.Add(statusItem);
				}
			}
			DialogType = typeof(BiTreeDialog);
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
					XmlNode tempNode = selectedItem.ItemInfo as XmlNode;
					if(tempNode != null)
					{
						dataNode = tempNode;
					}
				}
			}
			else  //说明用户选择自定义数据
			{
				//这里不需要输入数据
			}
			return true;
			
		}

		
		public override void Initialize(bool isOpen)
		{
			base.Initialize(isOpen);
			
			status = new BiTreeStatus();

			InitGraph();
			
			WorkbenchSingleton.Workbench.ActiveViewContent.SelectView();
		}

		
		public override void InitGraph() 
		{
			if(dataNode != null)
			{
				biTreeGenerator.DataNode = dataNode;
				biTreeGenerator.IsPreview = false;
				biTreeGenerator.GenerateTree(diameter,status.结点颜色);
			}
			inOrderTreeIterator = new BiTreeInOrderIterator(biTreeGenerator.RootNode);
			inOrderTreeIterator1 =  new BiTreeInOrderIterator(biTreeGenerator.RootLineNode);
			
			stackIterator = new StackIterator(new ArrayList());
			
			visitedIterator = new ArrayIterator(new ArrayList());
		}

		
		IGlyph CreateStackItem(IGlyph nodeGlyph)
		{
			int itemCount = ((StackIterator)stackIterator).ItemCount;
			int x = 2;
			int y = 10;
			int width = 1;
			int height = 28;
			IPadContent stackPad = WorkbenchSingleton.Workbench.GetPad(typeof(NetFocus.DataStructure.Gui.Pads.StackPad));
			if(stackPad != null)
			{
				y = stackPad.Control.Height - (itemCount + 1) * (2 + 28) - 1;
				width = stackPad.Control.Width - 6;
			}
			string text = ((IBiTreeNode)nodeGlyph).Text;

			return new StackItem(x,y,width,height,SystemColors.Control,GlyphAppearance.Popup,text);

		}
		IGlyph CreateVisitedGlyph(IGlyph glyph)
		{
			int count = ((ArrayIterator)visitedIterator).Count;
			int diameter = 35;
			int x = 1;
			int y = 1;
			IPadContent animationPad = WorkbenchSingleton.Workbench.GetPad(typeof(NetFocus.DataStructure.Gui.Pads.AnimationPad));
			if(animationPad != null)
			{
				x = 10 + count * (diameter + 5);
				y = animationPad.Control.Height - diameter - 10;
			}
			string text = ((IBiTreeNode)glyph).Text;

			return new BiTreeNode(x,y,diameter,status.遍历过结点颜色,text);

		}
		public override void ExecuteAndUpdateCurrentLine()
		{
			switch (CurrentLine)
			{
				case 0:
					CurrentLine = 2;
					return;
				case 2: //InitStack(S);  p = T;
					currentIterator = new BiTreeInOrderIterator(biTreeGenerator.RootNode);
					((BiTreeInOrderIterator)currentIterator).SetToRootNode();
					status.CanEdit = true;
					status.P = ((IBiTreeNode)currentIterator.CurrentItem).Text;
					break;
				case 3: //while(p || !StackEmpty(S)){
					if(currentIterator.CurrentItem == null && ((BiTreeInOrderIterator)currentIterator).NodesStack.Count == 0)
					{
						CurrentLine = 16;
						return;
					}
					break;
				case 4: //if(p){
					if(currentIterator.CurrentItem == null)
					{
						CurrentLine = 9;
						return;
					}
					break;
				case 5: //Push(S,p);
					((BiTreeInOrderIterator)currentIterator).PushCurrentNode();
					((StackIterator)stackIterator).PushGlyph(CreateStackItem(currentIterator.CurrentItem));
					break;
				case 6: //p = p->lchild;
					((BiTreeInOrderIterator)currentIterator).SetToLeftChild();
					CurrentLine = 3;
					visiting = false;
					if(currentIterator.CurrentItem == null)
					{
						status.CanEdit = true;
						status.P = null;
					}
					else
					{
						status.CanEdit = true;
						status.P = ((IBiTreeNode)currentIterator.CurrentItem).Text;
					}
					return;
				case 9: //Pop(S,p);
					((BiTreeInOrderIterator)currentIterator).PopupToCurrentNode();
					((StackIterator)stackIterator).Pop();
					visiting = false;
					status.CanEdit = true;
					status.P = ((IBiTreeNode)currentIterator.CurrentItem).Text;
					break;
				case 10: //if(!Visit(p->data)){
					visiting = true;
					CurrentLine = 13;
					((ArrayIterator)visitedIterator).InsertGlyph(CreateVisitedGlyph(currentIterator.CurrentItem));
					return;
				case 13: //p = p->rchild;
					((BiTreeInOrderIterator)currentIterator).SetToRightChild();
					CurrentLine = 3;
					visiting = false;
					if(currentIterator.CurrentItem == null)
					{
						status.CanEdit = true;
						status.P = null;
					}
					else
					{
						status.CanEdit = true;
						status.P = ((IBiTreeNode)currentIterator.CurrentItem).Text;
					}
					return;
				case 16:
					return;
			}
			CurrentLine++;
		}
		

		public override void UpdateGraphAppearance()
		{
			for(IIterator iterator = inOrderTreeIterator.First();!inOrderTreeIterator.IsDone();iterator = inOrderTreeIterator.Next())
			{
				iterator.CurrentItem.BackColor = status.结点颜色;
			}

			for(IIterator iterator = visitedIterator.First();!visitedIterator.IsDone();iterator = visitedIterator.Next())
			{
				if(iterator.CurrentItem != null)
				{
					iterator.CurrentItem.BackColor = status.遍历过结点颜色;
				}
			}
		}
		
		
		public override void UpdateAnimationPad() 
		{
			Graphics g = AlgorithmManager.Algorithms.ClearAnimationPad();
			Graphics g1 = AlgorithmManager.Algorithms.ClearStackPad();
			IPadContent stackPad = WorkbenchSingleton.Workbench.GetPad(typeof(NetFocus.DataStructure.Gui.Pads.StackPad));
			if(stackPad != null && stackIterator != null)
			{
				((StackIterator)stackIterator).RefreshItems(stackPad.Control.Width - 6,stackPad.Control.Height - 1);
			}

			if(AlgorithmManager.Algorithms.CurrentAlgorithm != null)
			{
				if(inOrderTreeIterator != null)
				{
					for(IIterator iterator = inOrderTreeIterator.First();!inOrderTreeIterator.IsDone();iterator = inOrderTreeIterator.Next())
					{
						if(iterator.CurrentItem != null)
						{
							iterator.CurrentItem.BackColor = status.结点颜色;
							iterator.CurrentItem.Draw(g);
						}
					}
				}
				
				if(inOrderTreeIterator1 != null)
				{
					for(IIterator iterator = inOrderTreeIterator1.First();!inOrderTreeIterator1.IsDone();iterator = inOrderTreeIterator1.Next())
					{
						if(iterator.CurrentItem != null)
						{
							iterator.CurrentItem.Draw(g);
						}
					}
				}

				if(currentIterator != null)
				{
					if(currentIterator.CurrentItem != null)
					{
						if(visiting == false)
						{
							currentIterator.CurrentItem.BackColor = status.当前结点颜色;
						}
						else
						{
							currentIterator.CurrentItem.BackColor = status.输出结点颜色;
						}
						currentIterator.CurrentItem.Draw(g);
					}
				}
				if(stackIterator != null)
				{
					for(IIterator iterator = stackIterator.First();!stackIterator.IsDone();iterator = stackIterator.Next())
					{
						if(iterator.CurrentItem != null)
						{
							iterator.CurrentItem.Draw(g1);
						}
					}
				}
				if(visitedIterator != null)
				{
					for(IIterator iterator = visitedIterator.First();!visitedIterator.IsDone();iterator = visitedIterator.Next())
					{
						if(iterator.CurrentItem != null)
						{
							iterator.CurrentItem.Draw(g);
						}
					}
				}
			}
		}


	}
}
