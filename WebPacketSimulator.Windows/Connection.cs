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
        public enum ConnectionLocations { Top, Right, Bottom, Left }
        public ConnectionLocations SourcePointLocation;
        public ConnectionLocations DestinationPointLocation;
        public WpfRouter SourceRouter;
        public WpfRouter DestinationRouter;
        public Line ConnectionLine = new Line();
        public static string TopConnectionTag = "Top";
        public static string RightConnectionTag = "Right";
        public static string LeftConnectionTag = "Left";
        public static string BottomConnectionTag = "Bottom";
        public static Size ConnectionImageSize = new Size(20, 20);
        public static ConnectionLocations LastClickedConnectionLocation;

        public static ConnectionLocations TagToConnectionLocation(string tag)
        {
            if (tag.CompareTo(TopConnectionTag) == 0)
            {
                return ConnectionLocations.Top;
            }
            if (tag.CompareTo(RightConnectionTag) == 0)
            {
                return ConnectionLocations.Right;
            }
            if (tag.CompareTo(BottomConnectionTag) == 0)
            {
                return ConnectionLocations.Bottom;
            }
            if (tag.CompareTo(LeftConnectionTag) == 0)
            {
                return ConnectionLocations.Left;
            }
            throw new Exception("Unknown connection location tag!");
        }
    }
}