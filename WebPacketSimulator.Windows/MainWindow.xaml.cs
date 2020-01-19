using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        Point previousMousePosition = new Point();
        public static Canvas Canvas;
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        bool canSendMessage;
        /// <summary>
        /// Specifies if the user can send the message at this moment
        /// </summary>
        public bool CanSendMessage
        {
            get => canSendMessage;
            set
            {
                if (canSendMessage != value)
                {
                    canSendMessage = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(canSendMessage)));
                }
            }
        }

        public static Image MessageImage = new Image()
        {
            Source = new BitmapImage(new Uri("Packet.png", UriKind.Relative)),
            Width = 24,
            Height = 24
        };

        enum Components { Select, Router, Line }
        Components SelectedComponent = Components.Select;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Canvas = MainCanvas;
            SourceRouterListView.ItemsSource = WpfRouter.Routers;
            SourceRouterListView.DisplayMemberPath = "Router.Name";
            DestinationRouterListView.ItemsSource = WpfRouter.Routers;
            DestinationRouterListView.DisplayMemberPath = "Router.Name";
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

        private async void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            var source = WpfRouter.GetRouter((SourceRouterListView.SelectedItem as WpfRouter).Router.Name, true);
            var destination = WpfRouter.GetRouter((DestinationRouterListView.SelectedItem as WpfRouter).Router.Name, true);
            await VisualHelpers.SendPacket(source, destination, Canvas, this);
        }

        private void Source_Destination_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CanSendMessage =
                (SourceRouterListView.SelectedIndex >= 0 &&
                 DestinationRouterListView.SelectedIndex >= 0) ? true : false;
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
            MessageImage.BeginAnimation(Image.MarginProperty, animation);
            return taskCompleted.Task;
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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
                        if (clickedRouter.RouterImage.Opacity == 1)
                        {
                            clickedRouter.HighlightRouter();
                        }
                        else
                        {
                            clickedRouter.UnhighlightRouter();
                        }
                    }
                    //Unhighlighting all routers except the clicked one if ctrl isn't clicked
                    else if (clickedRouter.RouterImage.Opacity == 1)
                    {
                        WpfRouter.HighlightedRouters.UnhighlightAllRouters();
                        clickedRouter.HighlightRouter();
                    }
                }
                #endregion

                #region Line
                if(SelectedComponent == Components.Line)
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
            }

            //Creating a router if there is no router on location where mouse was clicked
            else
            {
                var newRouter = WpfRouter.CreateRouter(location);

                //Unhighlighting other routers and higlighting the new router
                foreach (var router in WpfRouter.HighlightedRouters)
                {
                    router.RouterImage.Opacity = 1;
                }
                WpfRouter.HighlightedRouters.Clear();
                newRouter.HighlightRouter();

                //Cleanup
                WpfRouter.Routers.Add(newRouter);
                MainCanvas.Children.Add(newRouter.RouterImage);
                Canvas.SetZIndex(newRouter.RouterImage, 1);
            }
        }
    }
}