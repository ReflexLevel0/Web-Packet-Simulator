using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WebPacketSimulator.Wpf
{
    public class OpenCommand : ICommand
    {
        public static OpenCommand Instance = new OpenCommand();
        public event EventHandler CanExecuteChanged = delegate { };
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter)
        {
            //Saving current work
            if (WpfRouter.Routers.Count > 0)
            {
                switch (VisualHelpers.SaveCurrentWorkQuery())
                {
                    case MessageBoxResult.Cancel:
                        return;
                    case MessageBoxResult.Yes:
                        SaveCommand.Instance.Execute(null);
                        break;
                }
            }

            //Opening the new file
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = FileHandler.FileDialogFilter;
            dialog.ShowDialog();
            if (string.IsNullOrEmpty(dialog.FileName))
            {
                return;
            }

            //Deleting current data and loading data from the file
            while (WpfRouter.Routers.Count > 0)
            {
                WpfRouter.Routers[0].Delete();
            }
            while (Connection.Connections.Count > 0)
            {
                Connection.Connections[0].Delete();
            }
            FileHandler.LoadFile(dialog.FileName);
            MainWindow.CurrentFilePath = dialog.FileName;
        }
    }
}
