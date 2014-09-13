using System;
using System.Windows.Forms;
using System.Drawing;

namespace NetFocus.DataStructure.Internal.Algorithm
{
	/// <summary>
	/// this class represents an example data item of one algorithm
	/// </summary>
	public class StatusItem : Panel
	{
		PictureBox pic = new PictureBox();
		object itemInfo;

		public System.Drawing.Image Image
		{
			get
			{
				return pic.Image;
			}
			set
			{
				pic.Image = value;
			}
		}


		public object ItemInfo
		{
			get
			{
				return itemInfo;
			}
			set
			{
				itemInfo = value;
			}
		}
		
		public bool BoolTag
		{
			get
			{
				return (bool)pic.Tag;
			}
			set
			{
				pic.Tag = value;
			}
		}
		
		
		public event EventHandler ItemClick;

		public virtual void OnItemClick(EventArgs e)
		{
			if (ItemClick != null) 
			{
				ItemClick(this, e);
			}
		}

		
		private void pic_Click(object sender, System.EventArgs e)
		{
			PictureBox pic = sender as PictureBox;
			if(pic != null)
			{
				pic.Tag = true;
				pic.Invalidate();
				OnItemClick(e);
				
			}
		}
		
	
		private void pic_Paint(object sender, PaintEventArgs e)
		{
			PictureBox pic = sender as PictureBox;
			if(pic != null && (bool)pic.Tag == true)
			{
				e.Graphics.DrawRectangle(new Pen(Color.Black,1),1,1,pic.Width - 3,pic.Height - 3);
			}
		}

		
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);

			pic.Invalidate();

		}


		public StatusItem() : this(null)
		{
		}

		
		public StatusItem(object itemInfo) : base()
		{
			this.itemInfo = itemInfo;

			this.Dock = DockStyle.Top;
			this.DockPadding.Bottom = 2;
			this.DockPadding.Left = 4;
			this.DockPadding.Right = 4;
			this.pic.Dock = DockStyle.Fill;
			this.pic.SizeMode = PictureBoxSizeMode.StretchImage;
			pic.Tag = false;
			pic.Click +=new EventHandler(pic_Click);
			pic.Paint +=new PaintEventHandler(pic_Paint);
			this.Controls.Add(pic);

		}

	
	}
}
