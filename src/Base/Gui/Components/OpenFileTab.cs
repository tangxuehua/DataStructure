
using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using DevComponents.DotNetBar;
using Crownwood.Magic.Menus;
using Crownwood.Magic.Controls;

using NetFocus.Components.AddIns;
using NetFocus.DataStructure.Services;


namespace NetFocus.DataStructure.Gui.Components
{
	public class OpenFileTab : Crownwood.Magic.Controls.TabControl, IOwnerState
	{
		readonly static string contextMenuPath = "/DataStructure/Workbench/OpenFileTab/ContextMenu";
		DotNetBarManager dotNetBarManager1 = new DotNetBarManager();
		#region the implements of the interface IOwnerState

		[Flags]
		public enum OpenFileTabState 
		{
			Nothing             = 0,
			FileDirty           = 1,
			ClickedWindowIsForm = 2,
			FileUntitled        = 4
		}
		OpenFileTabState internalState = OpenFileTabState.Nothing;
		public System.Enum InternalState 
		{
			get 
			{
				return internalState;
			}
		}
		

		#endregion

		int clickedTabIndex = -1;
		public int ClickedTabIndex 
		{
			get 
			{
				return clickedTabIndex;
			}
			set 
			{
				clickedTabIndex = value;
			}
		}

		
		public IViewContent ClickedView 
		{
			get 
			{
				if (clickedTabIndex == -1) 
				{
					return null;
				}
				return ((MyTabPage)this.TabPages[clickedTabIndex]).ViewContent;
			}
		}


		void MySelectionChanged(object sender, EventArgs e)
		{
			if (SelectedIndex >= 0 && SelectedIndex < TabPages.Count && TabPages[SelectedIndex] != null) 
			{
				((MyTabPage)TabPages[SelectedIndex]).ViewContent.SelectView();
			}
		}
		void MyClosePressed(object sender, EventArgs e)
		{
			if (SelectedIndex >= 0 && SelectedIndex < TabPages.Count) 
			{
				((MyTabPage)TabPages[SelectedIndex]).ViewContent.CloseView(false);
			}
		}
		
		
		public OpenFileTab()
		{
			this.SelectionChanged += new EventHandler(MySelectionChanged);
			this.ClosePressed     += new EventHandler(MyClosePressed);

		}
				
		
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right) 
			{
				ButtonItem[] contextMenu = (ButtonItem[])(AddInTreeSingleton.AddInTree.GetTreeNode(contextMenuPath).BuildChildItems(this)).ToArray(typeof(ButtonItem));
				
				if (contextMenu.Length > 0 && TabPages.Count > 0 && clickedTabIndex >= 0) 
				{
					ButtonItem item = new ButtonItem();
					item.SubItems.AddRange(contextMenu);
					dotNetBarManager1.RegisterPopup(item);
					//Control ctrl=this as Control;
					//Point p=this.PointToScreen(new Point(ctrl.Left,ctrl.Bottom));
					Point p = this.PointToScreen(new Point(e.X,e.Y));
					item.PopupMenu(p);
					
					//PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
					//popup.Style = (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("NetFocus.DataStructure.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
					//popup.TrackPopup(PointToScreen(new Point(e.X, e.Y)));
				}
			} 
			else 
			{
				base.OnMouseUp(e);
			}
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			clickedTabIndex = -1;
			
			for(int i=0; i<_tabPages.Count; i++) 
			{
				Rectangle rect = (Rectangle)_tabRects[i];
				if (rect.Contains(e.X, e.Y)) 
				{
					clickedTabIndex = i;
					break;
				}
			}
			
			internalState = OpenFileTabState.Nothing;
			if (clickedTabIndex != -1) 
			{
				if (ClickedView.ContentName == null) 
				{
					internalState |= OpenFileTabState.FileUntitled;
				}
				if (ClickedView.IsDirty) 
				{
					internalState |= OpenFileTabState.FileDirty;
				}
				if (ClickedView is Form) 
				{
					internalState |= OpenFileTabState.ClickedWindowIsForm;
				}
			}
		}
		
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove (e);
			int currentTabIndex = -1;
			
			for(int i=0; i<_tabPages.Count; i++) 
			{
				Rectangle rect = (Rectangle)_tabRects[i];
				if (rect.Contains(e.X, e.Y)) 
				{
					currentTabIndex = i;
					break;
				}
			}
			DefaultStatusBarService defaultStatusBarService = (DefaultStatusBarService)ServiceManager.Services.GetService(typeof(DefaultStatusBarService));
			string message="¾ÍÐ÷";
			
			if(currentTabIndex != -1)
			{
				message = ((MyTabPage)TabPages[currentTabIndex]).ViewContent.ContentName;
			}
			defaultStatusBarService.SetMessage(message);

		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave (e);

			DefaultStatusBarService defaultStatusBarService = (DefaultStatusBarService)ServiceManager.Services.GetService(typeof(DefaultStatusBarService));

			defaultStatusBarService.SetMessage("¾ÍÐ÷");

		}

		
		public void AddViewContentToTabPage(IViewContent content)
		{
			Crownwood.Magic.Controls.TabPage tabPage = new MyTabPage(this, content);
			tabPage.Selected = true;
			tabPage.Tag = content;
			content.Control.Dock = DockStyle.Fill;
			tabPage.Controls.Add(content.Control);

			TabPages.Add(tabPage);

		}


		#region MyTabPage

		public class MyTabPage : Crownwood.Magic.Controls.TabPage
		{
			IViewContent content;
			OpenFileTab tab;
			EventHandler setTitleEvent = null;
			
			public IViewContent ViewContent 
			{
				get 
				{
					return content;
				}
			}
			
			public MyTabPage(OpenFileTab tab, IViewContent content)
			{
				this.tab    = tab;
				this.content = content;
				
				content.CloseEvent     += new EventHandler(CloseEvent);

				setTitleEvent = new EventHandler(SetTitle);

				content.ContentNameChanged += setTitleEvent;
				content.DirtyChanged       += setTitleEvent;

				SetTitle(null,null);

			}
			

			
			void CloseEvent(object sender, EventArgs e)
			{
				if (tab.TabPages.IndexOf(this) >= 0) 
				{
					tab.TabPages.Remove(this);
				}
			}
			void SetTitle(object sender, EventArgs e)
			{
				if (content == null) 
				{
					return;
				}
				string newTitle = "";
	
				if (content.ContentName == null) 
				{
					string baseName  = Path.GetFileNameWithoutExtension(content.UntitledName);
					int    number    = 0;

					foreach (IViewContent viewContent in WorkbenchSingleton.Workbench.ViewContentCollection) 
					{
						string title = viewContent.UntitledName;

						if (title.IndexOf(baseName) >= 0)  
						{
							++number;
						}
					}
					newTitle = baseName + number;
				}
				else 
				{
					newTitle = Path.GetFileName(content.ContentName);
				}
			
				if (content.IsDirty) 
				{
					newTitle += "*";
				} 
				else if (content.IsReadOnly) 
				{
					newTitle += "+";
				}
			
				if (newTitle != Title) 
				{
					Title = newTitle;
				}

			}
		


		}


		#endregion
		
	}
}
