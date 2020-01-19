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
        public Line ConnectionLine;
        public Line BackupConnectionLine;
        public static List<Line> HighlightedLines = new List<Line>();
        public static double HighlightedConnectionLineOpacity = 0.5f;
        public static double ConnectionLineWidth = 3;
        public static double BackupConnectionLineWidth = 15;

        /// <summary>
        /// This function deletes this connection
        /// </summary>
        public void Delete()
        {
            HighlightedLines.Remove(ConnectionLine);
            Canvas.Children.Remove(ConnectionLine);
            Canvas.Children.Remove(BackupConnectionLine);
            Connections.Remove(this);
        }
    }
}