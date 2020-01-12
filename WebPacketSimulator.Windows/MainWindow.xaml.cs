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
using Point = System.Drawing.Point;

namespace WebPacketSimulator.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables
        struct _Router
        {
            public Point Location;
            public Router Router;
            public Canvas RouterImage;
        }

        static Size routerImageSize = new Size(50, 50);
        static Size circleImageSize = new Size(10, 10);
        Point previousMousePosition = new Point();
        List<_Router> highlightedRouters
            = new List<_Router>();
        List<_Router> routers
            = new List<_Router>();
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Events
        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(MainCanvas);

            //Highlighting the router on the click position
            _Router highlightedRouter = new _Router();
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
                Point imageMargin = new Point((int)(position.X - routerImageSize.Width / 2),
                                                   (int)(position.Y - routerImageSize.Height / 2));
                var newRouter = new _Router()
                {
                    Location = imageMargin,
                    Router = new Router(),
                    RouterImage = new Canvas()
                };
                newRouter.RouterImage.Background = new ImageBrush(new BitmapImage(new Uri("Router.png", UriKind.Relative)));
                newRouter.RouterImage.Width = routerImageSize.Width;
                newRouter.RouterImage.Height = routerImageSize.Height;
                newRouter.RouterImage.Margin = new Thickness(imageMargin.X, imageMargin.Y, 0, 0);

                //Adding circles to router image (so other routers can be connected to it)
                for (int i = 0; i < 4; i++)
                {
                    double left = 0;
                    double top = 0;
                    switch (i)
                    {
                        case 0:
                            left = newRouter.RouterImage.Width / 2 - circleImageSize.Width / 2;
                            break;
                        case 1:
                            left = newRouter.RouterImage.Width - circleImageSize.Width;
                            top = newRouter.RouterImage.Height / 2 - circleImageSize.Height / 2;
                            break;
                        case 2:
                            left = newRouter.RouterImage.Width / 2 - circleImageSize.Width / 2;
                            top = newRouter.RouterImage.Height - circleImageSize.Height;
                            break;
                        case 3:
                            top = newRouter.RouterImage.Height / 2 - circleImageSize.Height / 2;
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
                    }
                    //TextBlock.Text = string.Format("{0};{1}", newMousePosition.X - previousMousePosition.X, newMousePosition.Y - previousMousePosition.Y);
                }
            }

            previousMousePosition = new Point((int)newMousePosition.X, (int)newMousePosition.Y);
        }
        #endregion

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
            circle.Height = circleImageSize.Height;
            circle.Width = circleImageSize.Width;
            return circle;
        }
    }
}