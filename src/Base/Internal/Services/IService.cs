
using System;

namespace NetFocus.DataStructure.Services
{
	/// <summary>
	/// 定义一个所有服务必须实现的接口.
	/// </summary>
	public interface IService
	{
		/// <summary>
		/// 用于初始化一个服务.
		/// </summary>
		void InitializeService();
		
		/// <summary>
		/// 用于卸载一个服务.
		/// </summary>
		void UnloadService();
		
		//定义两个事件,以便某些特定的服务为其添加处理程序.
		event EventHandler Initialize;
		event EventHandler Unload;
	}
}
