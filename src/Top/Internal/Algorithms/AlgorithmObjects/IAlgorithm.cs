// created on 2005-5-11 at 14:22
using System.Drawing;
using System.Windows.Forms;
using System;

namespace NetFocus.DataStructure.Internal.Algorithm {
	
	public interface IAlgorithm {

		/// <summary>
		/// 代表算法的当前行
		/// </summary>
		int CurrentLine{
			get;
			set;
		}
		/// <summary>
		/// 代表算法执行完毕时的行号集合
		/// </summary>
		int[] LastLines{
			get;
			set;
		}
		/// <summary>
		/// 演示算法时所需的源代码文件
		/// </summary>
		string[] CodeFiles{
			get;
			set;
		}
		/// <summary>
		/// 代表当前算法对象的状态
		/// </summary>
		object Status
		{
			get;
			set;
		}

		/// <summary>
		/// 当前算法的断点集合
		/// </summary>
		int[] BreakPoints
		{
			get;
			set;
		}
		//用于接受当前算法自定义数据的对话框的类型
		Type DialogType
		{
			get;
			set;
		}
		/// <summary>
		/// 为当前算法显示自定义对话框来接受数据
		/// </summary>
		bool ShowCustomizeDialog();
		/// <summary>
		/// 恢复算法到刚初始化时的状态
		/// </summary>
		void Recover();
		/// <summary>
		/// 得到演示算法所需要的初始化数据
		/// </summary>
		bool GetData();
		/// <summary>
		/// 初始化当前算法对象
		/// </summary>
		void Initialize(bool isOpen);
		/// <summary>
		/// 初始化当前算法对象中的动画显示
		/// </summary>
		void InitGraph();
		/// <summary>
		/// 执行并更新当前算法的当前行
		/// </summary>
		void ExecuteAndUpdateCurrentLine();
		/// <summary>
		/// 更新当前算法对象中动画的外观
		/// </summary>
		void UpdateGraphAppearance();
		/// <summary>
		/// 更新当前算法对象所在的视图,使其更新高亮度显示的行
		/// </summary>
		void UpdateCurrentView();
		/// <summary>
		/// 当当前算法对象的状态改变,或者从一个算法对象换到另一个算法对象时,更新属性面板
		/// </summary>
		void UpdatePropertyPad();
		/// <summary>
		/// 更新动画面板
		/// </summary>
		void UpdateAnimationPad();
		/// <summary>
		/// 更新当前算法对象的状态,这是一个模板方法
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void UpdateAlgorithmStatus(object sender,EventArgs e);

	} 

}

