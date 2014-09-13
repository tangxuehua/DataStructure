using System;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;

using DevComponents.DotNetBar;

using NetFocus.DataStructure.Commands;
using NetFocus.Components.AddIns.Codons;
using NetFocus.Components.AddIns.Attributes;
using NetFocus.DataStructure.Gui.Components;
using NetFocus.DataStructure.Services;
using NetFocus.Components.AddIns.Conditions;


namespace NetFocus.DataStructure.AddIns.Codons
{
	[Codon("MenuItem")]
	public class MenuItemCodon : AbstractCodon
	{
		[XmlMemberAttribute("label", IsRequired=true)]
		string label       = null;
		[XmlMemberAttribute("description")]
		string description = null;
		[XmlMemberAttribute("shortcut")]
		string shortcut    = null;
		[XmlMemberAttribute("icon")]
		string icon        = null;
		[XmlMemberAttribute("begingroup")]
		string beginGroup  = null;
	
		public string Label 
		{
			get 
			{
				return label;
			}
			set 
			{
				label = value;
			}
		}
		
		public string Description 
		{
			get 
			{
				return description;
			}
			set 
			{
				description = value;
			}
		}
		
		public string Icon 
		{
			get 
			{
				return icon;
			}
			set 
			{
				icon = value;
			}
		}
		public string BeginGroup 
		{
			get 
			{
				return beginGroup;
			}
			set 
			{
				beginGroup = value;
			}
		}
		
		public string Shortcut 
		{
			get 
			{
				return shortcut;
			}
			set 
			{
				shortcut = value;
			}
		}
		
		void SetShortcut(ButtonItem item,string shortcut)
		{
			foreach(eShortcut key in new ShortcutsCollection(item))
			{
				if (key.ToString() == shortcut)
				{
					item.Shortcuts.Add(key);
				}
			}
		}
		void newItem_MouseEnter(object sender,EventArgs e)
		{	
			IStatusBarService statusBarService = (IStatusBarService)ServiceManager.Services.GetService(typeof(IStatusBarService));

			statusBarService.SetMessage(((SdMenuCommand)sender).Description);
		}
		void newItem_MouseLeave(object sender,EventArgs e)
		{	
			IStatusBarService statusBarService = (IStatusBarService)ServiceManager.Services.GetService(typeof(IStatusBarService));

			statusBarService.SetMessage("就绪");
		}
		public override object BuildItem(object owner, ArrayList subItems, ConditionCollection conditions)
		{
			SdMenuCommand newItem = null;	
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			object o = null;
			if (Class != null) 
			{
				o = AddIn.CreateObject(Class);//说明当前菜单项是没有子菜单项�?即它有自己的功能,其功能由Class类具体实�?这种菜单项也是最常见�?
			}
			if (o != null) 
			{
				if (o is ISubmenuBuilder) 
				{
					return ((ISubmenuBuilder)o).BuildSubmenu(owner);
				}
				
				if (o is IMenuCommand) 
				{
					newItem = new SdMenuCommand(stringParserService.Parse(Label), new EventHandler(new MenuEventHandler(owner, (IMenuCommand)o).Execute));
					if(beginGroup == "true")
					{
						newItem.BeginGroup = true;
					}
				}
			}
			
			if (newItem == null) 
			{//说明当前菜单项既不是Link类型�?也没有指出其Class属�?所以有可能是一个包含子菜单的菜单项.
				newItem = new SdMenuCommand(stringParserService.Parse(Label));
				if (subItems != null && subItems.Count > 0) 
				{//判断是否有子菜单�?
					foreach (object item in subItems) 
					{
						if (item is ButtonItem) 
						{//添加一个子菜单�?
							newItem.SubItems.Add((ButtonItem)item);
						} 
						else 
						{//添加一组子菜单�?
							newItem.SubItems.AddRange((ButtonItem[])item);
						}
					}
				}
			}
			
			Debug.Assert(newItem != null);//到这里为�?newItem即当前菜单项不应该为空了.
			
			if (Icon != null) 
			{//为菜单设置Icon.
				ResourceService ResourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
				newItem.Image = ResourceService.GetBitmap(Icon);
			}
			newItem.Description = description;

			newItem.MouseEnter +=new EventHandler(newItem_MouseEnter);
			newItem.MouseLeave +=new EventHandler(newItem_MouseLeave);
			
			if (Shortcut != null) 
			{//为菜单设置Shortcut.
				try 
				{
					newItem.Shortcuts.Add((eShortcut)Enum.Parse(eShortcut.F1.GetType(),Shortcut));
				}
				catch (Exception) 
				{
				}
			}
			
			return newItem;//最后返回当前菜单项.

		}
		
		
		class MenuEventHandler
		{
			IMenuCommand action;
			
			public MenuEventHandler(object owner, IMenuCommand action)
			{
				this.action       = action;
				this.action.Owner = owner;
			}
			
			public void Execute(object sender, EventArgs e)
			{
				action.Run();
			}
		}
	}
}
