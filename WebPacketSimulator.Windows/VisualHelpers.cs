using System;
using System.Collections.Generic;
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
        /// This function highlights the selected router
        /// </summary>
        /// <param name="router"> Router to be highlighted </param>
        public static void HighlightRouter(this WpfRouter router)
        {
            router.RouterImage.Source = new BitmapImage(new Uri("HighlightedRouter.png", UriKind.Relative));
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
            router.RouterImage.Source = new BitmapImage(new Uri("Router.png", UriKind.Relative));
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
        /// <param name="canvas"> Canvas with all controls </param>
        public async static Task SendPacket(WpfRouter source, WpfRouter destination, Canvas canvas, MainWindow mainWindow)
        {
            //Getting path to destination and setting up message image
            var path = source.Router.GetPathToRouter(WpfRouter.GetRouters().ToList(),
                                                     destination.Router);
            path.RemoveAt(0);
            var messageImage = MainWindow.MessageImage;
            messageImage.Margin = source.RouterImage.Margin;
            canvas.Children.Add(messageImage);
            Canvas.SetZIndex(messageImage, 1);

            //Animating the message
            foreach (var router in path)
            {
                var wpfRouter = WpfRouter.GetRouter(router, true);
                var currentMargin = wpfRouter.RouterImage.Margin;
                var animation = new ThicknessAnimation(){
                    From = messageImage.Margin,
                    To = currentMargin,
                    Duration = new Duration(TimeSpan.FromSeconds(1)),
                    FillBehavior = FillBehavior.Stop 
                };
                await mainWindow.Animate(animation);
                messageImage.Margin = wpfRouter.RouterImage.Margin;
            }
            canvas.Children.Remove(messageImage);
        }
    }
}