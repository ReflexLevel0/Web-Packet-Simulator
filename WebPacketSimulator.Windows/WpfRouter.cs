using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WebPacketSimulator.Common;
using static WebPacketSimulator.Wpf.Connection;
using Point = System.Windows.Point;
using Size = System.Windows.Size;
using Color = System.Windows.Media.Color;
using System.Diagnostics;

namespace WebPacketSimulator.Wpf
{
    public class WpfRouter : DependencyObject
    {
        public System.Windows.Controls.Image RouterImage;
        public Router Router { get; set; }
        public static double RouterImageWidth { get; } = 50;
        public static double RouterImageHeight { get; } = 50;
        public static List<WpfRouter> HighlightedRouters = new List<WpfRouter>();
        public static ObservableCollection<WpfRouter> Routers = new ObservableCollection<WpfRouter>();
        public static WpfRouter LastClickedRouter = null;
        public bool IsHighlighted;

        #region Connection dependecy properties
        public static readonly DependencyProperty TopConnectionLocationProperty =
            DependencyProperty.Register(nameof(TopConnectionLocation),
                                        typeof(Point),
                                        typeof(WpfRouter));
        public Point TopConnectionLocation
        {
            get => (Point)GetValue(TopConnectionLocationProperty);
            set => SetValue(TopConnectionLocationProperty, value);
        }

        public static readonly DependencyProperty LeftConnectionLocationProperty =
            DependencyProperty.Register(nameof(LeftConnectionLocation),
                                        typeof(Point),
                                        typeof(WpfRouter));
        public Point LeftConnectionLocation
        {
            get => (Point)GetValue(LeftConnectionLocationProperty);
            set => SetValue(LeftConnectionLocationProperty, value);
        }

        public static readonly DependencyProperty RightConnectionLocationProperty =
            DependencyProperty.Register(nameof(RightConnectionLocation),
                                        typeof(Point),
                                        typeof(WpfRouter));
        public Point RightConnectionLocation
        {
            get => (Point)GetValue(RightConnectionLocationProperty);
            set => SetValue(RightConnectionLocationProperty, value);
        }

        public static readonly DependencyProperty BottomConnectionLocationProperty =
            DependencyProperty.Register(nameof(BottomConnectionLocation),
                                        typeof(Point),
                                        typeof(WpfRouter));
        public Point BottomConnectionLocation
        {
            get => (Point)GetValue(BottomConnectionLocationProperty);
            set => SetValue(BottomConnectionLocationProperty, value);
        }
        #endregion

        /// <summary>
        /// This function is used for connecting 2 routers
        /// </summary>
        /// <param name="routerA"> First router </param>
        /// <param name="routerB"> Second router </param>
        /// <returns> Object that represents the connection between the two connected routers </returns>
        public static void ConnectRouters(WpfRouter routerA, WpfRouter routerB)
        {
            //Setup
            LastClickedRouter = null;
            if (routerA == routerB)
            {
                throw new Exception("Can't connect the router to itself!");
            }
            DisconnectRouters(routerA, routerB);

            //Creating the connection
            var connection = new Connection()
            {
                SourceRouter = routerA,
                DestinationRouter = routerB,
                ConnectionLine = new Line(),
                BackupConnectionLine = new Line()
            };
            for (int i = 0; i < 2; i++)
            {
                var source = (i == 0) ? routerA.RouterImage : routerB.RouterImage;
                var binding = new Binding("Margin") { Source = source, Converter = new LineMarginToRouterCenter(), ConverterParameter = true };
                connection.ConnectionLine.SetBinding((i == 0) ? Line.X1Property : Line.X2Property, binding);
                binding = new Binding("Margin") { Source = source, Converter = new LineMarginToRouterCenter(), ConverterParameter = false };
                connection.ConnectionLine.SetBinding((i == 0) ? Line.Y1Property : Line.Y2Property, binding);
                binding = new Binding("Margin") { Source = source, Converter = new BackupLineMarginToRouterConverter(), ConverterParameter = true };
                connection.BackupConnectionLine.SetBinding((i == 0) ? Line.X1Property : Line.X2Property, binding);
                binding = new Binding("Margin") { Source = source, Converter = new BackupLineMarginToRouterConverter(), ConverterParameter = false };
                connection.BackupConnectionLine.SetBinding((i == 0) ? Line.Y1Property : Line.Y2Property, binding);
            }
            connection.ConnectionLine.Stroke = System.Windows.Media.Brushes.Black;
            connection.ConnectionLine.StrokeThickness = ConnectionLineWidth;
            connection.BackupConnectionLine.Stroke = System.Windows.Media.Brushes.Transparent;
            connection.BackupConnectionLine.StrokeThickness = BackupConnectionLineWidth;
            connection.BackupConnectionLine.MouseLeftButtonUp += ConnectionLine_MouseLeftButtonUp;

            //Connecting the routers
            routerA.Router.LinkedRouters.Add(routerB.Router);
            routerB.Router.LinkedRouters.Add(routerA.Router);
            Connections.Add(connection);
            MainWindow.Canvas.Children.Add(connection.ConnectionLine);
            MainWindow.Canvas.Children.Add(connection.BackupConnectionLine);
        }

        /// <summary>
        /// This function is used for disconnecting 2 routers
        /// </summary>
        /// <param name="routerA"> First router </param>
        /// <param name="routerB"> Second router </param>
        /// <returns></returns>
        public static void DisconnectRouters(WpfRouter routerA, WpfRouter routerB)
        {
            routerA.Router.LinkedRouters.Remove(routerB.Router);
            routerB.Router.LinkedRouters.Remove(routerA.Router);
            var connectionsToRemove = (from connection in Connections
                                       where
        (connection.SourceRouter == routerA && connection.DestinationRouter == routerB) ||
        (connection.SourceRouter == routerB && connection.DestinationRouter == routerA)
                                       select connection).ToList();
            foreach (var _connection in connectionsToRemove)
            {
                MainWindow.Canvas.Children.Remove(_connection.ConnectionLine);
                Connections.Remove(_connection);
            }
        }

        /// <summary>
        /// This function moves the selected routers for a desired ammount
        /// </summary>
        /// <param name="routers"> Router which will be moved </param>
        /// <param name="moveAmmount"> Ammount (in x and y coordinates) for which a router will be moved </param>
        public static void MoveRouters(List<WpfRouter> routers, Point moveAmmount)
        {
            for (int i = 0; i < routers.Count; i++)
            {
                var router = routers[i];
                double leftMargin = router.RouterImage.Margin.Left + moveAmmount.X;
                double topMargin = router.RouterImage.Margin.Top + moveAmmount.Y;
                router.RouterImage.Margin = new Thickness(leftMargin, topMargin, 0, 0);
            }
        }

        /// <summary>
        /// This function creates a router control for this router
        /// </summary>
        /// <returns></returns>
        public static WpfRouter CreateRouter(Point location)
        {
            //Creating router image and setting margins
            Point imageMargin = new Point((int)(location.X - RouterImageWidth / 2),
                                               (int)(location.Y - RouterImageHeight / 2));
            var newRouter = new WpfRouter()
            {
                Router = new Router(),
                RouterImage = new System.Windows.Controls.Image()
                {
                    Margin = new Thickness(imageMargin.X, imageMargin.Y, 0, 0),
                    Width = RouterImageWidth,
                    Height = RouterImageHeight
                }
            };
            newRouter.HighlightRouter();
            newRouter.Router.Name = "Test" + new Random().Next().ToString();
            return newRouter;
        }

        /// <summary>
        /// This function returns the router with the specified name
        /// </summary>
        /// <param name="routerName"> Name of the reouter which will be returned </param>
        /// <param name="useForce"> Is this parameter is null, exception will be thrown if router with specified name wasn't found </param>
        /// <returns></returns>
        public static WpfRouter GetRouter(string routerName, bool useForce)
        {
            WpfRouter result = (from router in Routers
                                where router.Router.Name.CompareTo(routerName) == 0
                                select router).FirstOrDefault();
            if (useForce == true && result == null)
            {
                throw new Exception("Router with name \"" + routerName + "\" wasn't found!");
            }
            return result;
        }

        /// <summary>
        /// This function gets a WpfRouter which represents a router
        /// </summary>
        /// <param name="router"> Router that is being searched for </param>
        /// <param name="useForce"> If true, exception will be thrown if router wasn't found </param>
        /// <returns></returns>
        public static WpfRouter GetRouter(Router router, bool useForce)
        {
            var result = (from _router in Routers
                          where _router.Router == router
                          select _router).FirstOrDefault();
            if (result == null && useForce == true)
            {
                throw new Exception();
            }
            return result;
        }

        /// <summary>
        /// This function returns all routers (not WpfRouters, but ordinary Routers)
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Router> GetRouters() =>
            from router in Routers
            select router.Router;

        /// <summary>
        /// This function deletes this router and removes it from the canvas
        /// </summary>
        public void Delete()
        {
            MainWindow.Canvas.Children.Remove(RouterImage);
            this.UnhighlightRouter();
            Routers.Remove(this);
        }

        /// <summary>
        /// This function gets called when the connection gets clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ConnectionLine_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Y");

            var clickedLine = (sender as Line);
            var connection = (from _connection in Connections
                              where _connection.ConnectionLine == clickedLine || 
                              _connection.BackupConnectionLine == clickedLine
                              select _connection).First();
            clickedLine = connection.ConnectionLine;
            bool isCtrlClicked = Keyboard.IsKeyDown(Key.LeftCtrl) == true ||
                                 Keyboard.IsKeyDown(Key.RightCtrl) == true;
            if (isCtrlClicked)
            {
                if(clickedLine.Opacity == 1)
                {
                    connection.ConnectionLine.HighlightLine();
                }
                else
                {
                    connection.ConnectionLine.UnhighlightLine();
                }
            }
            else
            {
                HighlightedLines.UnhighlightAllLines();
                connection.ConnectionLine.HighlightLine();
            }
            e.Handled = true;
        }
    }
}