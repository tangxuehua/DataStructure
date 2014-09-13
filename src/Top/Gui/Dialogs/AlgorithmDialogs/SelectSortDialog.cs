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
	public class SelectSortDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txtString1;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label label1;
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		string s;

		public SelectSortDialog()
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
			this.txtString1 = new System.Windows.Forms.TextBox();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txtString1
			// 
			this.txtString1.Location = new System.Drawing.Point(104, 24);
			this.txtString1.MaxLength = 14;
			this.txtString1.Name = "txtString1";
			this.txtString1.Size = new System.Drawing.Size(160, 21);
			this.txtString1.TabIndex = 38;
			this.txtString1.Text = "";
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(24, 72);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(80, 23);
			this.btnSave.TabIndex = 41;
			this.btnSave.Text = "确定并保存";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(232, 72);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(48, 23);
			this.btnCancel.TabIndex = 40;
			this.btnCancel.Text = "取 消";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(144, 72);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(48, 23);
			this.btnOK.TabIndex = 39;
			this.btnOK.Text = "确 定";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 23);
			this.label1.TabIndex = 37;
			this.label1.Text = "待排序的串";
			// 
			// SelectSortDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(306, 112);
			this.Controls.Add(this.txtString1);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SelectSortDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "选择排序";
			this.ResumeLayout(false);

		}
		#endregion

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			s = this.txtString1.Text.Trim();

			if(s.Length == 0)
			{
				MessageBox.Show("请输入完整的初始化数据！","警告",MessageBoxButtons.OK,MessageBoxIcon.Warning);
				return;
			}

			IAlgorithm algorithm = AlgorithmManager.Algorithms.CurrentAlgorithm;
			
			if(algorithm != null)
			{
				algorithm.Status = new SelectSortStatus(s);
			}
			
			this.DialogResult = DialogResult.OK;

			this.Close();

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
				if(el.Attributes["name"].Value == typeof(SelectSort).ToString())
				{
					parentNode = el.ChildNodes[0];
					break;
				}
			}
			if(parentNode != null)
			{
				XmlElement childNode = doc.CreateElement("Data");

				childNode.SetAttribute("OriginalString",s);

				parentNode.AppendChild(childNode);
			}
				
			doc.Save(AlgorithmManager.Algorithms.AlgorithmExampleDataFile);

		}
		
		
		private void btnSave_Click(object sender, System.EventArgs e)
		{
			s = this.txtString1.Text.Trim();

			if(s.Length == 0)
			{
				MessageBox.Show("请输入完整的初始化数据！","警告",MessageBoxButtons.OK,MessageBoxIcon.Warning);
				return;
			}

			SaveStatus();
			btnOK_Click(null,null);	
		}
	}
}
