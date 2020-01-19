﻿using System;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WebPacketSimulator.Common;
using static WebPacketSimulator.Wpf.Connection;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace WebPacketSimulator.Wpf
{
    public class WpfRouter : DependencyObject
    {
        public System.Drawing.Point Location;
        public System.Windows.Controls.Image RouterImage =
            new System.Windows.Controls.Image()
            {
                Width = RouterImageWidth,
                Height = RouterImageHeight,
                AllowDrop = true
            };
        public Router Router { get; set; }
        public static double RouterImageWidth { get; } = 50;
        public static double RouterImageHeight { get; } = 50;
        public static List<WpfRouter> HighlightedRouters = new List<WpfRouter>();
        public static ObservableCollection<WpfRouter> Routers = new ObservableCollection<WpfRouter>();
        public static WpfRouter LastClickedRouter = null;

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
            if (routerA == routerB)
            {
                throw new Exception("Can't connect the router to itself!");
            }

            DisconnectRouters(routerA, routerB);
            var connection = new Connection()
            {
                SourceRouter = routerA,
                DestinationRouter = routerB,
                ConnectionLine = new Line()
            };
            for (int i = 0; i < 2; i++)
            {
                WpfRouter source = (i == 0) ? routerA : routerB;
                var binding = new Binding("RouterImage.Margin.Left") { Source = source };
                connection.ConnectionLine.SetBinding((i == 0) ? Line.X1Property : Line.X2Property, binding);
                binding = new Binding("RouterImage.Margin.Top") { Source = source };
                connection.ConnectionLine.SetBinding((i == 0) ? Line.Y1Property : Line.Y2Property, binding);
            }
            connection.ConnectionLine.Stroke = System.Windows.Media.Brushes.Black;
            connection.ConnectionLine.StrokeThickness = 2;
            routerA.Router.LinkedRouters.Add(routerB.Router);
            routerB.Router.LinkedRouters.Add(routerA.Router);
            Connections.Add(connection);
            MainWindow.Canvas.Children.Add(connection.ConnectionLine);
            MainWindow.Canvas.Children.Add(new Line() { X1 = routerA.RouterImage.Margin.Left, Y1 = routerA.RouterImage.Margin.Top, X2 = routerB.RouterImage.Margin.Left + 50, Y2 = routerB.RouterImage.Margin.Top + 50, Stroke = System.Windows.Media.Brushes.Black, StrokeThickness = 2 });
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
                int leftMargin = (int)(router.RouterImage.Margin.Left + moveAmmount.X);
                int topMargin = (int)(router.RouterImage.Margin.Top + moveAmmount.Y);
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
                Location = new System.Drawing.Point((int)imageMargin.X, (int)imageMargin.Y),
                Router = new Router()
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
    }
}