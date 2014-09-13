using System;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Globalization;
using System.Configuration;
using System.Xml;

using NetFocus.Components.AddIns;
using NetFocus.DataStructure.Properties;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Gui;
using NetFocus.DataStructure.Gui.Pads;


namespace NetFocus.DataStructure.Internal.Algorithm
{
	public delegate void PictureBoxPaintEventHandler(Graphics g);
	
	public class AlgorithmManager
	{
		static AlgorithmManager algorithmManager = new AlgorithmManager();
		string algorithmExampleDataFile = Application.StartupPath + Path.DirectorySeparatorChar + ConfigurationSettings.AppSettings["AlgorithmExampleDataFile"];
		string algorithmFilesPath = Application.StartupPath + Path.DirectorySeparatorChar + ConfigurationSettings.AppSettings["AlgorithmFilesPath"];
		System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
		int interval = 1000;
		bool isByStep = false;
		bool isRunto  = false;
		IAlgorithm currentAlgorithm = null;  
		Hashtable exampleDataHashTable = new Hashtable(); //用于保存所有算法对象的样本数据
		Hashtable algorithmsHashTable = new Hashtable(); //用于保存已经打开过的算法对象的一个Hashtable
		Hashtable openingAlgorithms = new Hashtable();
		ArrayList algorithmList = new ArrayList(); //用于保存所有算法对象的一个Hashtable
		EventHandler updateAlgorithmStatusEventHandler = null;
		public EventHandler ClearPadsHandler = null;
		PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		Hashtable LoadExampleDatasFromStream(string filename)
		{
			XmlDocument doc = new XmlDocument();
			try 
			{
				doc.Load(filename);
				
				XmlNodeList nodes  = doc.DocumentElement.ChildNodes;
				foreach (XmlElement el in nodes)
				{
					exampleDataHashTable[el.Attributes["name"].Value] = el.SelectSingleNode("ExampleData");
				}
			} 
			catch
			{
				return null;
			}
			return exampleDataHashTable;
		}
		
		private AlgorithmManager()
		{
			ClearPadsHandler = new EventHandler(ClearAllPads);
			timer.Interval = interval = Convert.ToInt32(propertyService.GetProperty("NetFocus.DataStructure.AlgorithmExecuteSpeed", "1000"));
            InitializeAlgorithms("/DataStructure/Internal/Algorithm");
		}

		
		/// <summary>
		/// 算法执行的速度
		/// </summary>
		public int ExecuteSpeed
		{
			get
			{
				return interval;
			}
			set
			{
				interval = value;
			}
		}
		
		/// <summary>
		/// 应用程序存储算法初始化数据的文件
		/// </summary>
		public string AlgorithmExampleDataFile
		{
			get
			{
				return algorithmExampleDataFile;
			}
		}
		
		/// <summary>
		/// 存放算法源文件的路径
		/// </summary>
		public string AlgorithmFilesPath
		{
			get
			{
				return this.algorithmFilesPath;
			}
		}
		
		/// <summary>
		/// 得到所有算法的样本数据,并放到哈希表中
		/// </summary>
		/// <returns></returns>
		public Hashtable GetExampleDatas()
		{
			return LoadExampleDatasFromStream(algorithmExampleDataFile);
		}

		/// <summary>
		/// 提供访问算法管理器的唯一访问点
		/// </summary>
		public static AlgorithmManager Algorithms{
			get{
				return algorithmManager;
			}
		
		}
		
	    /// <summary>
	    /// 更新当前算法状态的事件处理程序
	    /// </summary>
		public EventHandler UpdateAlgorithmStatusEventHandler
		{
			get
			{
				return updateAlgorithmStatusEventHandler;
			}
			set
			{
				updateAlgorithmStatusEventHandler = value;
			}
		}
		/// <summary>
		/// 维护当前已经打开的算法对象
		/// </summary>
		public Hashtable OpeningAlgorithms
		{
			get
			{
				return openingAlgorithms;
			}
		}
		/// <summary>
		/// 一个定时器,用于控制算法演示时的行执行间隔
		/// </summary>
		public System.Windows.Forms.Timer Timer
		{
			get 
			{
				 return timer;
			}
		}
		/// <summary>
		/// 表明当前是否为单步调试
		/// </summary>
		public bool IsByStep
		{
			get
			{
				return isByStep;
			}
			set
			{
				isByStep = value;
			}
		}
		/// <summary>
		/// 表明当前是否为运行到操作
		/// </summary>
		public bool IsRunto
		{
			get
			{
				return isRunto;
			}
			set
			{
				isRunto = value;
			}
		}
		/// <summary>
		/// 用于保存当前算法
		/// </summary>
		public IAlgorithm CurrentAlgorithm
		{
			get
			{
				return currentAlgorithm;	
			}
			set
			{
				currentAlgorithm = value;
			}
		}

		/// <summary>
		/// 通过一个指定的路径初始化所有的算法对象,并保存到一个ArrayList中
		/// </summary>
		/// <param name="algorithmPath">一个从插件文件中读取的逻辑路径(即密码子的路径)</param>
		public void InitializeAlgorithms(string algorithmPath) 
		{
			try
			{
				IAddInTreeNode treeNode = AddInTreeSingleton.AddInTree.GetTreeNode(algorithmPath);
				ArrayList childItems = treeNode.BuildChildItems(this);
				AddAlgorithms((IAlgorithm[])childItems.ToArray(typeof(IAlgorithm)));
			}
			catch
			{
			}
	
		}
		
		bool IsInstanceOfType(Type type, IAlgorithm algorithm) {
			Type algorithmType = algorithm.GetType();

			Type[] interfaces = algorithmType.GetInterfaces();

			foreach (Type iface in interfaces) {
				if (iface == type) {
					return true;
				}
			}
			
			while (algorithmType != typeof(System.Object)) {
				if (type == algorithmType) {
					return true;
				}
				algorithmType = algorithmType.BaseType;
			}
			return false;
		}

		/// <summary>
		/// 此函数很重要，根据一个算法的类型得到一个算法,其中以一个哈希表作为缓存。
		/// </summary>
		/// <param name="algorithmType"></param>
		/// <returns></returns>
		public IAlgorithm GetAlgorithm(Type algorithmType) 
		{	 
			IAlgorithm a = (IAlgorithm)algorithmsHashTable[algorithmType];
			if(a != null) {
				return a;	 
			}
			foreach(IAlgorithm algorithm in algorithmList) {
				if (IsInstanceOfType(algorithmType, algorithm)) {
					algorithmsHashTable[algorithmType]=algorithm;
					return algorithm;	 
				}
			}
			return null;

		}

		void AddAlgorithm(IAlgorithm algorithm) 
		{
			 algorithmList.Add(algorithm);
		}

		void AddAlgorithms(IAlgorithm[] algorithms) 
		{
			foreach(IAlgorithm algorithm in algorithms) {
				 AddAlgorithm(algorithm);
			}
		}
		
		
		Color ParseColor(string c)
		{
			int a = 255;
			int offset = 0;
			Color color;
			try
			{
				offset = 2;
				a = Int32.Parse(c.Substring(1,2), NumberStyles.HexNumber);
				int r = Int32.Parse(c.Substring(1 + offset,2), NumberStyles.HexNumber);
				int g = Int32.Parse(c.Substring(3 + offset,2), NumberStyles.HexNumber);
				int b = Int32.Parse(c.Substring(5 + offset,2), NumberStyles.HexNumber);

				color = Color.FromArgb(a, r, g, b);
			}
			catch
			{
				color = Color.FromName(c);
			}
			return color;
		}
		/// <summary>
		/// 将动画面板清空
		/// </summary>
		public Graphics ClearAnimationPad()
		{
			IPadContent animationPad = WorkbenchSingleton.Workbench.GetPad(typeof(NetFocus.DataStructure.Gui.Pads.AnimationPad));
			if(animationPad == null)
			{
				Bitmap bmp1 = new Bitmap(1 ,1);
				Graphics g1 = Graphics.FromImage(bmp1);
				return g1;
			}
			int x = animationPad.Control.Left > 0 ? animationPad.Control.Left : 0;
			int y = animationPad.Control.Top > 0 ? animationPad.Control.Top : 0;
			int width = animationPad.Control.Width > 0 ? animationPad.Control.Width : 1;
			int height = animationPad.Control.Height > 0 ? animationPad.Control.Height : 1;

			Bitmap bmp = new Bitmap(width,height);
			Graphics g = Graphics.FromImage(bmp);
			g.DrawRectangle(new Pen(Color.Gray,1),x,y,width - 1,height - 1);
			((PictureBox)animationPad.Control).BackgroundImage = bmp;
			
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			Color c = ParseColor(propertyService.GetProperty("NetFocus.DataStructure.AnimationPadPanel.BackColor","White"));
			animationPad.Control.BackColor = c;
			
			return g;  //这里,最后要把Graphics对象返回,因为一个特定的算法对象还要借助这个对象画一些特定的动画信息

		}
		/// <summary>
		/// 将堆栈面板清空
		/// </summary>
		public Graphics ClearStackPad()
		{
			IPadContent stackPad = WorkbenchSingleton.Workbench.GetPad(typeof(NetFocus.DataStructure.Gui.Pads.StackPad));
			if(stackPad == null)
			{
				Bitmap bmp1 = new Bitmap(1 ,1);
				Graphics g1 = Graphics.FromImage(bmp1);
				return g1;
			}
			int x = stackPad.Control.Left > 0 ? stackPad.Control.Left : 0;
			int y = stackPad.Control.Top > 0 ? stackPad.Control.Top : 0;
			int width = stackPad.Control.Width > 0 ? stackPad.Control.Width : 1;
			int height = stackPad.Control.Height > 0 ? stackPad.Control.Height : 1;

			Bitmap bmp = new Bitmap(width,height);
			Graphics g = Graphics.FromImage(bmp);
			g.DrawRectangle(new Pen(Color.Gray,1),x,y,width - 1,height - 1);
            ((PictureBox)stackPad.Control).BackgroundImage = bmp;
			
			return g;  //这里,最后要把Graphics对象返回,因为一个特定的算法对象还要借助这个对象画一些特定的动画信息

		}

		/// <summary>
		/// 清空属性面板
		/// </summary>
		public void ClearPropertyPad()
		{
			IPadContent propertyPad = WorkbenchSingleton.Workbench.GetPad(typeof(PropertyPad));
			if(propertyPad != null) 
			{
				((PropertyGrid)propertyPad.Control).SelectedObject = null;
			}
		}
		
		/// <summary>
		/// 同时清空动画面板和属性面板,并将当前算法设置为空
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void ClearAllPads(object sender,EventArgs e)
		{
			AlgorithmManager.Algorithms.CurrentAlgorithm = null;
			AlgorithmManager.Algorithms.Timer.Stop();
			AlgorithmManager.Algorithms.Timer.Tick -= AlgorithmManager.Algorithms.UpdateAlgorithmStatusEventHandler;
			ClearAnimationPad();
			ClearPropertyPad();
			ClearStackPad();
		}
		
		/// <summary>
		/// 恢复所有已经打开的算法到初始化状态
		/// </summary>
		public void RecoverAllOpeningAlgorithms()
		{
			IAlgorithm tempAlgorithm = currentAlgorithm;

			foreach(DictionaryEntry entry in algorithmManager.openingAlgorithms)
			{
				currentAlgorithm = entry.Value as IAlgorithm;
				currentAlgorithm.Recover();
			}
			currentAlgorithm = tempAlgorithm;
			if(currentAlgorithm != null)
			{
				currentAlgorithm.Recover();
			}
			else
			{
				ClearAllPads(null,null);
			}
		}
		

	}
}
