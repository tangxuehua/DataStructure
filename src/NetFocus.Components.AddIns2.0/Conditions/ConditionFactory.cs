
using System;
using System.Xml;

using NetFocus.Components.AddIns.Exceptions;

namespace NetFocus.Components.AddIns.Conditions
{
	/// <summary>
	/// Creates a new <code>ICondition</code> object.
	/// </summary>
	public class ConditionFactory
	{
		ConditionBuilderCollection conditionBuilderCollection = new ConditionBuilderCollection();
		
		/// <summary>
		/// Add a new condition builder to the collection.
		/// </summary>
		/// <exception cref="DuplicateConditionException">
		/// When there is already a condition which the same required attributes
		/// in this collection.
		/// </exception>
		/// <exception cref="ConditionWithoutRequiredAttributesException">
		/// When the given condition does not have required attributes.
		/// </exception>
		public void AddConditionBuilder(ConditionBuilder builder) 
		{
			foreach (ConditionBuilder b in conditionBuilderCollection) 
			{
				if (b.RequiredAttributes.Equals(builder.RequiredAttributes)) 
				{
					throw new DuplicateConditionException(builder.RequiredAttributes);
				}
			}
			if (builder.RequiredAttributes.Count == 0) 
			{
				throw new ConditionWithoutRequiredAttributesException();
			}
			conditionBuilderCollection.AddConditionBuilder(builder);
		}
	
		
		/// <summary>
		/// Creates a new <code>ICondition</code> object using <code>conditionNode</code>
		/// as a mask of which class to take for creation.
		/// </summary>
		public ICondition CreateCondition(AddIn addIn, XmlNode conditionNode)
		{
			//通过一个XmlNode来得到一个条件创建器,因此,这里与得到代码子创建器的方式不一样
			//那么为什么不像代码子的方式那样根据条件名字来得到一个条件创建器呢?
			//原因是因为条件不能像代码子那样用一个简单的名字来区别,要根据builder.RequiredAttributes来区别,
			//所以不能简单的使用一个Hashtable来存放条件创建器
			ConditionBuilder builder = conditionBuilderCollection.GetConditionBuilder(conditionNode);
			
			if (builder == null) {
				throw new ConditionNotFoundException("unknown condition found");
			}
			
			return builder.BuildCondition(addIn);
		}
		
	}
}
