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
	/// <summary>
	/// SequenceMergeDialog 的摘要说明。
	/// </summary>
	public class SequenceMergeDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtString2;
		private System.Windows.Forms.TextBox txtString1;
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		private string s1,s2;

		public SequenceMergeDialog()
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
			this.txtString2 = new System.Windows.Forms.TextBox();
			this.txtString1 = new System.Windows.Forms.TextBox();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txtString2
			// 
			this.txtString2.Location = new System.Drawing.Point(104, 56);
			this.txtString2.MaxLength = 9;
			this.txtString2.Name = "txtString2";
			this.txtString2.Size = new System.Drawing.Size(160, 21);
			this.txtString2.TabIndex = 25;
			this.txtString2.Text = "";
			// 
			// txtString1
			// 
			this.txtString1.Location = new System.Drawing.Point(104, 16);
			this.txtString1.MaxLength = 9;
			this.txtString1.Name = "txtString1";
			this.txtString1.Size = new System.Drawing.Size(160, 21);
			this.txtString1.TabIndex = 21;
			this.txtString1.Text = "";
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(24, 96);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(80, 23);
			this.btnSave.TabIndex = 24;
			this.btnSave.Text = "确定并保存";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(232, 96);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(48, 23);
			this.btnCancel.TabIndex = 23;
			this.btnCancel.Text = "取 消";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(144, 96);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(48, 23);
			this.btnOK.TabIndex = 22;
			this.btnOK.Text = "确 定";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 23);
			this.label2.TabIndex = 20;
			this.label2.Text = "有序串2";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 23);
			this.label1.TabIndex = 19;
			this.label1.Text = "有序串1";
			// 
			// SequenceMergeDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(306, 135);
			this.Controls.Add(this.txtString2);
			this.Controls.Add(this.txtString1);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SequenceMergeDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "有序顺序表合并";
			this.ResumeLayout(false);

		}
		#endregion
		
		private bool CheckAvailable()
		{
			s1 = this.txtString1.Text.Trim();
			s2 = this.txtString2.Text.Trim();

			if(s1 == "" || s2 == "")
			{
				MessageBox.Show("请输入完整的初始化数据！","警告",MessageBoxButtons.OK,MessageBoxIcon.Warning);
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
					algorithm.Status = new SequenceMergeStatus(s1,s2);
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
				if(el.Attributes["name"].Value == typeof(SequenceMerge).ToString())
				{
					parentNode = el.ChildNodes[0];
					break;
				}
			}
			if(parentNode != null)
			{
				XmlElement childNode = doc.CreateElement("Data");

				childNode.SetAttribute("String1",s1);
				childNode.SetAttribute("String2",s2);

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
