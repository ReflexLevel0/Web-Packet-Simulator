using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebPacketSimulator.Wpf
{
    public class BoolEventArgs : EventArgs
    {
        public bool Result;
        public BoolEventArgs(bool result)
        {
            Result = result;
        }
    }
}