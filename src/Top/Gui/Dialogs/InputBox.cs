
using System;
using System.Windows.Forms;
using System.IO;
using NetFocus.DataStructure.Gui.XmlForms;

namespace NetFocus.DataStructure.Gui.Dialogs {
	
	public class InputBox : Form
	{

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;

		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(32, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(280, 23);
			this.label1.TabIndex = 0;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(32, 48);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(280, 21);
			this.textBox1.TabIndex = 1;
			this.textBox1.Text = "";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(136, 88);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(72, 24);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "确  定";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(224, 88);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(72, 24);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "取  消";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// InputBox
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(344, 134);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InputBox";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.ResumeLayout(false);

		}


		public InputBox()
		{
			InitializeComponent();
		}

		public Label Label
		{
			get
			{
				return this.label1;
			}

		}
		public TextBox TextBox
		{
			get
			{
				return this.textBox1;
			}
		}
		private void btnOK_Click(object sender, System.EventArgs e)
		{
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
