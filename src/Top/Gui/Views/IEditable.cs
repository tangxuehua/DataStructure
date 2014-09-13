
namespace NetFocus.DataStructure.Gui
{
	public interface IEditable
	{
		IClipboardHandler ClipboardHandler {
			get;
		}
		
		bool EnableUndo {
			get;
		}
		
		bool EnableRedo {
			get;
		}
		
		string Text {
			get;
			set;
		}
		
		void Undo();
		void Redo();
		
	}
}
