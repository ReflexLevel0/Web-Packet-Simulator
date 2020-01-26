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
        public Canvas RouterCanvas = new Canvas();
        public System.Windows.Controls.Image RouterImage;
        public TextBlock RouterNameTextBlock = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Center };
        public TextBlock RouterAddressTextBlock = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Center };
        public StackPanel RouterStackPanel = new StackPanel();
        public Router Router { get; set; }
        public static double RouterImageWidth { get; } = 50;
        public static double RouterImageHeight { get; } = 50;
        public static ObservableCollection<WpfRouter> HighlightedRouters = new ObservableCollection<WpfRouter>();
        public static ObservableCollection<WpfRouter> Routers = new ObservableCollection<WpfRouter>();
        public static EventHandler HighlightedRoutersCollectionChanged;
        public static WpfRouter LastClickedRouter = null;
        public bool IsHighlighted;

        static WpfRouter()
        {
            HighlightedRouters.CollectionChanged += delegate 
            { 
                if(HighlightedRoutersCollectionChanged != null)
                {
                    HighlightedRoutersCollectionChanged.Invoke(null, null);
                }
            };
        }

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
                var currentRouter = (i == 0) ? routerA : routerB;
                var binding = new Binding("Margin") 
                { 
                    Source = currentRouter.RouterCanvas, 
                    Converter = new LineMarginToRouterCenter(), 
                    ConverterParameter = new LineMarginToRouterConverterParameter() 
                    { 
                        Left = true, 
                        Router = currentRouter
                    } 
                };
                connection.ConnectionLine.SetBinding((i == 0) ? Line.X1Property : Line.X2Property, binding);
                connection.BackupConnectionLine.SetBinding((i == 0) ? Line.X1Property : Line.X2Property, binding);
                binding = new Binding("Margin") 
                { 
                    Source = currentRouter.RouterCanvas, 
                    Converter = new LineMarginToRouterCenter(), 
                    ConverterParameter = new LineMarginToRouterConverterParameter() 
                    { 
                        Left = false,
                        Router = currentRouter
                    } 
                };
                connection.ConnectionLine.SetBinding((i == 0) ? Line.Y1Property : Line.Y2Property, binding);
                connection.BackupConnectionLine.SetBinding((i == 0) ? Line.Y1Property : Line.Y2Property, binding);
                currentRouter.Router.NameChanged += delegate
                {
                    ManuallyUpdateMargin(currentRouter);
                };
                currentRouter.Router.AddressChanged += delegate
                {
                    ManuallyUpdateMargin(currentRouter);
                };
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
            MainCanvas.Instance.Canvas.Children.Add(connection.ConnectionLine);
            MainCanvas.Instance.Canvas.Children.Add(connection.BackupConnectionLine);
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
                MainCanvas.Instance.Canvas.Children.Remove(_connection.ConnectionLine);
                Connections.Remove(_connection);
            }
        }

        /// <summary>+
        /// This function moves the selected routers for a desired ammount
        /// </summary>
        /// <param name="routers"> Router which will be moved </param>
        /// <param name="moveAmmount"> Ammount (in x and y coordinates) for which a router will be moved </param>
        public static void MoveRouters(IEnumerable<WpfRouter> routers, Point moveAmmount)
        {
            for (int i = 0; i < routers.Count(); i++)
            {
                var router = routers.ElementAt(i);
                double leftMargin = router.RouterCanvas.Margin.Left + moveAmmount.X;
                double topMargin = router.RouterCanvas.Margin.Top + moveAmmount.Y;
                router.RouterCanvas.Margin = new Thickness(leftMargin, topMargin, 0, 0);
            }
        }

        /// <summary>
        /// This function creates a router with the given parameter
        /// </summary>
        /// <param name="address"> Router's IP address </param>
        /// <param name="location"> Router's location on canvas </param>
        /// <param name="name"> Name of the router </param>
        /// <returns></returns>
        public static WpfRouter CreateRouter(Point location, string address = null, string name = null)
        {
            //Creating router image and setting margins
            Point imageMargin = new Point((int)location.X,
                                               (int)location.Y);
            var newRouter = new WpfRouter()
            {
                Router = new Router() { Address = address, Name = name },
                RouterImage = new System.Windows.Controls.Image()
                {
                    Width = RouterImageWidth,
                    Height = RouterImageHeight,
                    HorizontalAlignment = HorizontalAlignment.Center
                }
            };
            newRouter.RouterNameTextBlock.Text = newRouter.Router.Name;
            newRouter.RouterAddressTextBlock.Text = newRouter.Router.Address;
            newRouter.RouterCanvas.Margin = new Thickness(imageMargin.X, imageMargin.Y, 0, 0);
            newRouter.RouterCanvas.Children.Add(newRouter.RouterStackPanel);
            newRouter.RouterStackPanel.Children.Add(newRouter.RouterImage);
            newRouter.RouterStackPanel.Children.Add(newRouter.RouterNameTextBlock);
            newRouter.RouterStackPanel.Children.Add(newRouter.RouterAddressTextBlock);
            newRouter.Router.NameChanged += delegate 
            { 
                newRouter.RouterNameTextBlock.Text = newRouter.Router.Name; 
            };
            newRouter.Router.AddressChanged += delegate 
            { 
                newRouter.RouterAddressTextBlock.Text = newRouter.Router.Address; 
            };
            Routers.Add(newRouter);
            newRouter.UnhighlightRouter(false);
            MainCanvas.Instance.Canvas.Children.Add(newRouter.RouterCanvas);
            Canvas.SetZIndex(newRouter.RouterCanvas, 1);
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
            MainCanvas.Instance.Canvas.Children.Remove(RouterCanvas);
            this.UnhighlightRouter(true);
            Routers.Remove(this);
            var connections = (from connection in Connections
                               where connection.SourceRouter == this
                               || connection.DestinationRouter == this
                               select connection).ToList();
            while (connections.Count > 0)
            {
                connections[0].Delete();
                connections.RemoveAt(0);
            }
        }

        /// <summary>
        /// This function gets called when the connection gets clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void ConnectionLine_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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
                HighlightedConnections.UnhighlightAllLines();
                connection.ConnectionLine.HighlightLine();
            }
            e.Handled = true;
        }

        /// <summary>
        /// This function manually updates margin property of the router (this is used so that locations of connection lines update after route's data changes (otherwise if text was too large, router's image would move to center of the stack panel and connection would stay on the same location))
        /// </summary>
        /// <param name="router"></param>
        static void ManuallyUpdateMargin(WpfRouter router)
        {
            var margin = router.RouterCanvas.Margin;
            var newMargin = new Thickness(margin.Left, margin.Top, margin.Right, margin.Bottom - 1);
            router.RouterCanvas.Margin = newMargin;
            newMargin.Bottom += 1;
            router.RouterCanvas.Margin = newMargin;
        }
    }
}