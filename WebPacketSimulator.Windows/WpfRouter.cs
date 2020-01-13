using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using WebPacketSimulator.Common;
using Point = System.Windows.Point;

namespace WebPacketSimulator.Wpf
{
    public class WpfRouter : DependencyObject
    {
        public System.Drawing.Point Location;
        public Canvas RouterImage;
        public Router Router;

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
            int connectionWidth = (int)MainWindow.CircleImageSize.Width;
            int connectionHeight = (int)MainWindow.CircleImageSize.Height;
            connections = connections.OrderBy(connection => connection.Margin.Top)
                                     .ThenBy(connection => connection.Margin.Left)
                                     .ToList();
            for(int i = 0; i < 4; i++) {
                int left = (int)(RouterImage.Margin.Left + connections[i].Margin.Left);
                int top = (int)(RouterImage.Margin.Top + connections[i].Margin.Top);
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
    }
}