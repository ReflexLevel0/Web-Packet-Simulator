using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WebPacketSimulator.Wpf
{
    public class DeleteCommand : ICommand
    {
        public static DeleteCommand Instance = new DeleteCommand();
        public event EventHandler CanExecuteChanged;
        public DeleteCommand()
        {
            WpfRouter.HighlightedRoutersCollectionChanged += delegate
            {
                CanExecuteChanged_Invoke();
            };
            Connection.HighlightedConnectionsChanged += delegate
            {
                CanExecuteChanged_Invoke();
            };
        }
        public bool CanExecute(object parameter) =>
            MainCanvas.IsMessageAnimationRunning == false &&
            (WpfRouter.HighlightedRouters.Count != 0 ||
            Connection.HighlightedConnections.Count != 0);
        public void CanExecuteChanged_Invoke()
        {
            CanExecuteChanged.Invoke(this, new BoolEventArgs(CanExecute(null)));
        }
        public void Execute(object parameter)
        {
            var response = MessageBox.Show("Are you sure that you want to delete the selected objects?",
                                               "Delete query", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (response == MessageBoxResult.Yes)
            {
                while (WpfRouter.HighlightedRouters.Count > 0)
                {
                    WpfRouter.HighlightedRouters[0].Delete();
                }
                var highlightedConnections = (from connection in Connection.Connections
                                              where connection.ConnectionLine.Opacity != 1
                                              select connection).ToList();
                while (Connection.HighlightedConnections.Count > 0)
                {
                    highlightedConnections[0].Delete();
                    highlightedConnections.RemoveAt(0);
                }
            }
        }
    }
}