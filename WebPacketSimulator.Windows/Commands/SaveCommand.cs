using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WebPacketSimulator.Wpf
{
    public class SaveCommand : ICommand
    {
        public static SaveCommand Instance = new SaveCommand();
        public event EventHandler CanExecuteChanged = delegate { };
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter)
        {
            if (MainWindow.CurrentFilePath == null && WpfRouter.Routers.Count != 0)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = FileHandler.FileDialogFilter;
                dialog.ShowDialog();
                MainWindow.CurrentFilePath = dialog.FileName;
            }
            FileHandler.SaveFile(WpfRouter.Routers, Connection.Connections, MainWindow.CurrentFilePath);
        }
    }
}