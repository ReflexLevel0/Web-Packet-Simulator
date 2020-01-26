﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        public static List<Line> HighlightedConnections = new List<Line>();
        public static double HighlightedConnectionLineOpacity = 0.5f;
        public static double ConnectionLineWidth = 3;
        public static double BackupConnectionLineWidth = 15;

        /// <summary>
        /// This function deletes this connection
        /// </summary>
        public void Delete()
        {
            Canvas canvas = MainCanvas.Instance.Canvas;
            HighlightedConnections.Remove(ConnectionLine);
            canvas.Children.Remove(ConnectionLine);
            canvas.Children.Remove(BackupConnectionLine);
            Connections.Remove(this);
        }
    }
}