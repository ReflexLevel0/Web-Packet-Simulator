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
        System.Windows.Point previousMousePosition = new System.Windows.Point();
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        enum Components { Select, Router, Line, Packet }
        Components SelectedComponent = Components.Select;
        bool isAnimationRunning;
        Line currentLine = null;
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
                isAnimationRunning = false;
            };
            animation.Completed += OnCompleted;
            isAnimationRunning = true;
            PacketImage.BeginAnimation(Image.MarginProperty, animation);
            return taskCompleted.Task;
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var newMousePosition = e.GetPosition(MainCanvas);

            //Moving highlighted routers if left mouse button 
            //is pressed while mouse is being moved
            if (e.LeftButton == MouseButtonState.Pressed && isAnimationRunning == false)
            {
                var highlightedRouters = (from router in WpfRouter.Routers
                                          where WpfRouter.HighlightedRouters.Contains(router)
                                          select router).ToList();
                //Images won't be moved if button was released 
                //and then mouse was clicked outside of all routers
                if (newMousePosition.IsOnAnyImage(highlightedRouters) ||
                    previousMousePosition.IsOnAnyImage(highlightedRouters))
                {
                    var offsetAmmount = new System.Windows.Point(
                                            newMousePosition.X - previousMousePosition.X,
                                            newMousePosition.Y - previousMousePosition.Y
                                        );
                    WpfRouter.MoveRouters(WpfRouter.HighlightedRouters, offsetAmmount);
                }
            }
            else if(SelectedComponent == Components.Line && WpfRouter.LastClickedRouter != null)
            {
                if(currentLine == null)
                {
                    currentLine = new Line() 
                    { 
                        Stroke = Brushes.Black, 
                        StrokeThickness = ConnectionLineWidth 
                    };
                    MainCanvas.Children.Add(currentLine);
                }
                currentLine.X1 = WpfRouter.LastClickedRouter.RouterImage.Margin.Left + WpfRouter.RouterImageWidth / 2;
                currentLine.Y1 = WpfRouter.LastClickedRouter.RouterImage.Margin.Top + WpfRouter.RouterImageHeight / 2;
                currentLine.X2 = newMousePosition.X;
                currentLine.Y2 = newMousePosition.Y;
            }

            previousMousePosition = new System.Windows.Point((int)newMousePosition.X, (int)newMousePosition.Y);
        }

        private async void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var location = e.GetPosition(MainCanvas);

            //Highlighting the router on the click position
            WpfRouter clickedRouter = location.GetRouterOnLocation();

            #region Line
            if (SelectedComponent == Components.Line && isAnimationRunning == false && clickedRouter != null)
            {
                //If this is the first part of the connection
                if (WpfRouter.LastClickedRouter == null)
                {
                    WpfRouter.LastClickedRouter = clickedRouter;
                }
                //If this is second (last) part of the connection
                else
                {
                    Canvas.Children.Remove(currentLine);
                    currentLine = null;
                    WpfRouter.ConnectRouters(WpfRouter.LastClickedRouter, clickedRouter);
                }
            }
            #endregion

            #region Packet
            else if (SelectedComponent == Components.Packet && isAnimationRunning == false)
            {
                if (WpfRouter.LastClickedRouter == null)
                {
                    WpfRouter.LastClickedRouter = clickedRouter;
                }
                else
                {
                    await VisualHelpers.SendPacket(WpfRouter.LastClickedRouter, clickedRouter, this);
                    WpfRouter.LastClickedRouter = null;
                }
            }
            #endregion

            #region Router
            else if (SelectedComponent == Components.Router && clickedRouter == null)
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
            #endregion
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var clickPosition = e.GetPosition(Canvas);
            var clickedRouter = clickPosition.GetRouterOnLocation();
            if (clickedRouter != null && SelectedComponent == Components.Router)
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
                else
                {
                    WpfRouter.HighlightedRouters.UnhighlightAllRouters();
                    clickedRouter.HighlightRouter();
                }
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

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var response = MessageBox.Show("Are you sure that you want to delete the selected objects?",
                                                   "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (response == MessageBoxResult.Yes)
                {
                    while (WpfRouter.HighlightedRouters.Count > 0)
                    {
                        WpfRouter.HighlightedRouters[0].Delete();
                    }
                    while(Connections.Count > 0)
                    {
                        Connections[0].Delete();
                    }
                }
            }
            else if(e.Key == Key.Escape)
            {
                WpfRouter.HighlightedRouters.UnhighlightAllRouters();
                Connection.HighlightedLines.UnhighlightAllLines();
                if(currentLine != null)
                {
                    MainCanvas.Children.Remove(currentLine);
                    WpfRouter.LastClickedRouter = null;
                    currentLine = null;
                }
            }
        }
    }
}