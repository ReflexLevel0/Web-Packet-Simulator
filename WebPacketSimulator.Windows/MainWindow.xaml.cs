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
            public Image RouterImage;
        }

        Size routerImageSize = new Size(50, 50);
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

            //If no router is where mouse has been clicked
            else
            {
                var routerImage = new Image()
                {
                    Source = new BitmapImage(new Uri("Router.png", UriKind.Relative)),
                    MaxHeight = routerImageSize.Height,
                    MaxWidth = routerImageSize.Width
                };
                Point routerLocation = new Point((int)(position.X - routerImageSize.Width / 2),
                                                   (int)(position.Y - routerImageSize.Height / 2));
                routerImage.Margin = new Thickness(routerLocation.X, routerLocation.Y, 0, 0);
                MainCanvas.Children.Add(routerImage);
                routers.Add(new _Router()
                {
                    Location = routerLocation,
                    Router = new Router(),
                    RouterImage = routerImage
                });
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

        static bool IsPointOnImage(System.Windows.Point clickLocation, Image image) =>
            image.Margin.Left <= clickLocation.X &&
            image.Margin.Left + image.ActualWidth >= clickLocation.X &&
            image.Margin.Top <= clickLocation.Y &&
            image.Margin.Top + image.ActualHeight >= clickLocation.Y;
    }
}