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

using NetFocus.Components.AddIns;


namespace NetFocus.DataStructure.Services
{
	/// <summary>
	/// 定义一个管理所有服务的类,该类以singleton设计模式的方式向外界提供所要求的服务.
	/// </summary>
	public class ServiceManager
	{
		static ArrayList serviceList       = new ArrayList();//存放服务的一个列表
		Hashtable servicesHashtable = new Hashtable();//用于存放一些已经被访问过的服务，这里使用Hashtable是因为它速度更快
		
		static ServiceManager defaultServiceManager = new ServiceManager();
		static bool isInitialized = false;

		/// <summary>
		/// 得到ServiceManager对象.
		/// </summary>
		public static ServiceManager Services {
			get {
                if (!isInitialized)
                {
                    isInitialized = true;
                    InitializeServicesSubsystem("/Workspace/Services");
                }
				return defaultServiceManager;
			}
		}		
		
		/// <summary>
		/// 声明一个私有的构造函数,已使得该类不能被外界得类所实例化,这样可以保证应用程序只有一个ServiceManager对象,这正是Singleton设计模式的关键.
		/// </summary>
		private ServiceManager()
		{
			//添加三个核心服务.
			AddService(new PropertyService());
			AddService(new StringParserService());
			AddService(new ResourceService());
		}
		
		/// <remarks>
		/// 初始化服务子系统,由插件文件中定义的代码子来确定要初始化哪些服务.
		/// </remarks>
		private static void InitializeServicesSubsystem(string servicesPath)
		{
			IAddInTreeNode treeNode = AddInTreeSingleton.AddInTree.GetTreeNode(servicesPath);
            ArrayList childItems = treeNode.BuildChildItems(defaultServiceManager);
            foreach (IService service in (IService[])childItems.ToArray(typeof(IService)))
            {
                serviceList.Add(service);
            }
			// 下面通过迭代来初始化所有的服务.
			foreach (IService service in serviceList) 
			{
				service.InitializeService();
			}
		}
		
		/// <remarks>
		/// Calls UnloadService on all services. This method must be called ONCE.
		/// </remarks>
		public void UnloadAllServices()
		{
			foreach (IService service in serviceList) {
				service.UnloadService();
			}
		}
		
		
		public void AddService(IService service)
		{
			serviceList.Add(service);
		}
		
		public void AddServices(IService[] services)
		{
			foreach (IService service in services) {
				AddService(service);
			}
		}
		

		bool IsInstanceOfType(Type type, IService service)
		{
			Type serviceType = service.GetType();

			Type[] interfaces = serviceType.GetInterfaces();

			foreach (Type iface in interfaces) 
			{
				if (iface == type) 
				{
					return true;
				}
			}
			
			while (serviceType != typeof(System.Object)) 
			{
				if (type == serviceType) 
				{
					return true;
				}
				serviceType = serviceType.BaseType;
			}
			return false;
		}
		
		/// <remarks>
		/// 根据服务的类型,提供给外界一个特定的服务.
		/// </remarks>
		public IService GetService(Type serviceType)
		{
			IService s = (IService)servicesHashtable[serviceType];
			if (s != null) //表明该服务已经被访问过
			{
				return s;
			}
			
			foreach (IService service in serviceList) 
			{
				if (IsInstanceOfType(serviceType, service)) 
				{
					servicesHashtable[serviceType] = service;//这里,只要是被第一次被声请的服务,都会被加入Hashtable中,
					return service;                          //当第二次再请求这个服务时,就可以直接从这个Hashtable中取出,
					                                         //也就是用一个Hashtable来做二级缓存.
				}
			}
			
			return null;
		}
	}
}
