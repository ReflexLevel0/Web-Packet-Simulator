using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WebPacketSimulator.Wpf
{
    class UnhighlightCommand : ICommand
    {
        public static UnhighlightCommand Instance = new UnhighlightCommand();
        public event EventHandler CanExecuteChanged = delegate { };
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter)
        {
            WpfRouter.HighlightedRouters.UnhighlightAllRouters(true);
            Connection.HighlightedConnections.UnhighlightAllLines();
            if (MainCanvas.CurrentLine != null)
            {
                MainCanvas.Instance.Canvas.Children.Remove(MainCanvas.CurrentLine);
                WpfRouter.LastClickedRouter = null;
                MainCanvas.CurrentLine = null;
            }
        }
    }
}
