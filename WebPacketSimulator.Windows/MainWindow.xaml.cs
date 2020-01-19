using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Local variables
        Point previousMousePosition = new Point();
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        enum Components { Select, Router, Line, Packet }
        Components SelectedComponent = Components.Select;
        #endregion

        #region Static variables
        public static Canvas Canvas;
        public static Image PacketImage = new Image()
        {
            Source = new BitmapImage(new Uri("Packet.png", UriKind.Relative)),
            Width = 24,
            Height = 24
        };
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Canvas = MainCanvas;
        }

        /// <summary>
        /// This function is used for animation (the task will be awaited until the animation stops)
        /// </summary>
        /// <param name="animation"> Animation to be executed </param>
        /// <returns></returns>
        public Task Animate(ThicknessAnimation animation)
        {
            TaskCompletionSource<bool> taskCompleted = new TaskCompletionSource<bool>();
            EventHandler OnCompleted = null;
            OnCompleted = delegate
            {
                taskCompleted.SetResult(true);
                animation.Completed -= OnCompleted;
            };
            animation.Completed += OnCompleted;
            PacketImage.BeginAnimation(Image.MarginProperty, animation);
            return taskCompleted.Task;
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var newMousePosition = e.GetPosition(MainCanvas);

            //Moving highlighted routers if left mouse button 
            //is pressed while mouse is being moved
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var highlightedRouters = (from router in WpfRouter.Routers
                                      where WpfRouter.HighlightedRouters.Contains(router)
                                      select router).ToList();
                //Images won't be moved if button was released 
                //and then mouse was clicked outside of all routers
                if (newMousePosition.IsOnAnyImage(highlightedRouters))
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

        private async void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var location = e.GetPosition(MainCanvas);

            //Highlighting the router on the click position
            WpfRouter clickedRouter = location.GetRouterOnLocation();
            bool clickedOnRouter = clickedRouter != null;
            if (clickedOnRouter)
            {
                #region Router
                if (SelectedComponent == Components.Router)
                {
                    bool isCtrlClicked =
                        Keyboard.IsKeyDown(Key.LeftCtrl) == true ||
                        Keyboard.IsKeyDown(Key.RightCtrl) == true;

                    //Highlighting/unhighlighting the clicked router if ctrl is pressed
                    if (isCtrlClicked == true)
                    {
                        if (clickedRouter.IsHighlighted == false)
                        {
                            clickedRouter.HighlightRouter();
                        }
                        else
                        {
                            clickedRouter.UnhighlightRouter();
                        }
                    }
                    //Unhighlighting all routers except the clicked one if ctrl isn't clicked
                    else if (clickedRouter.IsHighlighted == false)
                    {
                        WpfRouter.HighlightedRouters.UnhighlightAllRouters();
                        clickedRouter.HighlightRouter();
                    }
                }
                #endregion

                #region Line
                else if (SelectedComponent == Components.Line)
                {
                    //If this is the first part of the connection
                    if (WpfRouter.LastClickedRouter == null)
                    {
                        WpfRouter.LastClickedRouter = clickedRouter;
                    }
                    //If this is second (last) part of the connection
                    else
                    {
                        WpfRouter.ConnectRouters(WpfRouter.LastClickedRouter, clickedRouter);
                        WpfRouter.LastClickedRouter = null;
                    }
                }
                #endregion

                #region Message
                else if (SelectedComponent == Components.Packet)
                {
                    if (WpfRouter.LastClickedRouter == null)
                    {
                        WpfRouter.LastClickedRouter = clickedRouter;
                    }
                    else
                    {
                        await VisualHelpers.SendPacket(WpfRouter.LastClickedRouter, clickedRouter, this);
                    }
                }
                #endregion
            }

            //Creating a router if there is no router on location where mouse was clicked
            else
            {
                //Unhighlighting other routers and higlighting the new router
                var newRouter = WpfRouter.CreateRouter(location);
                WpfRouter.HighlightedRouters.UnhighlightAllRouters();
                newRouter.HighlightRouter();

                //Cleanup
                WpfRouter.Routers.Add(newRouter);
                MainCanvas.Children.Add(newRouter.RouterImage);
                Canvas.SetZIndex(newRouter.RouterImage, 1);
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WpfRouter.HighlightedRouters.UnhighlightAllRouters();
            WpfRouter.LastClickedRouter = null;
            var selectedValue = ((sender as ListView).SelectedValue as Component).Text.ToString();
            if (selectedValue.CompareTo(Component.RouterComponentText) == 0)
            {
                SelectedComponent = Components.Router;
            }
            else if (selectedValue.CompareTo(Component.LineComponentText) == 0)
            {
                SelectedComponent = Components.Line;
            }
            else if (selectedValue.CompareTo(Component.SelectComponentText) == 0) 
            {
                SelectedComponent = Components.Select;
            }
            else
            {
                SelectedComponent = Components.Packet;
            }
        }
    }
}