using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WebPacketSimulator.Wpf
{
    public class NewFileCommand : ICommand
    {
        public static NewFileCommand Instance = new NewFileCommand();
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter)
        {
            switch (VisualHelpers.SaveCurrentWorkQuery())
            {
                case MessageBoxResult.Cancel:
                    return;
                case MessageBoxResult.Yes:
                    SaveCommand.Instance.Execute(null);
                    break;
            }
            WpfRouter.DeleteAll(WpfRouter.Routers);
            Connection.DeleteAll(Connection.Connections);
            MainWindow.CurrentFilePath = null;
        }
    }
}
