using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebPacketSimulator.Common;
using static WebPacketSimulator.Wpf.Connection;
using Point = System.Drawing.Point;

namespace WebPacketSimulator.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables
        public static Size RouterImageSize = new Size(50, 50);
        public static Size CircleImageSize = new Size(7.5f, 7.5f);
        Point previousMousePosition = new Point();
        List<WpfRouter> highlightedRouters
            = new List<WpfRouter>();
        List<WpfRouter> routers
            = new List<WpfRouter>();
        #endregion

        public MainWindow()
        {
            Binding binding = new Binding("TopConnectionLocation.Y");
            binding.Source = new WpfRouter() { TopConnectionLocation = new System.Windows.Point(100,200) };
            binding.Mode = BindingMode.TwoWay;
            Connection connection = new Connection();
            connection.ConnectionLine.SetBinding(Line.X1Property, binding);
            MessageBox.Show(connection.ConnectionLine.X1.ToString());
            InitializeComponent();
        }

        #region Events
        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(MainCanvas);

            //Highlighting the router on the click position
            WpfRouter highlightedRouter = null;
            bool clickedOnRouter = false;
            foreach (var router in routers)
            {
                if (IsPointOnImage(position, router.RouterImage))
                {
                    highlightedRouter = router;
                    clickedOnRouter = true;
                    break;
                }
            }
            if (clickedOnRouter)
            {
                bool isCtrlClicked =
                    Keyboard.IsKeyDown(Key.LeftCtrl) == true ||
                    Keyboard.IsKeyDown(Key.RightCtrl) == true;

                //Highlighting/unhighlighting the clicked router if ctrl is pressed
                if (isCtrlClicked == true)
                {
                    if (highlightedRouter.RouterImage.Opacity == 1)
                    {
                        highlightedRouters.Add(highlightedRouter);
                        highlightedRouter.RouterImage.Opacity = 0.5f;
                    }
                    else
                    {
                        highlightedRouters.Remove(highlightedRouter);
                        highlightedRouter.RouterImage.Opacity = 1;
                    }
                }
                //Unhighlighting all routers except the clicked one if ctrl isn't clicked
                else if (highlightedRouter.RouterImage.Opacity == 1)
                {
                    foreach (var router in highlightedRouters)
                    {
                        router.RouterImage.Opacity = 1;
                    }
                    highlightedRouters.Clear();
                    highlightedRouters.Add(highlightedRouter);
                    highlightedRouter.RouterImage.Opacity = 0.5f;
                }
            }

            //Creating a router if there is no router on location where mouse was clicked
            else
            {
                //Creating router image and setting margins
                Point imageMargin = new Point((int)(position.X - RouterImageSize.Width / 2),
                                                   (int)(position.Y - RouterImageSize.Height / 2));
                var newRouter = new WpfRouter()
                {
                    Location = imageMargin,
                    RouterImage = new Canvas()
                };
                var image = new Image() { 
                    Source = new BitmapImage(new Uri("Router.png", UriKind.Relative)),
                    Width = RouterImageSize.Width,
                    Height = RouterImageSize.Height
                };
                image.Margin = new Thickness(
                        CircleImageSize.Width / 2, 
                        CircleImageSize.Height / 2, 
                        CircleImageSize.Width / 2, 
                        CircleImageSize.Height / 2);
                newRouter.RouterImage.Children.Add(image);
                newRouter.RouterImage.Width = RouterImageSize.Width + CircleImageSize.Width;
                newRouter.RouterImage.Height = RouterImageSize.Height + CircleImageSize.Height;
                newRouter.RouterImage.Margin = new Thickness(imageMargin.X, imageMargin.Y, 0, 0);

                //Adding circles to router image (so other routers can be connected to it)
                for (int i = 0; i < 4; i++)
                {
                    double left = 0;
                    double top = 0;
                    switch (i)
                    {
                        case 0:
                            left = newRouter.RouterImage.Width / 2 - CircleImageSize.Width / 2;
                            break;
                        case 1:
                            left = newRouter.RouterImage.Width - CircleImageSize.Width;
                            top = newRouter.RouterImage.Height / 2 - CircleImageSize.Height / 2;
                            break;
                        case 2:
                            left = newRouter.RouterImage.Width / 2 - CircleImageSize.Width / 2;
                            top = newRouter.RouterImage.Height - CircleImageSize.Height;
                            break;
                        case 3:
                            top = newRouter.RouterImage.Height / 2 - CircleImageSize.Height / 2;
                            break;
                    }
                    var circle = GetDefaultCircleImage();
                    circle.Margin = new Thickness(left, top, 0, 0);
                    newRouter.RouterImage.Children.Add(circle);
                }

                //Unhighlighting other routers
                foreach (var router in highlightedRouters)
                {
                    router.RouterImage.Opacity = 1;
                }
                highlightedRouters.Clear();

                //Higlighting the new router
                newRouter.RouterImage.Opacity = 0.5f;
                highlightedRouters.Add(newRouter);

                //Cleanup
                newRouter.RouterImage.MouseLeftButtonUp += RouterImage_MouseLeftButtonUp;
                routers.Add(newRouter);
                MainCanvas.Children.Add(newRouter.RouterImage);
            }
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var newMousePosition = e.GetPosition(MainCanvas);

            //Moving highlighted routers if left mouse button 
            //is pressed while mouse is being moved
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //Checking if images have to be moved
                bool moveImages = false;
                foreach (var router in (from _router in routers
                                        where _router.RouterImage.Opacity != 1
                                        select _router))
                {
                    if (IsPointOnImage(newMousePosition, router.RouterImage))
                    {
                        moveImages = true;
                        break;
                    }
                }

                //Images won't be moved if button was released 
                //and then mouse was clicked outside of all routers
                if (moveImages == true)
                {
                    for (int i = 0; i < highlightedRouters.Count; i++)
                    {
                        var router = highlightedRouters[i];
                        int leftMargin = (int)router.RouterImage.Margin.Left +
                                         (int)newMousePosition.X - previousMousePosition.X;
                        int topMargin = (int)router.RouterImage.Margin.Top +
                                        (int)newMousePosition.Y - previousMousePosition.Y;
                        router.RouterImage.Margin = new Thickness(leftMargin, topMargin, 0, 0);
                        router.UpdateConnectionLocations();
                    }
                }
            }

            previousMousePosition = new Point((int)newMousePosition.X, (int)newMousePosition.Y);
        }

        /// <summary>
        /// This event gets fired once router gets clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RouterImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Canvas canvas = sender as Canvas;
            var point = e.GetPosition(canvas);
            PointLocations? pointLocation = GetClickedPointIndex(
                canvas, 
                new Point((int)point.X, (int)point.Y));
            if(pointLocation == null)
            {
                return;
            }
            else
            {
                
            }
        }
        #endregion

        #region Other functions
        /// <summary>
        /// This function checks if a router image has been clicked
        /// </summary>
        /// <param name="clickLocation"> Click location (relative to image's parent (canvas)) </param>
        /// <param name="image"> Image which is being tested </param>
        /// <returns></returns>
        static bool IsPointOnImage(System.Windows.Point clickLocation, Canvas image) =>
            image.Margin.Left <= clickLocation.X &&
            image.Margin.Left + image.ActualWidth >= clickLocation.X &&
            image.Margin.Top <= clickLocation.Y &&
            image.Margin.Top + image.ActualHeight >= clickLocation.Y;

        /// <summary>
        /// This function returns the default circle which is used for connecting routers
        /// </summary>
        /// <returns></returns>
        static Ellipse GetDefaultCircleImage()
        {
            var circle = new Ellipse();
            circle.Stroke = Brushes.Black;
            circle.StrokeThickness = 1;
            circle.Height = CircleImageSize.Height;
            circle.Width = CircleImageSize.Width;
            return circle;
        }

        /// <summary>
        /// This function checks if the click location is on one of router's points or not
        /// </summary>
        /// <param name="routerCanvas"> Canvas which contains router image  </param>
        /// <param name="clickLocation"> Location on the click (relative to the canvas which was clicked) </param>
        /// <returns></returns>
        static PointLocations? GetClickedPointIndex(Canvas routerCanvas, Point clickLocation)
        {
            int i = 0;
            foreach(var child in routerCanvas.Children)
            {
                Ellipse point = child as Ellipse;
                if(point == null)
                {
                    continue;
                }
                if (point.Margin.Left >= clickLocation.X &&
                    point.Margin.Left + point.Width <= clickLocation.X &&
                    point.Margin.Top >= clickLocation.Y &&
                    point.Margin.Top + point.Height <= clickLocation.Y)
                {
                    switch (i)
                    {
                        case 0:
                            return PointLocations.Top;
                        case 1:
                            return PointLocations.Right;
                        case 2:
                            return PointLocations.Bottom;
                        case 3:
                            return PointLocations.Left;
                    }
                }
                i++;
            }
            return null;
        }
        #endregion
    }
}