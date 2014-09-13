using System.Drawing;
using System.Windows.Forms;

namespace NetFocus.DataStructure.Services
{
	public interface IStatusBarService
	{
		
		Control Control {
			get;
		}
		
		void ShowErrorMessage(string message);
		
		void SetMessage(string message);
		void SetMessage(Image image, string message);
		
		void SetCaretPosition(int x, int y, int charOffset);
		void SetInsertMode(bool insertMode);
		
	}
}
