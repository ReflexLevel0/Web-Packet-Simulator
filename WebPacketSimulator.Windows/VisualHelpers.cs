using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
        public static bool IsOnImage(this Point clickLocation, Canvas image) =>
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
            foreach(var image in routers.Select(router => router.RouterImage))
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
            router.RouterImage.Opacity = WpfRouter.HighlightedImageOpacity;
            WpfRouter.HighlightedRouters.Add(router);
        }

        /// <summary>
        /// This function highlights all routers from a list of routers
        /// </summary>
        /// <param name="routers"> Routers to be highlighted </param>
        public static void HighlightAllRouters(this List<WpfRouter> routers) =>
            routers.ForEach(new Action<WpfRouter>(router => HighlightRouter(router)));

        /// <summary>
        /// This function unhighlights the selected router
        /// </summary>
        /// <param name="router"> Router to be unhighlighted </param>
        public static void UnhighlightRouter(this WpfRouter router)
        {
            router.RouterImage.Opacity = 1;
            WpfRouter.HighlightedRouters.Remove(router);
        }

        /// <summary>
        /// This function unhighlights all routers from a list of routers
        /// </summary>
        /// <param name="routers"> Routers to be unhighlighted </param>
        public static void UnhighlightAllRouters(this List<WpfRouter> routers) =>
            routers.ForEach(new Action<WpfRouter>(router => UnhighlightRouter(router)));
    }
}
