using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WebPacketSimulator.Wpf
{
    public static class VisualHelpers
    {
        /// <summary>
        /// This function checks if a router image has been clicked
        /// </summary>
        /// <param name="clickLocation"> Click location (relative to image's parent (canvas)) </param>
        /// <param name="image"> Image which is being tested </param>
        /// <returns></returns>
        public static bool IsOnImage(this Point clickLocation, Image image) =>
            image.Margin.Left <= clickLocation.X &&
            image.Margin.Left + image.ActualWidth >= clickLocation.X &&
            image.Margin.Top <= clickLocation.Y &&
            image.Margin.Top + image.ActualHeight >= clickLocation.Y;

        /// <summary>
        /// This function checks if any of the routers have been clicked
        /// </summary>
        /// <param name="clickLocation"> Location of the click </param>
        /// <param name="routers"> Routers </param>
        /// <returns></returns>
        public static bool IsOnAnyImage(this Point clickLocation, List<WpfRouter> routers)
        {
            foreach (var image in routers.Select(router => router.RouterImage))
            {
                if (clickLocation.IsOnImage(image))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This function returns the default circle which is used for connecting routers
        /// </summary>
        /// <returns></returns>
        public static Ellipse GetDefaultCircleImage(Size connectionImageSize)
        {
            var circle = new Ellipse();
            circle.Stroke = Brushes.Black;
            circle.StrokeThickness = 1;
            circle.Height = connectionImageSize.Height;
            circle.Width = connectionImageSize.Width;
            return circle;
        }

        /// <summary>
        /// This function returns router on the chosen location (or null if there are no routers on the chosen location)
        /// </summary>
        /// <param name="location"> Location from which a router will be recieved </param>
        /// <returns></returns>
        public static WpfRouter GetRouterOnLocation(this Point location)
        {
            foreach (var router in WpfRouter.Routers)
            {
                if (location.IsOnImage(router.RouterImage))
                {
                    return router;
                }
            }
            return null;
        }

        /// <summary>
        /// This function is used for simulating message sending from one router to another
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public async static Task SendPacket(WpfRouter source, WpfRouter destination, MainWindow mainWindow)
        {
            //Getting path to destination and setting up message image
            var path = source.Router.GetPathToRouter(WpfRouter.GetRouters().ToList(),
                                                     destination.Router);
            path.RemoveAt(0);
            var messageImage = MainWindow.PacketImage;
            messageImage.Margin = source.RouterImage.Margin;
            MainWindow.Canvas.Children.Add(messageImage);
            Canvas.SetZIndex(messageImage, 1);

            //Animating the message
            foreach (var router in path)
            {
                var wpfRouter = WpfRouter.GetRouter(router, true);
                var currentMargin = wpfRouter.RouterImage.Margin;
                Thickness fromThickness = new Thickness(messageImage.Margin.Left + WpfRouter.RouterImageWidth / 2 - MainWindow.PacketImage.Width / 2,
                                         messageImage.Margin.Top + WpfRouter.RouterImageHeight / 2 - MainWindow.PacketImage.Height / 2,
                                         0, 0);
                Thickness toThickness = new Thickness(currentMargin.Left + WpfRouter.RouterImageWidth / 2 - MainWindow.PacketImage.Width / 2,
                                         currentMargin.Top + WpfRouter.RouterImageHeight / 2 - MainWindow.PacketImage.Height / 2,
                                         0, 0);
                double xDifference = Math.Abs(toThickness.Left - fromThickness.Left);
                double yDifference = Math.Abs(toThickness.Top - fromThickness.Top);
                double pathLength = Math.Sqrt(xDifference * xDifference + yDifference * yDifference);
                var animation = new ThicknessAnimation()
                {
                    From = fromThickness,
                    To = toThickness,
                    Duration = new Duration(TimeSpan.FromSeconds(pathLength / 250)),
                    FillBehavior = FillBehavior.Stop
                };
                await mainWindow.Animate(animation);
                messageImage.Margin = wpfRouter.RouterImage.Margin;
            }
            MainWindow.Canvas.Children.Remove(messageImage);
        }

        #region Highlight/unhighlight functions
        /// <summary>
        /// This function highlights the selected router
        /// </summary>
        /// <param name="router"> Router to be highlighted </param>
        public static void HighlightRouter(this WpfRouter router)
        {
            router.IsHighlighted = true;
            router.RouterImage.Source = new BitmapImage(new Uri("/Images/HighlightedRouter.png", UriKind.Relative));
            WpfRouter.HighlightedRouters.Add(router);
        }

        /// <summary>
        /// This function highlights all routers from a list of routers
        /// </summary>
        /// <param name="routers"> Routers to be highlighted </param>
        public static void HighlightAllRouters(this List<WpfRouter> routers)
        {
            while (routers.Count != 0)
            {
                routers[0].HighlightRouter();
            }
        }

        /// <summary>
        /// This function unhighlights the selected router
        /// </summary>
        /// <param name="router"> Router to be unhighlighted </param>
        public static void UnhighlightRouter(this WpfRouter router)
        {
            router.IsHighlighted = false;
            router.RouterImage.Source = new BitmapImage(new Uri("/Images/Router.png", UriKind.Relative));
            WpfRouter.HighlightedRouters.Remove(router);
        }

        /// <summary>
        /// This function unhighlights all routers from a list of routers
        /// </summary>
        /// <param name="routers"> Routers to be unhighlighted </param>
        public static void UnhighlightAllRouters(this List<WpfRouter> routers)
        {
            while (routers.Count != 0)
            {
                routers[0].UnhighlightRouter();
            }
        }

        /// <summary>
        /// This function highlights the line
        /// </summary>
        /// <param name="connectionLine"> Line to be highlighted </param>
        public static void HighlightLine(this Line connectionLine)
        {
            connectionLine.Opacity = Connection.HighlightedConnectionLineOpacity;
            Connection.HighlightedLines.Add(connectionLine);
        }

        /// <summary>
        /// This function highlights all lines in the list
        /// </summary>
        /// <param name="lines"> Lines to be highlighted </param>
        public static void HighlightAllLines(this IEnumerable<Line> lines)
        {
            foreach(var line in lines)
            {
                line.HighlightLine();
            }
        }

        /// <summary>
        /// This function unhighlights the line
        /// </summary>
        /// <param name="connectionLine"> Line to be unhighlighted </param>
        public static void UnhighlightLine(this Line connectionLine)
        {
            connectionLine.Opacity = 1;
            Connection.HighlightedLines.Remove(connectionLine);
        }

        /// <summary>
        /// This function unhighlights all lines in the list
        /// </summary>
        /// <param name="lines"> Lines to be unhighlighted </param>
        public static void UnhighlightAllLines(this IEnumerable<Line> lines)
        {
            while (lines.FirstOrDefault() != null)
            {
                lines.First().UnhighlightLine();
            }
        }
        #endregion
    }
}