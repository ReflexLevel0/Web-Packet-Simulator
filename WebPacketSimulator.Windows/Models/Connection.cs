using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public static ObservableCollection<Line> HighlightedConnections = new ObservableCollection<Line>();
        public static event EventHandler HighlightedConnectionsChanged;
        public static double HighlightedConnectionLineOpacity = 0.5f;
        public static double ConnectionLineWidth = 3;
        public static double BackupConnectionLineWidth = 25;

        static Connection()
        {
            HighlightedConnections.CollectionChanged += delegate
            {
                HighlightedConnectionsChanged.Invoke(null, null);
            };
        }

        /// <summary>
        /// This function is used to delete a connection
        /// </summary>
        public void Delete()
        {
            SourceRouter.Router.LinkedRouters.Remove(DestinationRouter.Router);
            DestinationRouter.Router.LinkedRouters.Remove(SourceRouter.Router);
            var connectionsToRemove = GetConnections(SourceRouter, DestinationRouter).ToList();
            while(connectionsToRemove.Count > 0)
            {
                var connection = connectionsToRemove[0];
                HighlightedConnections.Remove(connection.ConnectionLine);
                MainCanvas.Instance.Canvas.Children.Remove(connection.ConnectionLine);
                MainCanvas.Instance.Canvas.Children.Remove(connection.BackupConnectionLine);
                Connections.Remove(connection);
                connectionsToRemove.RemoveAt(0);
            }
        }

        /// <summary>
        /// Deletes all connections in a list
        /// </summary>
        /// <param name="connections"> Connections to be deleted </param>
        public static void DeleteAll(IEnumerable<Connection> connections)
        {
            while(connections.Count() > 0)
            {
                connections.First().Delete();
            }
        }

        /// <summary>
        /// This function gets all connections between two routers
        /// </summary>
        /// <param name="routerA"></param>
        /// <param name="routerB"></param>
        /// <returns></returns>
        public static IEnumerable<Connection> GetConnections(WpfRouter routerA, WpfRouter routerB) =>
            from connection in Connections
            where (connection.SourceRouter == routerA &&
            connection.DestinationRouter == routerB) ||
            (connection.SourceRouter == routerB &&
            connection.DestinationRouter == routerA)
            select connection;
    }
}