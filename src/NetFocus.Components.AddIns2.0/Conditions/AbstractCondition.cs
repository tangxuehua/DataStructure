
using System;
using System.Xml;

using NetFocus.Components.AddIns.Attributes;

namespace NetFocus.Components.AddIns.Conditions
{
	/// <summary>
	/// This is a abstract implementation of the <see cref="ICondition"/> interface.
	/// </summary>
	public abstract class AbstractCondition : ICondition
	{
		[XmlMemberAttribute("FailedAction")]
		ConditionFailedAction failedAction = ConditionFailedAction.Exclude;
		
		/// <summary>
		/// Returns the action, if the condition is failed.
		/// </summary>
		public ConditionFailedAction FailedAction {
			get {
				return failedAction;
			}
			set {
				failedAction = value;
			}
		}
		
		/// <summary>
		/// Inheriting classes need to overwrite this method.
		/// </summary>
		/// <seealso cref="ICondition"/> interface.
		public abstract bool IsValid(object caller);
	}
}
