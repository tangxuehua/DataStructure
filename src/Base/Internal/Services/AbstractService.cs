
using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Resources;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace NetFocus.DataStructure.Services
{
	/// <summary>
	/// 对IService接口的一个抽象实现.
	/// </summary>
	public class AbstractService : IService
	{
		public virtual void InitializeService()
		{
			OnInitialize(EventArgs.Empty);
		}
		
		
		public virtual void UnloadService()
		{
			OnUnload(EventArgs.Empty);
		}
		
		protected virtual void OnInitialize(EventArgs e)
		{
			if (Initialize != null) {
				Initialize(this, e);
			}
		}
		
		protected virtual void OnUnload(EventArgs e)
		{
			if (Unload != null) {
				Unload(this, e);
			}
		}
		
		public event EventHandler Initialize;
		public event EventHandler Unload;
	}
}
