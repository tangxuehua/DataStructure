
using System;
using System.Collections;
using System.Windows.Forms;
using System.Xml;
using System.Threading;

using NetFocus.Components.AddIns;
using NetFocus.DataStructure.Commands;
using NetFocus.DataStructure.Services;
using NetFocus.DataStructure.Internal.Algorithm;
using NetFocus.DataStructure.Gui.Dialogs;


namespace NetFocus.DataStructure
{
    public class DataStructureMain
    {
        static void ShowErrorBox(object sender, ThreadExceptionEventArgs eargs)
        {
            DialogResult result = MessageBox.Show(eargs.Exception.Message, "¥ÌŒÛÃ· æ", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

            switch (result)
            {
                case DialogResult.Ignore:
                    break;
                case DialogResult.Abort:
                    Application.Exit();
                    break;
                case DialogResult.Retry:
                    break;
            }
        }

        [STAThread()]
        public static void Main(string[] args)
        {
            SplashScreenForm.SetCommandLineArgs(args);
            SplashScreenForm.SplashScreen.Show();
            Application.ThreadException += new ThreadExceptionEventHandler(ShowErrorBox);
            ArrayList commands = null;

            try
            {
                string[] addInDirectories = AddInSettingsHandler.GetAddInDirectories();
                AddInTreeSingleton.SetAddInDirectories(addInDirectories);

                commands = AddInTreeSingleton.AddInTree.GetTreeNode("/Workspace/Autostart").BuildChildItems(null);
                for (int i = 0; i < commands.Count - 1; ++i)
                {
                    ((ICommand)commands[i]).Run();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (SplashScreenForm.SplashScreen != null)
                {
                    SplashScreenForm.SplashScreen.Close();
                }
            }

            try
            {
                if (commands.Count > 0)
                {
                    ((ICommand)commands[commands.Count - 1]).Run();
                }
            }
            finally
            {
                ServiceManager.Services.UnloadAllServices();
            }
        }

    }
}


