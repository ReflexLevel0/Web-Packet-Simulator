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
        public static bool IsOnImage(this Point clickLocation, Thickness routerMargin) =>
            routerMargin.Left <= clickLocation.X &&
            routerMargin.Left + WpfRouter.RouterImageWidth >= clickLocation.X &&
            routerMargin.Top <= clickLocation.Y &&
            routerMargin.Top + WpfRouter.RouterImageHeight >= clickLocation.Y;

        /// <summary>
        /// This function checks if any of the routers have been clicked
        /// </summary>
        /// <param name="clickLocation"> Location of the click </param>
        /// <param name="routers"> Routers </param>
        /// <returns></returns>
        public static bool IsOnAnyImage(this Point clickLocation, List<WpfRouter> routers)
        {
            foreach (var image in routers.Select(router => router.RouterCanvas.Margin))
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
                if (location.IsOnImage(router.RouterCanvas.Margin))
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
            messageImage.Margin = source.RouterCanvas.Margin;
            MainWindow.CurrentMainWindow.MainCanvas.Children.Add(messageImage);
            Canvas.SetZIndex(messageImage, 1);
            var lastRouter = source;

            //Animating the message
            foreach (var router in path)
            {
                var destinationRouter = WpfRouter.GetRouter(router, true);
                await AnimatePacket(lastRouter, destinationRouter, destinationRouter.Router == path[0]);
                lastRouter = destinationRouter;
            }
            MainWindow.CurrentMainWindow.MainCanvas.Children.Remove(messageImage);
            MainWindow.UpdatePacketConsole(path);
        }

        /// <summary>
        /// This function updates the visibility of the router data (the data which is used to modify router data)
        /// </summary>
        static void UpdateRouterDataVisibility()
        {
            var mainWindow = (Application.Current.MainWindow as MainWindow);
            if (WpfRouter.HighlightedRouters.Count == 1)
            {
                mainWindow.ShowRouterData();
            }
            else
            {
                mainWindow.HideRouterData();
            }
        }

        /// <summary>
        /// This function is used to simulate sending a packet from a location to the router (THE ROUTER MUST BE NEIGHBORS!!!)
        /// </summary>
        /// <param name="destinationRouter"> Router to which the packet will be sent to </param>
        /// <param name="sourceRouter"> Router from which the packet is being sent </param>
        /// <returns></returns>
        public static Task AnimatePacket(WpfRouter sourceRouter, WpfRouter destinationRouter, bool isFirstAnimation)
        {
            if(MainWindow.IsMessageAnimationRunning == true)
            {
                MainWindow.UpdatePacketConsole();
            }

            if(sourceRouter.Router.LinkedRouters.Contains(destinationRouter.Router) == false)
            {
                throw new Exception("Unknown error has occured!");
            }

            TaskCompletionSource<bool> taskCompleted = new TaskCompletionSource<bool>();

            //Calculations necessary for the animation
            var fromMargin = sourceRouter.RouterCanvas.Margin;
            var toMargin = destinationRouter.RouterCanvas.Margin;
            Thickness fromThickness = new Thickness(fromMargin.Left + WpfRouter.RouterImageWidth / 2 - MainWindow.PacketImage.Width / 2,
                                     fromMargin.Top + WpfRouter.RouterImageHeight / 2 - MainWindow.PacketImage.Height / 2,
                                     0, 0);
            Thickness toThickness = new Thickness(toMargin.Left + WpfRouter.RouterImageWidth / 2 - MainWindow.PacketImage.Width / 2,
                                     toMargin.Top + WpfRouter.RouterImageHeight / 2 - MainWindow.PacketImage.Height / 2,
                                     0, 0);
            double xDifference = Math.Abs(toThickness.Left - fromThickness.Left);
            double yDifference = Math.Abs(toThickness.Top - fromThickness.Top);
            double pathLength = Math.Sqrt(xDifference * xDifference + yDifference * yDifference);

            //Animation setup and start
            var animation = new ThicknessAnimation()
            {
                From = fromThickness,
                To = toThickness,
                Duration = new Duration(TimeSpan.FromSeconds(pathLength / 250 * MainWindow.CurrentMainWindow.AnimationSpeed)),
                FillBehavior = FillBehavior.Stop
            };
            EventHandler OnCompleted = null;
            OnCompleted = delegate
            {
                taskCompleted.SetResult(true);
                animation.Completed -= OnCompleted;
                MainWindow.IsMessageAnimationRunning = false;
                MainWindow.PacketImage.Margin = toThickness;
                MainWindow.UpdatePacketConsole(sourceRouter.Router, destinationRouter.Router, isFirstAnimation);
            };
            animation.Completed += OnCompleted;
            MainWindow.IsMessageAnimationRunning = true;
            MainWindow.PacketImage.BeginAnimation(Image.MarginProperty, animation, HandoffBehavior.SnapshotAndReplace);
            return taskCompleted.Task;
        }

        /// <summary>
        /// This function presents a dialog which is used for saving current work
        /// </summary>
        /// <returns></returns>
        public static MessageBoxResult SaveCurrentWorkQuery() =>
            MessageBox.Show("Do you want to save the current work?", "Save current work", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

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
            UpdateRouterDataVisibility();
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
            UpdateRouterDataVisibility();
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