
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;

namespace NetFocus.Components.AddIns.Conditions
{
	/// <summary>
	/// A collection containing <code>ConditionBuilder</code> objects.
	/// </summary>
	public class ConditionBuilderCollection : CollectionBase
	{
		/// <summary>
		/// Add a new condition builder
		/// </summary>
		/// <param name="conditionBuilder"></param>
		/// <returns></returns>
		public int AddConditionBuilder(ConditionBuilder conditionBuilder) 
		{
			return List.Add(conditionBuilder);
		}

		
		//查找一个由一个XmlNode结点指定的条件创建器
		bool MatchAttributes(ConditionBuilder builder, XmlNode conditionNode)
		{
			StringCollection requiredAttributes = builder.RequiredAttributes;
			foreach (string attr in requiredAttributes) {
				if (conditionNode.Attributes[attr] == null) {
					return false;
				}
			}
			return true;
		}
		
		
		/// <summary>
		/// Returns a <see cref="ConditionBuilder"/> object which is able to construct a new
		/// <see cref="ICondition"/> object with the data collected from <code>conditionNode</code>
		/// </summary>
		/// <param name="conditionNode">
		/// The node with the attributes for the condition.
		/// </param>
		public ConditionBuilder GetConditionBuilder(XmlNode conditionNode) 
		{
			foreach (ConditionBuilder builder in this) {
				if (MatchAttributes(builder, conditionNode)) {
					return builder;
				}
			}
			return null;
		}
	}
}
