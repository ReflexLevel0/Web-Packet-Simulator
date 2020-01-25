using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WebPacketSimulator.Wpf
{
    public class SpeedUpAnimationCommand : ICommand
    {
        public static SpeedUpAnimationCommand Instance = new SpeedUpAnimationCommand();
        public event EventHandler CanExecuteChanged = delegate { };
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter)
        {
            AnimationSpeed.IncreaseSpeed();
        }
    }
}