using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using static WebPacketSimulator.Wpf.MainWindow;

namespace WebPacketSimulator.Wpf
{
    public class Connection : DependencyObject
    {
        public static List<Connection> Connections = new List<Connection>();
        public WpfRouter SourceRouter;
        public WpfRouter DestinationRouter;
        public Line ConnectionLine = new Line();
        public static string TopConnectionTag = "Top";
        public static string RightConnectionTag = "Right";
        public static string LeftConnectionTag = "Left";
        public static string BottomConnectionTag = "Bottom";
    }
}