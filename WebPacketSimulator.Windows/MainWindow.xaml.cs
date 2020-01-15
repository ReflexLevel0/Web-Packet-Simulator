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
        Point previousMousePosition = new Point();

        public MainWindow()
        {
            //Binding binding = new Binding("TopConnectionLocation.Y");
            //binding.Source = new WpfRouter() { TopConnectionLocation = new System.Windows.Point(100,200) };
            //binding.Mode = BindingMode.TwoWay;
            //Connection connection = new Connection();
            //connection.ConnectionLine.SetBinding(Line.X1Property, binding);
            //MessageBox.Show(connection.ConnectionLine.X1.ToString());
            InitializeComponent();
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(MainCanvas);

            //Highlighting the router on the click position
            WpfRouter highlightedRouter = null;
            bool clickedOnRouter = position.IsOnAnyImage(WpfRouter.Routers);
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
                        highlightedRouter.HighlightRouter();
                    }
                    else
                    {
                        highlightedRouter.UnhighlightRouter();
                    }
                }
                //Unhighlighting all routers except the clicked one if ctrl isn't clicked
                else if (highlightedRouter.RouterImage.Opacity == 1)
                {
                    WpfRouter.HighlightedRouters.UnhighlightAllRouters();
                    highlightedRouter.HighlightRouter();
                }
            }

            //Creating a router if there is no router on location where mouse was clicked
            else
            {
                var newRouter = WpfRouter.CreateRouterControl(position);

                //Unhighlighting other routers
                foreach (var router in WpfRouter.HighlightedRouters)
                {
                    router.RouterImage.Opacity = 1;
                }
                WpfRouter.HighlightedRouters.Clear();

                //Higlighting the new router
                newRouter.RouterImage.Opacity = 0.5f;
                WpfRouter.HighlightedRouters.Add(newRouter);

                //Cleanup
                WpfRouter.Routers.Add(newRouter);
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
                var notHighlighted = (from router in WpfRouter.Routers
                                     where WpfRouter.HighlightedRouters.Contains(router)
                                     select router).ToList();
                //Images won't be moved if button was released 
                //and then mouse was clicked outside of all routers
                if (newMousePosition.IsOnAnyImage(notHighlighted))
                {
                    var offsetAmmount = new System.Windows.Point(
                                            newMousePosition.X - previousMousePosition.X,
                                            newMousePosition.Y - previousMousePosition.Y
                                        );
                    WpfRouter.MoveRouters(WpfRouter.HighlightedRouters, offsetAmmount);
                }
            }

            previousMousePosition = new Point((int)newMousePosition.X, (int)newMousePosition.Y);
        }
    }
}