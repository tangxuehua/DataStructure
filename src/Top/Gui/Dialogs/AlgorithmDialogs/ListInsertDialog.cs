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
using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.TextEditor;
using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.Gui.Pads;
using NetFocus.DataStructure.Internal.Algorithm;

namespace NetFocus.DataStructure.Gui.Algorithm.Dialogs
{
	public class ListInsertDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TextBox txtSourceString;
		private System.Windows.Forms.TextBox txtInsertPosition;
		private System.Windows.Forms.TextBox txtInsertChar;
		private System.Windows.Forms.Button btnSave;

		string s,p,c;

		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ListInsertDialog()
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.txtSourceString = new System.Windows.Forms.TextBox();
			this.txtInsertPosition = new System.Windows.Forms.TextBox();
			this.txtInsertChar = new System.Windows.Forms.TextBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "源字符串";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 23);
			this.label2.TabIndex = 1;
			this.label2.Text = "插入位置";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(24, 104);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(72, 23);
			this.label3.TabIndex = 2;
			this.label3.Text = "插入元素";
			// 
			// txtSourceString
			// 
			this.txtSourceString.Location = new System.Drawing.Point(112, 16);
			this.txtSourceString.MaxLength = 9;
			this.txtSourceString.Name = "txtSourceString";
			this.txtSourceString.Size = new System.Drawing.Size(160, 21);
			this.txtSourceString.TabIndex = 3;
			this.txtSourceString.Text = "";
			// 
			// txtInsertPosition
			// 
			this.txtInsertPosition.Location = new System.Drawing.Point(112, 56);
			this.txtInsertPosition.MaxLength = 2;
			this.txtInsertPosition.Name = "txtInsertPosition";
			this.txtInsertPosition.TabIndex = 4;
			this.txtInsertPosition.Text = "";
			// 
			// txtInsertChar
			// 
			this.txtInsertChar.Location = new System.Drawing.Point(112, 96);
			this.txtInsertChar.MaxLength = 1;
			this.txtInsertChar.Name = "txtInsertChar";
			this.txtInsertChar.TabIndex = 5;
			this.txtInsertChar.Text = "";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(144, 136);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(48, 23);
			this.btnOK.TabIndex = 6;
			this.btnOK.Text = "确 定";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(232, 136);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(48, 23);
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "取 消";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(24, 136);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(80, 23);
			this.btnSave.TabIndex = 8;
			this.btnSave.Text = "确定并保存";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// ListInsertDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(306, 173);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.txtInsertChar);
			this.Controls.Add(this.txtInsertPosition);
			this.Controls.Add(this.txtSourceString);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ListInsertDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "链表插入";
			this.ResumeLayout(false);

		}
		#endregion

		private bool IsNumeric(string s)
		{
			if(s.Trim().Length == 0)
			{
				return false;
			}
			if(s.Length == 1)
			{
				if(Char.IsDigit(s[0]) == false)
				{
					return false;
				}
			}
			if(s.Length == 2)
			{
				if(Char.IsDigit(s[0]) == false)
				{
					return false;
				}
				if(Char.IsDigit(s[1]) == false)
				{
					return false;
				}
			}
			return true;
		}
		private bool IsRightPosition(string s,int pos)
		{
			if(pos > s.Length + 1 || pos < 1)
			{
				return false;
			}
			return true;
		}
		private bool CheckAvailable()
		{
			s = this.txtSourceString.Text.Trim();
			p = this.txtInsertPosition.Text.Trim();
			c = this.txtInsertChar.Text.Trim();

			if(s == "" || p == "" || c == "")
			{
				MessageBox.Show("请输入完整的初始化数据！","警告",MessageBoxButtons.OK,MessageBoxIcon.Warning);
				return false;
			}
			if(IsNumeric(p) == false)
			{
				MessageBox.Show("插入位置必须为一个整数！","警告",MessageBoxButtons.OK,MessageBoxIcon.Warning);
				return false;
			}
			if(IsRightPosition(s,Int32.Parse(p)) == false)
			{
				MessageBox.Show("插入位置不合法，必须在【1,Length+1】之间","警告",MessageBoxButtons.OK,MessageBoxIcon.Warning);
				return false;
			}
			return true;

		}
		private void btnOK_Click(object sender, System.EventArgs e)
		{
			if(CheckAvailable() == true)
			{
				IAlgorithm algorithm = AlgorithmManager.Algorithms.CurrentAlgorithm;
			
				if(algorithm != null)
				{
					algorithm.Status = new ListInsertStatus(s,Int32.Parse(p),Char.Parse(c));
				}
			
				this.DialogResult = DialogResult.OK;

				this.Close();
			}
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;

			this.Close();
		}

		
		private void SaveStatus()
		{
			XmlDocument doc = new XmlDocument();

			doc.Load(AlgorithmManager.Algorithms.AlgorithmExampleDataFile);
			XmlNode parentNode = null;
			XmlNodeList nodes  = doc.DocumentElement.ChildNodes;
			foreach (XmlElement el in nodes)
			{
				if(el.Attributes["name"].Value == typeof(ListInsert).ToString())
				{
					parentNode = el.ChildNodes[0];
					break;
				}
			}
			if(parentNode != null)
			{
				XmlElement childNode = doc.CreateElement("Data");

				childNode.SetAttribute("OriginalString",s);
				childNode.SetAttribute("InsertPosition",p);
				childNode.SetAttribute("InsertData",c);

				parentNode.AppendChild(childNode);
			}
				
			doc.Save(AlgorithmManager.Algorithms.AlgorithmExampleDataFile);

		}
		
		
		private void btnSave_Click(object sender, System.EventArgs e)
		{
			if(CheckAvailable() == true)
			{
				SaveStatus();

				btnOK_Click(null,null);

			}
		}

	}
}
