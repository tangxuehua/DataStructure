
namespace NetFocus.DataStructure.Gui
{
	/// <summary>
	/// If a IViewContent object is from the type IPositionable it signals
	/// that it's a TextEditor which could set the caret to a position inside
	/// a file. 
	/// </summary>
	public interface IPositionable
	{
		/// <summary>
		/// Sets the 'caret' to the position pos, where pos.Y is the line (starting from 0).
		/// And pos.X is the column (starting from 0 too).
		/// </summary>
		void JumpTo(int line, int column);
	}
}
