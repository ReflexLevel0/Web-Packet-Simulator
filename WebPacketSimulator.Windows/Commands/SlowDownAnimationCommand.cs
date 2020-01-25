using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WebPacketSimulator.Wpf
{
    public class SlowDownAnimationCommand : ICommand
    {
        public static SlowDownAnimationCommand Instance = new SlowDownAnimationCommand();
        public event EventHandler CanExecuteChanged = delegate { };
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter)
        {
            AnimationSpeed.DecreaseSpeed();
        }
    }
}
