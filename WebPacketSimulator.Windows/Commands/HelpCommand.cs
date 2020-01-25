using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WebPacketSimulator.Wpf
{
    public class HelpCommand : ICommand
    {
        public static HelpCommand Instance = new HelpCommand(); 

        public event EventHandler CanExecuteChanged = delegate { };

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            new HelpWindow().Show();
        }
    }
}