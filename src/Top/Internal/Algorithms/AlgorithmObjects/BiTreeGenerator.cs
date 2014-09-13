using System;
using System.Text;
using System.Drawing;
using System.Xml;
using NetFocus.DataStructure.Internal.Algorithm.Glyphs;


namespace NetFocus.DataStructure.Internal.Algorithm
{
	public class BiTreeGenerator
	{
        XmlNode dataNode = null;
		bool isPreview = false;
		Random randomNumberGenerator = new Random();
		bool[,] flagArray = new bool[7,15];
		char[,] charArray = new char[7,15];
		Point[,] pointArray = new Point[7,15];
		int leftSpan = 10;
		int topSpan = 5;
		int colWidth = 34;
		int rowHeight = 45;
		Color lineColor = Color.Red;

		IBiTreeNode rootLineNode = null;
		IBiTreeNode rootNode = null;
		bool isFullTree = false;

		public bool[,] FlagArray
		{
			get
			{
				return flagArray;
			}
		}

		public char[,] CharArray
		{
			get
			{
				return charArray;
			}
		}

		public bool IsPreview
		{
			get
			{
				return isPreview;
			}
			set
			{
				isPreview = value;
			}
		}
		public bool IsFullTree
		{
			get
			{
				return isFullTree;
			}
			set
			{
				isFullTree = value;
			}
		}
		public XmlNode DataNode
		{
			get
			{
				return dataNode;
			}
			set
			{
				dataNode = value;
			}
		}
		

		public IBiTreeNode RootLineNode
		{
			get
			{
				return rootLineNode;
			}
		}


		public IBiTreeNode RootNode
		{
			get
			{
				return rootNode;
			}
		}

		//确定是否要在那个位置产生值
		bool HaveValue()
		{
			if(isFullTree == false)
			{
				if(randomNumberGenerator.NextDouble() > 0.2)  //为了让有值的机会大点，所以写以大于0.2就算有值
				{
					return true;
				}
				return false;
			}
			return true;
		}
		//确定要长生几层的二叉树结构
		int GetLevel()
		{
			if(isFullTree == false)
			{
				int n = randomNumberGenerator.Next(1,10);
				if(n >= 4)  //为了让出现4层的机会大点
				{
					return 4;
				}
				return n;
			}
			return 4;
		}
		//随即产生一个字母
		char GenerateChar()
		{
			Byte[] bytes = new Byte[1];
			Char[] chars = new Char[1];
			ASCIIEncoding ascii = new ASCIIEncoding();
			//产生一个从字母a到字母z中间的任何一个字符
			bytes[0] = (Byte)(randomNumberGenerator.Next(97,123));

			ascii.GetChars(bytes, 0, 1, chars, 0);

			return chars[0];
			
		}
		void AssignValues(int rowIndex,int col1,int col2)
		{
			flagArray[rowIndex,col1] = HaveValue();
			if(flagArray[rowIndex,col1] == true)
			{
				charArray[rowIndex,col1] = GenerateChar();
			}

			flagArray[rowIndex,col2] = HaveValue();
			if(flagArray[rowIndex,col2] == true)
			{
				charArray[rowIndex,col2] = GenerateChar();
			}
		}

		void InitTree()
		{
			for(int i = 0;i < 7;i++)  //initialize the charArray with char ' ',and the flagArray with false
			{
				for(int j = 0;j < 15;j++)
				{
					charArray[i,j] = ' ';
					flagArray[i,j] = false;
					pointArray[i,j] = new Point(leftSpan + j * colWidth,topSpan + i * rowHeight);
				}
			}
			flagArray[0,7] = true;
			charArray[0,7] = GenerateChar(); //根结点必须产生

		}

		IBiTreeNode CreateBiTreeNode(int rowIndex,int colIndex,int diameter,Color backColor)
		{
			int x,y;
			string text;

			x = ((Point)pointArray[rowIndex,colIndex]).X;
			y = ((Point)pointArray[rowIndex,colIndex]).Y;
			text = charArray[rowIndex,colIndex].ToString();

			return new BiTreeNode(x,y,diameter,backColor,text);
		}
		
		BiTreeLineNode CreateLineNode(IBiTreeNode treeNode1,IBiTreeNode treeNode2)
		{
			int x1,y1,x2,y2;
			if(treeNode1 == treeNode2)  //如果是根结点,就不画出来
			{
				return new BiTreeLineNode(1,1,1,1,1,Color.Transparent);
			}
			x1 = treeNode1.Bounds.X + treeNode1.Bounds.Width / 2;
			x2 = treeNode2.Bounds.X + treeNode2.Bounds.Width / 2;
			y1 = treeNode1.Bounds.Y + treeNode1.Bounds.Height;
			y2 = treeNode2.Bounds.Y;

			return new BiTreeLineNode(x1,y1,x2,y2,2,lineColor);

		}
		void AssignArrayValues()
		{
			InitTree();

			int level = GetLevel();  //确定二叉树的深度

			switch (level)
			{
				case 2:
					AssignValues(2,3,11);
					break;
				case 3:
					AssignValues(2,3,11);
					if(flagArray[2,3] == true)
					{
						AssignValues(4,1,5);
					}
					if(flagArray[2,11] == true)
					{
						AssignValues(4,9,13);
					}
					break;
				case 4:
					AssignValues(2,3,11);
					if(flagArray[2,3] == true)
					{
						AssignValues(4,1,5);
					}
					if(flagArray[2,11] == true)
					{
						AssignValues(4,9,13);
					}
					if(flagArray[4,1] == true)
					{
						AssignValues(6,0,2);
					}
					if(flagArray[4,5] == true)
					{
						AssignValues(6,4,6);
					}
					if(flagArray[4,9] == true)
					{
						AssignValues(6,8,10);
					}
					if(flagArray[4,13] == true)
					{
						AssignValues(6,12,14);
					}
					break;
			}
		}
		
		void AssignArrayValues1()
		{
			if(isPreview == true)
			{
				leftSpan = 20;
				topSpan = 5;
				colWidth = 30;
				rowHeight = 30;
			}

			InitTree();

			foreach (XmlNode el in dataNode.ChildNodes)
			{
				int i,j;
				char c;
				j = Convert.ToInt32(el.Attributes["X"].Value);
				i = Convert.ToInt32(el.Attributes["Y"].Value);
				c = el.Attributes["Value"].Value[0];
				charArray[i,j] = c;
				flagArray[i,j] = true;
				pointArray[i,j] = new Point(leftSpan + j * colWidth,topSpan + i * rowHeight);

			}

		}
		public void GenerateTree(int diameter,Color backColor)
		{
			if(dataNode == null)
			{
				AssignArrayValues();
			}
			else
			{
				AssignArrayValues1();
			}
			//先创建根结点（在二维数组中的位置为[0,7]）
			rootNode = CreateBiTreeNode(0,7,diameter,backColor);
			IBiTreeNode currentNode;
			IBiTreeNode parentNode = rootNode;
			IBiTreeNode tempParentNode0 = rootNode;  //用来暂时保存根结点

			rootLineNode = CreateLineNode(rootNode,rootNode);
			IBiTreeNode currentLineNode;
			IBiTreeNode parentLineNode = rootLineNode;
			IBiTreeNode tempParentLineNode0 = rootLineNode;  //用来暂时保存根结点

			if(flagArray[2,3] == true)
			{
				currentNode = CreateBiTreeNode(2,3,diameter,backColor);
				currentLineNode = CreateLineNode(parentNode,currentNode);

				parentNode.LeftChild = currentNode;
				parentNode = currentNode;
				IBiTreeNode tempParentNode1 = currentNode;
				
				parentLineNode.LeftChild = currentLineNode;
				parentLineNode = currentLineNode;
				IBiTreeNode tempParentLineNode1 = currentLineNode;  //同样用来保存某个结点

				if(flagArray[4,1] == true)
				{
					currentNode = CreateBiTreeNode(4,1,diameter,backColor);
					currentLineNode = CreateLineNode(parentNode,currentNode);

					parentNode.LeftChild = currentNode;
					parentNode = currentNode;

					parentLineNode.LeftChild = currentLineNode;
					parentLineNode = currentLineNode;

					if(flagArray[6,0] == true)
					{
						currentNode = CreateBiTreeNode(6,0,diameter,backColor);
						currentLineNode = CreateLineNode(parentNode,currentNode);

						parentNode.LeftChild = currentNode;
						parentLineNode.LeftChild = currentLineNode;
					}
					if(flagArray[6,2] == true)
					{
						currentNode = CreateBiTreeNode(6,2,diameter,backColor);
						currentLineNode = CreateLineNode(parentNode,currentNode);

						parentNode.RightChild = currentNode;
						parentLineNode.RightChild = currentLineNode;
					}

				}
				if(flagArray[4,5] == true)
				{
					currentNode = CreateBiTreeNode(4,5,diameter,backColor);

					parentNode = tempParentNode1;
					parentLineNode = tempParentLineNode1;

					currentLineNode = CreateLineNode(parentNode,currentNode);
					
					parentNode.RightChild = currentNode;
					parentNode = currentNode;

					parentLineNode.RightChild = currentLineNode;
					parentLineNode = currentLineNode;

					if(flagArray[6,4] == true)
					{
						currentNode = CreateBiTreeNode(6,4,diameter,backColor);
						currentLineNode = CreateLineNode(parentNode,currentNode);

						parentNode.LeftChild = currentNode;
						parentLineNode.LeftChild = currentLineNode;
					}
					if(flagArray[6,6] == true)
					{
						currentNode = CreateBiTreeNode(6,6,diameter,backColor);
						currentLineNode = CreateLineNode(parentNode,currentNode);

						parentNode.RightChild = currentNode;
						parentLineNode.RightChild = currentLineNode;
					}

				}
			}

			if(flagArray[2,11] == true)
			{
				currentNode = CreateBiTreeNode(2,11,diameter,backColor);

				parentNode = tempParentNode0;
				parentLineNode = tempParentLineNode0;

				currentLineNode = CreateLineNode(parentNode,currentNode);

				parentNode.RightChild = currentNode;
				parentLineNode.RightChild = currentLineNode;

				parentNode = currentNode;
				parentLineNode = currentLineNode;

				IBiTreeNode tempParentNode2 = currentNode;  //同样用来保存某个结点
				IBiTreeNode tempParentLineNode2 = currentLineNode;  //同样用来保存某个结点

				if(flagArray[4,9] == true)
				{
					currentNode = CreateBiTreeNode(4,9,diameter,backColor);
					currentLineNode = CreateLineNode(parentNode,currentNode);

					parentNode.LeftChild = currentNode;
					parentLineNode.LeftChild = currentLineNode;

					parentNode = currentNode;
					parentLineNode = currentLineNode;

					if(flagArray[6,8] == true)
					{
						currentNode = CreateBiTreeNode(6,8,diameter,backColor);
						currentLineNode = CreateLineNode(parentNode,currentNode);

						parentNode.LeftChild = currentNode;
						parentLineNode.LeftChild = currentLineNode;
					}
					if(flagArray[6,10] == true)
					{
						currentNode = CreateBiTreeNode(6,10,diameter,backColor);
						currentLineNode = CreateLineNode(parentNode,currentNode);
						
						parentNode.RightChild = currentNode;
						parentLineNode.RightChild = currentLineNode;
					}

				}
				if(flagArray[4,13] == true)
				{
					currentNode = CreateBiTreeNode(4,13,diameter,backColor);

					parentNode = tempParentNode2;
					parentLineNode = tempParentLineNode2;

					currentLineNode = CreateLineNode(parentNode,currentNode);

					parentNode.RightChild = currentNode;
					parentNode = currentNode;

					parentLineNode.RightChild = currentLineNode;
					parentLineNode = currentLineNode;

					if(flagArray[6,12] == true)
					{
						currentNode = CreateBiTreeNode(6,12,diameter,backColor);
						currentLineNode = CreateLineNode(parentNode,currentNode);

						parentNode.LeftChild = currentNode;
						parentLineNode.LeftChild = currentLineNode;
					}
					if(flagArray[6,14] == true)
					{
						currentNode = CreateBiTreeNode(6,14,diameter,backColor);
						currentLineNode = CreateLineNode(parentNode,currentNode);

						parentNode.RightChild = currentNode;
						parentLineNode.RightChild = currentLineNode;
					}

				}
				
			}

			for(int i = 0;i < 7;i++)
			{
				for(int j = 0;j < 15;j++)
				{
					Console.Write(charArray[i,j] + " ");
				}
				Console.WriteLine();
			}
		}


		public void GenerateTree1(int diameter,Color backColor)
		{
			leftSpan = 20;
			topSpan = 5;
			colWidth = 30;
			rowHeight = 30;

			AssignArrayValues();

			//先创建根结点（在二维数组中的位置为[0,7]）
			rootNode = CreateBiTreeNode(0,7,diameter,backColor);
			IBiTreeNode currentNode;
			IBiTreeNode parentNode = rootNode;
			IBiTreeNode tempParentNode0 = rootNode;  //用来暂时保存根结点

			rootLineNode = CreateLineNode(rootNode,rootNode);
			IBiTreeNode currentLineNode;
			IBiTreeNode parentLineNode = rootLineNode;
			IBiTreeNode tempParentLineNode0 = rootLineNode;  //用来暂时保存根结点

			if(flagArray[2,3] == true)
			{
				currentNode = CreateBiTreeNode(2,3,diameter,backColor);
				currentLineNode = CreateLineNode(parentNode,currentNode);

				parentNode.LeftChild = currentNode;
				parentNode = currentNode;
				IBiTreeNode tempParentNode1 = currentNode;
				
				parentLineNode.LeftChild = currentLineNode;
				parentLineNode = currentLineNode;
				IBiTreeNode tempParentLineNode1 = currentLineNode;  //同样用来保存某个结点

				if(flagArray[4,1] == true)
				{
					currentNode = CreateBiTreeNode(4,1,diameter,backColor);
					currentLineNode = CreateLineNode(parentNode,currentNode);

					parentNode.LeftChild = currentNode;
					parentNode = currentNode;

					parentLineNode.LeftChild = currentLineNode;
					parentLineNode = currentLineNode;

					if(flagArray[6,0] == true)
					{
						currentNode = CreateBiTreeNode(6,0,diameter,backColor);
						currentLineNode = CreateLineNode(parentNode,currentNode);

						parentNode.LeftChild = currentNode;
						parentLineNode.LeftChild = currentLineNode;
					}
					if(flagArray[6,2] == true)
					{
						currentNode = CreateBiTreeNode(6,2,diameter,backColor);
						currentLineNode = CreateLineNode(parentNode,currentNode);

						parentNode.RightChild = currentNode;
						parentLineNode.RightChild = currentLineNode;
					}

				}
				if(flagArray[4,5] == true)
				{
					currentNode = CreateBiTreeNode(4,5,diameter,backColor);

					parentNode = tempParentNode1;
					parentLineNode = tempParentLineNode1;

					currentLineNode = CreateLineNode(parentNode,currentNode);
					
					parentNode.RightChild = currentNode;
					parentNode = currentNode;

					parentLineNode.RightChild = currentLineNode;
					parentLineNode = currentLineNode;

					if(flagArray[6,4] == true)
					{
						currentNode = CreateBiTreeNode(6,4,diameter,backColor);
						currentLineNode = CreateLineNode(parentNode,currentNode);

						parentNode.LeftChild = currentNode;
						parentLineNode.LeftChild = currentLineNode;
					}
					if(flagArray[6,6] == true)
					{
						currentNode = CreateBiTreeNode(6,6,diameter,backColor);
						currentLineNode = CreateLineNode(parentNode,currentNode);

						parentNode.RightChild = currentNode;
						parentLineNode.RightChild = currentLineNode;
					}

				}
			}

			if(flagArray[2,11] == true)
			{
				currentNode = CreateBiTreeNode(2,11,diameter,backColor);

				parentNode = tempParentNode0;
				parentLineNode = tempParentLineNode0;

				currentLineNode = CreateLineNode(parentNode,currentNode);

				parentNode.RightChild = currentNode;
				parentLineNode.RightChild = currentLineNode;

				parentNode = currentNode;
				parentLineNode = currentLineNode;

				IBiTreeNode tempParentNode2 = currentNode;  //同样用来保存某个结点
				IBiTreeNode tempParentLineNode2 = currentLineNode;  //同样用来保存某个结点

				if(flagArray[4,9] == true)
				{
					currentNode = CreateBiTreeNode(4,9,diameter,backColor);
					currentLineNode = CreateLineNode(parentNode,currentNode);

					parentNode.LeftChild = currentNode;
					parentLineNode.LeftChild = currentLineNode;

					parentNode = currentNode;
					parentLineNode = currentLineNode;

					if(flagArray[6,8] == true)
					{
						currentNode = CreateBiTreeNode(6,8,diameter,backColor);
						currentLineNode = CreateLineNode(parentNode,currentNode);

						parentNode.LeftChild = currentNode;
						parentLineNode.LeftChild = currentLineNode;
					}
					if(flagArray[6,10] == true)
					{
						currentNode = CreateBiTreeNode(6,10,diameter,backColor);
						currentLineNode = CreateLineNode(parentNode,currentNode);
						
						parentNode.RightChild = currentNode;
						parentLineNode.RightChild = currentLineNode;
					}

				}
				if(flagArray[4,13] == true)
				{
					currentNode = CreateBiTreeNode(4,13,diameter,backColor);

					parentNode = tempParentNode2;
					parentLineNode = tempParentLineNode2;

					currentLineNode = CreateLineNode(parentNode,currentNode);

					parentNode.RightChild = currentNode;
					parentNode = currentNode;

					parentLineNode.RightChild = currentLineNode;
					parentLineNode = currentLineNode;

					if(flagArray[6,12] == true)
					{
						currentNode = CreateBiTreeNode(6,12,diameter,backColor);
						currentLineNode = CreateLineNode(parentNode,currentNode);

						parentNode.LeftChild = currentNode;
						parentLineNode.LeftChild = currentLineNode;
					}
					if(flagArray[6,14] == true)
					{
						currentNode = CreateBiTreeNode(6,14,diameter,backColor);
						currentLineNode = CreateLineNode(parentNode,currentNode);

						parentNode.RightChild = currentNode;
						parentLineNode.RightChild = currentLineNode;
					}

				}
				
			}

			for(int i = 0;i < 7;i++)
			{
				for(int j = 0;j < 15;j++)
				{
					Console.Write(charArray[i,j] + " ");
				}
				Console.WriteLine();
			}
		}


	}
}
