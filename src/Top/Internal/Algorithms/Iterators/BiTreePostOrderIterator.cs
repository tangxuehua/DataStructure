using System;
using System.Collections;
using System.Drawing;

namespace NetFocus.DataStructure.Internal.Algorithm.Glyphs
{
	/// <summary>
	/// 针对二叉树数据结构后序遍历的迭代器
	/// </summary>
	public class BiTreePostOrderIterator : IIterator
	{
		IBiTreeNode rootNode = null;  //前序遍历的根结点
		IBiTreeNode currentNode = null;  //遍历时的当前结点
		IBiTreeNode currentBackupNode = null;//用来保存刚刚遍历的结点
		Stack nodesStack = new Stack();

		public Stack NodesStack
		{
			get
			{
				return nodesStack;
			}
		}

		public BiTreePostOrderIterator(IBiTreeNode rootNode)
		{
			this.rootNode = rootNode;
		}

		#region IIterator 成员
		
		public IIterator First()
		{
			nodesStack.Clear();
			currentBackupNode = null; //注意,这里不能忘了,第一次不用保存刚刚被遍历的结点
			currentNode = rootNode;  //先指向根结点
			nodesStack.Push(currentNode);

			while(nodesStack.Count > 0)
			{
				if(currentNode != null && currentNode != currentBackupNode)  //当前结点不空并且没有被访问过
				{
					nodesStack.Push(currentNode);
					currentNode = currentNode.LeftChild;
				}
				else
				{	
					currentNode = nodesStack.Pop() as IBiTreeNode;  //当前currentNode保存最左边结点

					if(currentNode.RightChild != null && currentNode.RightChild != currentBackupNode)  //当前结点的右结点不空并且没有被访问过
					{
						nodesStack.Push(currentNode);
						currentNode = currentNode.RightChild;
					}
					else
					{
						currentBackupNode = currentNode;
						break;
					}
				}
			}
			
			return this;
		}

		public IIterator Next()
		{
			while(nodesStack.Count > 0)
			{
				if(currentNode != null && currentNode != currentBackupNode)  //当前结点不空并且没有被访问过
				{
					nodesStack.Push(currentNode);
					currentNode = currentNode.LeftChild;
				}
				else
				{	
					currentNode = nodesStack.Pop() as IBiTreeNode;  //当前currentNode保存最左边结点

					if(currentNode.RightChild != null && currentNode.RightChild != currentBackupNode)  //当前结点的右结点不空并且没有被访问过
					{
						nodesStack.Push(currentNode);
						currentNode = currentNode.RightChild;
					}
					else
					{
						currentBackupNode = currentNode;
						break;
					}
				}
			}

			return this;
		}

		public bool IsDone()
		{
			return nodesStack.Count == 0;
		}

		public IGlyph CurrentItem
		{
			get
			{
				return currentNode;
			}
		}

		
		#endregion

		public void SetToRootNode()
		{
			currentNode = rootNode;
		}

		public void SetToLeftChild()
		{
			currentNode = currentNode.LeftChild;
		}
		public void SetToRightChild()
		{
			currentNode = currentNode.RightChild;
		}
		public void PopupToCurrentNode()
		{
			currentNode = (IBiTreeNode)nodesStack.Pop();
		}
		public void PushCurrentNode()
		{
			nodesStack.Push(currentNode);
		}
		public void SetToNewNode(IBiTreeNode newNode)
		{
			currentNode = newNode;
		}

	}
}