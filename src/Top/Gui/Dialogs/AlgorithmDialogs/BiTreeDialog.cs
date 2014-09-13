using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Threading;
using System.Globalization;
using System.Xml;

using NetFocus.DataStructure.Gui.Views;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.TextEditor;
using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.Gui.Pads;
using NetFocus.DataStructure.Internal.Algorithm;
using NetFocus.DataStructure.Internal.Algorithm.Glyphs;

namespace NetFocus.DataStructure.Gui.Algorithm.Dialogs
{
	public class BiTreeDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnRadomTree;
		private System.Windows.Forms.Button btnFullTree;
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		XmlElement childNode = null;//用于保存随机生成的二叉树所对应的Xml结点
		BiTreeGenerator biTreeGenerator = new BiTreeGenerator(); //用于随机生成二叉树

		public BiTreeDialog()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
		}

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnRadomTree = new System.Windows.Forms.Button();
			this.btnFullTree = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.Color.White;
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(524, 248);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(224, 264);
			this.btnSave.Name = "btnSave";
			this.btnSave.TabIndex = 1;
			this.btnSave.Text = "确定并保存";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(320, 264);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "确 定";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(416, 264);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "取 消";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnRadomTree
			// 
			this.btnRadomTree.Location = new System.Drawing.Point(128, 264);
			this.btnRadomTree.Name = "btnRadomTree";
			this.btnRadomTree.TabIndex = 4;
			this.btnRadomTree.Text = "随机生成";
			this.btnRadomTree.Click += new System.EventHandler(this.btnRadomTree_Click);
			// 
			// btnFullTree
			// 
			this.btnFullTree.Location = new System.Drawing.Point(32, 264);
			this.btnFullTree.Name = "btnFullTree";
			this.btnFullTree.TabIndex = 5;
			this.btnFullTree.Text = "满 树";
			this.btnFullTree.Click += new System.EventHandler(this.btnFullTree_Click);
			// 
			// BiTreeDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(524, 304);
			this.Controls.Add(this.btnFullTree);
			this.Controls.Add(this.btnRadomTree);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BiTreeDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "随即生成二叉树";
			this.ResumeLayout(false);

		}
		#endregion

		//生成一棵二叉树,并把生成的位图返回
		Bitmap GenerateTree(bool isFullTree)
		{
			int height = 240;
			int width = 530;
			int diameter = 40;
			
			biTreeGenerator.IsFullTree = isFullTree;
			biTreeGenerator.GenerateTree1(diameter,Color.HotPink);

			Bitmap bmp = new Bitmap(width,height);
			Graphics g = Graphics.FromImage(bmp);

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
		
		
		private void btnRadomTree_Click(object sender, System.EventArgs e)
		{
			this.pictureBox1.Image = GenerateTree(false);

		}


		private void btnFullTree_Click(object sender, System.EventArgs e)
		{
			this.pictureBox1.Image = GenerateTree(true);
		}

		
		private void btnOK_Click(object sender, System.EventArgs e)
		{
			if(this.pictureBox1.Image == null)
			{
				MessageBox.Show("请生成一个二叉树！","警告",MessageBoxButtons.OK,MessageBoxIcon.Warning);
				return;
			}
			IAlgorithm algorithm = AlgorithmManager.Algorithms.CurrentAlgorithm;
			
			if(algorithm != null)
			{
				SaveStatus(false);  //参数false表示只生成一个Xml结点,但并不保存到Xml文档中
				algorithm.Status = childNode;
			}
			
			this.DialogResult = DialogResult.OK;

			this.Close();
		}

		
		private void SaveStatus(bool flag)
		{
			XmlDocument doc = new XmlDocument();

			doc.Load(AlgorithmManager.Algorithms.AlgorithmExampleDataFile);
			
			XmlNodeList nodes  = doc.DocumentElement.ChildNodes;
			XmlNode parentNode = null;
			
			foreach (XmlElement el in nodes)
			{
				if(el.Attributes["name"].Value == AlgorithmManager.Algorithms.CurrentAlgorithm.GetType().ToString())
				{
					parentNode = el.ChildNodes[0];
					break;
				}
			}
			if(parentNode != null)  //就创建一个新的Data结点
			{
				childNode = doc.CreateElement("Data");

				bool[,] flagArray = biTreeGenerator.FlagArray;
				char[,] charArray = biTreeGenerator.CharArray;
				for(int i = 0;i < 7;i++)
				{
					for(int j = 0;j < 15;j++)
					{
						if(flagArray[i,j] != false)
						{
							XmlElement node = doc.CreateElement("Node");
							node.SetAttribute("X",j.ToString());
							node.SetAttribute("Y",i.ToString());
							node.SetAttribute("Value",charArray[i,j].ToString());
							childNode.AppendChild(node);
						}
					}
				}

				parentNode.AppendChild(childNode);
			}
			if(flag == true && parentNode != null)
			{
				doc.Save(AlgorithmManager.Algorithms.AlgorithmExampleDataFile);
			}
		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			if(this.pictureBox1.Image == null)
			{
				MessageBox.Show("请生成一个二叉树！","警告",MessageBoxButtons.OK,MessageBoxIcon.Warning);
				return;
			}
			IAlgorithm algorithm = AlgorithmManager.Algorithms.CurrentAlgorithm;
			
			if(algorithm != null)
			{
				SaveStatus(true);
				algorithm.Status = childNode;
			}
			
			this.DialogResult = DialogResult.OK;

			this.Close();
		}

		
		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;

			this.Close();
		}


	}
}
