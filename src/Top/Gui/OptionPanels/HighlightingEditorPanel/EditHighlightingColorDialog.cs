
using System;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using NetFocus.DataStructure.Gui.XmlForms;

using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Nodes;

namespace NetFocus.DataStructure.Gui.Dialogs {
	
	public class ColorButton : Button
	{			
		Color centerColor;
		
		public ColorButton()
		{		
		}
		
		public Color CenterColor
		{
			get { return centerColor; }
			set { centerColor = value; }
		}		
		
		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			Invalidate();
		}
		
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			Invalidate();
		}	
		
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			Invalidate();
		}
		
		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			Invalidate();
		}
		
		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
    		
			Point p = new Point(0, Height);
			p = PointToScreen(p);
			
			using (ColorPaletteDialog clDlg = new ColorPaletteDialog(p.X, p.Y)) 
			{
				clDlg.ShowDialog();    	 
				if (clDlg.DialogResult == DialogResult.OK) 
				{
					CenterColor = clDlg.Color;
				}
			}
	    		
		}  	    
		
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			
			Graphics g = e.Graphics;
			
			Rectangle r = ClientRectangle;	
			
			byte border = 4;
			byte right_border = 15;
			
			Rectangle rc = new Rectangle(r.Left + border, r.Top + border,
				r.Width - border - right_border - 1, r.Height - border * 2 - 1);
			
			SolidBrush centerColorBrush = new SolidBrush( centerColor );
			g.FillRectangle(centerColorBrush, rc);	
			
			Pen pen = Pens.Black;
			g.DrawRectangle(pen, rc);
			
			Pen greyPen = new Pen(SystemColors.ControlDark);
			
			//draw the arrow
			Point p1 = new Point(r.Width - 9, r.Height / 2 - 1);
			Point p2 = new Point(r.Width - 5, r.Height / 2 - 1);		
			g.DrawLine(Enabled ? pen : greyPen, p1, p2);
			
			p1 = new Point(r.Width - 8, r.Height / 2);
			p2 = new Point(r.Width - 6, r.Height / 2);		
			g.DrawLine(Enabled ? pen : greyPen, p1, p2);
			
			p1 = new Point(r.Width - 7, r.Height / 2);
			p2 = new Point(r.Width - 7, r.Height / 2 + 1);		
			g.DrawLine(Enabled ? pen : greyPen, p1, p2);
			
			//draw the divider line
			pen = new Pen(SystemColors.ControlDark); 
			p1 = new Point(r.Width - 12, 4);
			p2 = new Point(r.Width - 12, r.Height - 5 );		
			g.DrawLine(pen, p1, p2);
			
			pen = new Pen(SystemColors.ControlLightLight); 
			p1 = new Point(r.Width - 11, 4);
			p2 = new Point(r.Width - 11, r.Height - 5 );		
			g.DrawLine(pen, p1, p2);
		} 
	}

	
	public class EditHighlightingColorDialog : BaseXmlForm
	{
		private System.Windows.Forms.RadioButton foreNo;
		private System.Windows.Forms.RadioButton foreUser;
		private NetFocus.DataStructure.Gui.Dialogs.ColorButton backBtn;
		private System.Windows.Forms.RadioButton backSys;
		private System.Windows.Forms.Button acceptBtn;
		private NetFocus.DataStructure.Gui.Dialogs.ColorButton foreBtn;
		private System.Windows.Forms.RadioButton backNo;
		private System.Windows.Forms.ComboBox foreList;
		private System.Windows.Forms.CheckBox italicBox;
		private System.Windows.Forms.RadioButton foreSys;
		private System.Windows.Forms.RadioButton backUser;
		private System.Windows.Forms.CheckBox boldBox;
		private System.Windows.Forms.ComboBox backList;

		public EditorHighlightColor Color;
		
		public EditHighlightingColorDialog(EditorHighlightColor color)
		{
			SetupFromXmlFile(System.IO.Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\HighlightingEditor\ColorDialog.xfrm"));
			
			Color = color;
			
			boldBox   = (CheckBox)ControlDictionary["boldBox"];
			italicBox = (CheckBox)ControlDictionary["italicBox"];
			
			foreNo   = (RadioButton)ControlDictionary["foreNo"];
			foreUser = (RadioButton)ControlDictionary["foreUser"];
			foreSys  = (RadioButton)ControlDictionary["foreSys"];
			foreList = (ComboBox)ControlDictionary["foreList"];
			
			backNo   = (RadioButton)ControlDictionary["backNo"];
			backUser = (RadioButton)ControlDictionary["backUser"];
			backSys  = (RadioButton)ControlDictionary["backSys"];
			backList = (ComboBox)ControlDictionary["backList"];
			
			acceptBtn = (Button)ControlDictionary["acceptBtn"];
			
			this.foreBtn = new ColorButton();
			this.foreBtn.CenterColor = System.Drawing.Color.Empty;
			this.foreBtn.Enabled = false;
			this.foreBtn.Location = new System.Drawing.Point(30, 78);
			this.foreBtn.Name = "foreBtn";
			this.foreBtn.Size = new System.Drawing.Size(98, 24);
			
			this.ControlDictionary["foreBox"].Controls.Add(foreBtn);
			
			this.backBtn = new ColorButton();
			this.backBtn.CenterColor = System.Drawing.Color.Empty;
			this.backBtn.Enabled = false;
			this.backBtn.Location = new System.Drawing.Point(30, 78);
			this.backBtn.Name = "backBtn";
			this.backBtn.Size = new System.Drawing.Size(98, 24);
			
			this.ControlDictionary["backBox"].Controls.Add(backBtn);

			this.acceptBtn.Click += new EventHandler(AcceptClick);
			this.foreNo.CheckedChanged   += new EventHandler(foreCheck);
			this.foreSys.CheckedChanged  += new EventHandler(foreCheck);
			this.foreUser.CheckedChanged += new EventHandler(foreCheck);
			this.backNo.CheckedChanged   += new EventHandler(backCheck);
			this.backSys.CheckedChanged  += new EventHandler(backCheck);
			this.backUser.CheckedChanged += new EventHandler(backCheck);
			
			PropertyInfo[] names = typeof(System.Drawing.SystemColors).GetProperties(BindingFlags.Static | BindingFlags.Public);
			
			foreach(PropertyInfo info in names) {
				foreList.Items.Add(info.Name);
				backList.Items.Add(info.Name);
			}
			foreList.SelectedIndex = backList.SelectedIndex = 0;
			
			if (color.SysForeColor) {
				foreSys.Checked = true;
				for (int i = 0; i < foreList.Items.Count; ++i) {
					if ((string)foreList.Items[i] == color.SysForeColorName) foreList.SelectedIndex = i;
				}
			} else if (color.HasForeColor) {
				foreUser.Checked = true;
				foreBtn.CenterColor = color.ForeColor;
			} else {
				foreNo.Checked = true;
			}
			
			if (color.SysBackColor) {
				backSys.Checked = true;
				for (int i = 0; i < backList.Items.Count; ++i) {
					if ((string)backList.Items[i] == color.SysForeColorName) backList.SelectedIndex = i;
				}
			} else if (color.HasBackColor) {
				backUser.Checked = true;
				backBtn.CenterColor = color.BackColor;
			} else {
				backNo.Checked = true;
			}
			
			boldBox.Checked = color.Bold;
			italicBox.Checked = color.Italic;
		}
		
		void foreCheck(object sender, EventArgs e)
		{
			if (foreNo.Checked) {
				foreBtn.Enabled = false;
				foreList.Enabled = false;
			} else if (foreUser.Checked) {
				foreBtn.Enabled = true;
				foreList.Enabled = false;
			} else if (foreSys.Checked) {
				foreBtn.Enabled = false;
				foreList.Enabled = true;
			}
		}
		
		void backCheck(object sender, EventArgs e)
		{
			if (backNo.Checked) {
				backBtn.Enabled = false;
				backList.Enabled = false;
			} else if (backUser.Checked) {
				backBtn.Enabled = true;
				backList.Enabled = false;
			} else if (backSys.Checked) {
				backBtn.Enabled = false;
				backList.Enabled = true;
			}
		}
		
		void AcceptClick(object sender, EventArgs e)
		{
			object foreColor = null;
			object backColor = null;
			
			if (foreUser.Checked) {
				foreColor = (System.Drawing.Color)foreBtn.CenterColor;
			} else if (foreSys.Checked) {
				foreColor = (string)foreList.SelectedItem;
			}
			
			if (backUser.Checked) {
				backColor = (System.Drawing.Color)backBtn.CenterColor;
			} else if (backSys.Checked) {
				backColor = (string)backList.SelectedItem;
			}
			
			Color = new EditorHighlightColor(foreColor, backColor, boldBox.Checked, italicBox.Checked);
			
			DialogResult = DialogResult.OK;
		}
		
	}
}
