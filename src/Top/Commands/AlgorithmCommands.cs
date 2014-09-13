using System;
using System.Windows.Forms;
using NetFocus.DataStructure.Internal.Algorithm;
using NetFocus.Components.AddIns.Codons;


namespace NetFocus.DataStructure.Commands.Algorithm
{
	//提供一个子类来为所有的Run方法提供一个统一的调用
	class InternalCommand
	{
		static bool HaveFinished()
		{
			if(AlgorithmManager.Algorithms.CurrentAlgorithm == null)
			{
				return true;
			}
			for(int i = 0;i < AlgorithmManager.Algorithms.CurrentAlgorithm.LastLines.Length;i++)
			{
				if(AlgorithmManager.Algorithms.CurrentAlgorithm.CurrentLine == AlgorithmManager.Algorithms.CurrentAlgorithm.LastLines[i])//判断是否运行到最后一行
				{	
					return true;
				}
			}
			return false;
			
		}
	    public static void Run(Type algorithmType)
		{
			IAlgorithm currentAlgorithm = AlgorithmManager.Algorithms.CurrentAlgorithm;

			IAlgorithm algorithm = (IAlgorithm)AlgorithmManager.Algorithms.GetAlgorithm(algorithmType);
			if(algorithm != null) 
			{
				if(AlgorithmManager.Algorithms.OpeningAlgorithms.Contains(algorithmType))
				{
					MessageBox.Show("算法已经打开！","消息",MessageBoxButtons.OK,MessageBoxIcon.Warning);
					return;
				}
				AlgorithmManager.Algorithms.Timer.Enabled = false;
				AlgorithmManager.Algorithms.CurrentAlgorithm = algorithm;
				AlgorithmManager.Algorithms.OpeningAlgorithms.Add(algorithmType,algorithm);
				if(algorithm.GetData() == true)
				{
					algorithm.Initialize(false);	 
				}
				else
				{
					AlgorithmManager.Algorithms.CurrentAlgorithm = currentAlgorithm;
					AlgorithmManager.Algorithms.OpeningAlgorithms.Remove(algorithmType);
					if(HaveFinished() == false)
					{
						AlgorithmManager.Algorithms.Timer.Start();
					}
				}
			}
		}
	}
	
	public class SequenceInsert : AbstractMenuCommand
	{
		public override void Run() {
			InternalCommand.Run(typeof(NetFocus.DataStructure.Internal.Algorithm.SequenceInsert));
		}
	}

	public class SequenceDelete : AbstractMenuCommand 
	{
		public override void Run() {
			InternalCommand.Run(typeof(NetFocus.DataStructure.Internal.Algorithm.SequenceDelete));
		}
	}

	public class SequenceMerge : AbstractMenuCommand 
	{
		public override void Run() {
			InternalCommand.Run(typeof(NetFocus.DataStructure.Internal.Algorithm.SequenceMerge));
		}
	}

	public class CreateList : AbstractMenuCommand 
	{
		public override void Run() {
			InternalCommand.Run(typeof(NetFocus.DataStructure.Internal.Algorithm.CreateList));
		}
	}

	public class ListInsert : AbstractMenuCommand 
	{
		public override void Run() {
			InternalCommand.Run(typeof(NetFocus.DataStructure.Internal.Algorithm.ListInsert));
		}
	}

	public class ListDelete : AbstractMenuCommand 
	{
		public override void Run() {
			InternalCommand.Run(typeof(NetFocus.DataStructure.Internal.Algorithm.ListDelete));
		}
	}
	public class IndexBF : AbstractMenuCommand 
	{
		public override void Run() 
		{
			InternalCommand.Run(typeof(NetFocus.DataStructure.Internal.Algorithm.IndexBF));
		}
	}

	public class SequenceSearch : AbstractMenuCommand
	{
		public override void Run() 
		{
			InternalCommand.Run(typeof(NetFocus.DataStructure.Internal.Algorithm.SeqSearch));
		}
	}

	public class BinarySearch : AbstractMenuCommand 
	{
		public override void Run() 
		{
			InternalCommand.Run(typeof(NetFocus.DataStructure.Internal.Algorithm.BinSearch));
		}
	}

	public class BinarySearchTree : AbstractMenuCommand 
	{
		public override void Run() 
		{
			InternalCommand.Run(typeof(NetFocus.DataStructure.Internal.Algorithm.BSTSearch));
		}
	}

	public class BubbleSort : AbstractMenuCommand 
	{
		public override void Run() 
		{
			InternalCommand.Run(typeof(NetFocus.DataStructure.Internal.Algorithm.BubbleSort));
		}
	}

	public class InsertSort : AbstractMenuCommand 
	{
		public override void Run() 
		{
			InternalCommand.Run(typeof(NetFocus.DataStructure.Internal.Algorithm.InsertSort));
		}
	}

	public class SelectSort : AbstractMenuCommand 
	{
		public override void Run() 
		{
			InternalCommand.Run(typeof(NetFocus.DataStructure.Internal.Algorithm.SelectSort));
		}
	}

	public class QuickSort : AbstractMenuCommand 
	{
		public override void Run() 
		{
			InternalCommand.Run(typeof(NetFocus.DataStructure.Internal.Algorithm.OneQuickPass));
		}
	}

	public class PreOrderTraverse : AbstractMenuCommand 
	{
		public override void Run() 
		{
			InternalCommand.Run(typeof(NetFocus.DataStructure.Internal.Algorithm.PreOrderTraverse));
		}
	}

	public class InOrderTraverse : AbstractMenuCommand 
	{
		public override void Run() 
		{
			InternalCommand.Run(typeof(NetFocus.DataStructure.Internal.Algorithm.InOrderTraverse));
		}
	}

	public class PostOrderTraverse : AbstractMenuCommand 
	{
		public override void Run() 
		{
			InternalCommand.Run(typeof(NetFocus.DataStructure.Internal.Algorithm.PostOrderTraverse));
		}
	}


}
