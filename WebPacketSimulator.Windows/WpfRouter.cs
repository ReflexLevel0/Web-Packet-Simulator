using System;
using System.Collections.Generic;
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
        public Canvas RouterImage;
        public Router Router;
        public static Size RouterImageSize = new Size(50, 50);
        public static List<WpfRouter> HighlightedRouters = new List<WpfRouter>();
        public static List<WpfRouter> Routers = new List<WpfRouter>();
        public static WpfRouter LastClickedRouter = null;
        public static double HighlightedImageOpacity = 0.5;

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
        /// This function updates connection locations for the current router
        /// </summary>
        public void UpdateConnectionLocations()
        {
            //Getting all connections
            List<Ellipse> connections = new List<Ellipse>();
            foreach (var child in RouterImage.Children)
            {
                Ellipse circle = child as Ellipse;
                if (circle != null)
                {
                    connections.Add(circle);
                }
            }

            //Updating connection locations
            int connectionWidth = (int)ConnectionImageSize.Width;
            int connectionHeight = (int)ConnectionImageSize.Height;
            connections = connections.OrderBy(connection => connection.Margin.Top)
                                     .ThenBy(connection => connection.Margin.Left)
                                     .ToList();
            for (int i = 0; i < 4; i++)
            {
                int left = (int)(RouterImage.Margin.Left +
                                 connections[i].Margin.Left +
                                 connectionWidth / 2);
                int top = (int)(RouterImage.Margin.Top +
                                connections[i].Margin.Top +
                                connectionHeight / 2);
                switch (i)
                {
                    case 0:
                        TopConnectionLocation = new Point(left, top);
                        break;
                    case 1:
                        LeftConnectionLocation = new Point(left, top);
                        break;
                    case 2:
                        RightConnectionLocation = new Point(left, top);
                        break;
                    case 3:
                        BottomConnectionLocation = new Point(left, top);
                        break;
                }
            }
        }

        /// <summary>
        /// This function is used for connecting 2 routers
        /// </summary>
        /// <param name="routerA"> First router </param>
        /// <param name="routerB"> Second router </param>
        /// <param name="routerAConnectionLocation"> Connection location for the first router </param>
        /// <param name="routerBConnectionLocation"> Connection location for the second router </param>
        /// <returns></returns>
        public static Connection ConnectRouters(WpfRouter routerA, WpfRouter routerB, ConnectionLocations routerAConnectionLocation, ConnectionLocations routerBConnectionLocation)
        {
            var connection = new Connection()
            {
                SourceRouter = routerA,
                DestinationRouter = routerB,
                SourcePointLocation = routerAConnectionLocation,
                DestinationPointLocation = routerBConnectionLocation,
                ConnectionLine = new Line()
            };
            for (int i = 0; i < 2; i++)
            {
                string pathToProperty = "";
                switch ((i == 0) ? routerAConnectionLocation : routerBConnectionLocation)
                {
                    case ConnectionLocations.Bottom:
                        pathToProperty = nameof(WpfRouter.BottomConnectionLocation);
                        break;
                    case ConnectionLocations.Left:
                        pathToProperty = nameof(WpfRouter.LeftConnectionLocation);
                        break;
                    case ConnectionLocations.Right:
                        pathToProperty = nameof(WpfRouter.RightConnectionLocation);
                        break;
                    case ConnectionLocations.Top:
                        pathToProperty = nameof(WpfRouter.TopConnectionLocation);
                        break;
                }
                WpfRouter source = (i == 0) ? routerA : routerB;
                var binding = new Binding(pathToProperty + ".X") { Source = source };
                connection.ConnectionLine.SetBinding((i == 0) ? Line.X1Property : Line.X2Property, binding);
                binding = new Binding(pathToProperty + ".Y") { Source = source };
                connection.ConnectionLine.SetBinding((i == 0) ? Line.Y1Property : Line.Y2Property, binding);
            }
            connection.ConnectionLine.Stroke = System.Windows.Media.Brushes.Black;
            connection.ConnectionLine.StrokeThickness = 2;
            return connection;
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
                router.UpdateConnectionLocations();
            }
        }

        /// <summary>
        /// This function creates a router control for this router
        /// </summary>
        /// <returns></returns>
        public static WpfRouter CreateRouterControl(Point location)
        {
            //Creating router image and setting margins
            Point imageMargin = new Point((int)(location.X - RouterImageSize.Width / 2),
                                               (int)(location.Y - RouterImageSize.Height / 2));
            var newRouter = new WpfRouter()
            {
                Location = new System.Drawing.Point((int)imageMargin.X, (int)imageMargin.Y),
                RouterImage = new Canvas()
            };
            var image = new System.Windows.Controls.Image()
            {
                Source = new BitmapImage(new Uri("Router.png", UriKind.Relative)),
                Width = RouterImageSize.Width,
                Height = RouterImageSize.Height
            };
            image.Margin = new Thickness(
                    ConnectionImageSize.Width / 2,
                    ConnectionImageSize.Height / 2,
                    ConnectionImageSize.Width / 2,
                    ConnectionImageSize.Height / 2);
            newRouter.RouterImage.Children.Add(image);
            newRouter.RouterImage.Width = RouterImageSize.Width + ConnectionImageSize.Width;
            newRouter.RouterImage.Height = RouterImageSize.Height + ConnectionImageSize.Height;
            newRouter.RouterImage.Margin = new Thickness(imageMargin.X, imageMargin.Y, 0, 0);

            //Adding circles to router image (so other routers can be connected to it)
            for (int i = 0; i < 4; i++)
            {
                double left = 0;
                double top = 0;
                string tag = "";
                switch (i)
                {
                    case 0:
                        left = newRouter.RouterImage.Width / 2 - ConnectionImageSize.Width / 2;
                        tag = TopConnectionTag;
                        break;
                    case 1:
                        left = newRouter.RouterImage.Width - ConnectionImageSize.Width;
                        top = newRouter.RouterImage.Height / 2 - ConnectionImageSize.Height / 2;
                        tag = RightConnectionTag;
                        break;
                    case 2:
                        left = newRouter.RouterImage.Width / 2 - ConnectionImageSize.Width / 2;
                        top = newRouter.RouterImage.Height - ConnectionImageSize.Height;
                        tag = BottomConnectionTag;
                        break;
                    case 3:
                        top = newRouter.RouterImage.Height / 2 - ConnectionImageSize.Height / 2;
                        tag = LeftConnectionTag;
                        break;
                }
                var circle = VisualHelpers.GetDefaultCircleImage(ConnectionImageSize);
                circle.Margin = new Thickness(left, top, 0, 0);
                circle.Tag = tag;
                circle.Fill = System.Windows.Media.Brushes.Transparent;
                circle.MouseLeftButtonUp += Connection_MouseLeftButtonUp;
                newRouter.RouterImage.Children.Add(circle);
            }
            return newRouter;
        }

        private static void Connection_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WpfRouter clickedRouter = (from router in Routers
                                       let canvas = (sender as Ellipse).Parent as Canvas
                                       where router.RouterImage == canvas
                                       select router).First();
            var connectionLocation = TagToConnectionLocation((sender as Ellipse).Tag.ToString());

            //If this is the first part of the connection
            if (LastClickedRouter == null)
            {
                LastClickedRouter = clickedRouter;
                LastClickedConnectionLocation = connectionLocation;
            }
            //If this is second (last) part of the connection
            else
            {
                var connection = ConnectRouters(LastClickedRouter, clickedRouter, LastClickedConnectionLocation, connectionLocation);
                MainCanvas.Children.Add(connection.ConnectionLine);
                LastClickedRouter = null;
            }
        }
    }
}