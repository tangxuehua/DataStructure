
using System;
using System.Xml;

namespace NetFocus.Components.AddIns.Conditions
{
	/// <summary>
	/// Default actions, when a condition is failed.
	/// </summary>
	public enum ConditionFailedAction {
		Nothing,
		Exclude,
		Disable
	}
	
	
	/// <summary>
	/// The <code>ICondition</code> interface describes the basic funcionality
	/// a condition must have.
	/// </summary>
	public interface ICondition
	{
		/// <summary>
		/// Returns the action which occurs, when this condition fails.
		/// </summary>
		ConditionFailedAction FailedAction {
			get;
			set;
		}
		
		/// <summary>
		/// Returns true, when the condition is valid otherwise false.
		/// </summary>
		bool IsValid(object caller);
	}
}
