using System;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WebPacketSimulator.Wpf
{
    /// <summary>
    /// Interaction logic for MainCanvasUserControl.xaml
    /// </summary>
    public partial class MainCanvas : UserControl
    {
        public static MainCanvas Instance { get; set; }

        Point previousMousePosition = new Point();
        public static bool IsMessageAnimationRunning;
        public static Line CurrentLine = null;
        private Point lastMouseDownLocation;

        public MainCanvas()
        {
            Instance = this;
            InitializeComponent();
        }

        private async void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var location = e.GetPosition(Canvas);
            WpfRouter clickedRouter = location.GetRouterOnLocation();
            bool isCtrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

            #region Line
            if (MainWindow.SelectedComponent == MainWindow.Components.Line && IsMessageAnimationRunning == false && clickedRouter != null)
            {
                //If this is the first part of the connection
                if (WpfRouter.LastClickedRouter == null)
                {
                    WpfRouter.LastClickedRouter = clickedRouter;
                }
                //If this is second (last) part of the connection
                else
                {
                    Canvas.Children.Remove(CurrentLine);
                    CurrentLine = null;
                    WpfRouter.ConnectRouters(WpfRouter.LastClickedRouter, clickedRouter);
                }
            }
            #endregion

            #region Packet
            else if (MainWindow.SelectedComponent == MainWindow.Components.Packet && clickedRouter != null)
            {
                if (WpfRouter.LastClickedRouter == null)
                {
                    WpfRouter.LastClickedRouter = clickedRouter;
                }
                else if (WpfRouter.LastClickedRouter != clickedRouter)
                {
                    await VisualHelpers.SendPacket(WpfRouter.LastClickedRouter, clickedRouter);
                    WpfRouter.LastClickedRouter = null;
                }
            }
            #endregion

            #region Router
            //Unhighlighting all routers except the clicked one (if routers weren't moved)
            else if (MainWindow.SelectedComponent == MainWindow.Components.Router && clickedRouter != null)
            {
                if (Math.Abs(location.X - lastMouseDownLocation.X) < 1 &&
                    Math.Abs(location.Y - lastMouseDownLocation.Y) < 1 &&
                    isCtrlPressed == false)
                {
                    WpfRouter.HighlightedRouters.UnhighlightAllRouters(false);
                    clickedRouter.HighlightRouter();
                }
            }
            //Unhighlighting other routers and higlighting the new router
            else if (MainWindow.SelectedComponent == MainWindow.Components.Router && clickedRouter == null)
            {
                WpfRouter.HighlightedRouters.UnhighlightAllRouters(false);
                WpfRouter.CreateRouter(new Point(location.X - WpfRouter.RouterImageWidth / 2, 
                                       location.Y - WpfRouter.RouterImageHeight / 2)).HighlightRouter();
            }
            #endregion
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var clickPosition = e.GetPosition(Canvas);
            var clickedRouter = clickPosition.GetRouterOnLocation();
            if (clickedRouter != null && MainWindow.SelectedComponent == MainWindow.Components.Router)
            {
                bool isCtrlPressed =
                    Keyboard.IsKeyDown(Key.LeftCtrl) == true ||
                    Keyboard.IsKeyDown(Key.RightCtrl) == true;

                //Highlighting/unhighlighting the clicked router if ctrl is pressed
                if (isCtrlPressed == true)
                {
                    if (clickedRouter.IsHighlighted == false)
                    {
                        clickedRouter.HighlightRouter();
                    }
                    else
                    {
                        clickedRouter.UnhighlightRouter(true);
                    }
                }
                //Highlighting the clicked router
                else
                {
                    if (clickedRouter.IsHighlighted == false)
                    {
                        WpfRouter.HighlightedRouters.UnhighlightAllRouters(false);
                        clickedRouter.HighlightRouter();
                    }
                }
            }
            lastMouseDownLocation = clickPosition;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var newMousePosition = e.GetPosition(Canvas);

            //Moving highlighted routers if left mouse button 
            //is pressed while mouse is being moved
            if (e.LeftButton == MouseButtonState.Pressed && IsMessageAnimationRunning == false)
            {
                var highlightedRouters = (from router in WpfRouter.Routers
                                          where WpfRouter.HighlightedRouters.Contains(router)
                                          select router).ToList();
                //Images won't be moved if button was released 
                //and then mouse was clicked outside of all routers
                if (newMousePosition.IsOnAnyImage(highlightedRouters) ||
                    previousMousePosition.IsOnAnyImage(highlightedRouters))
                {
                    var offsetAmmount = new Point(newMousePosition.X - previousMousePosition.X,
                                                  newMousePosition.Y - previousMousePosition.Y);
                    WpfRouter.MoveRouters(WpfRouter.HighlightedRouters, offsetAmmount);
                }
            }
            else if (MainWindow.SelectedComponent == MainWindow.Components.Line && WpfRouter.LastClickedRouter != null)
            {
                if (CurrentLine == null)
                {
                    CurrentLine = new Line()
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = Connection.ConnectionLineWidth
                    };
                    Canvas.Children.Add(CurrentLine);
                }
                CurrentLine.X1 = WpfRouter.LastClickedRouter.RouterCanvas.Margin.Left + WpfRouter.RouterImageWidth / 2;
                CurrentLine.Y1 = WpfRouter.LastClickedRouter.RouterCanvas.Margin.Top + WpfRouter.RouterImageHeight / 2;
                CurrentLine.X2 = newMousePosition.X;
                CurrentLine.Y2 = newMousePosition.Y;
            }

            previousMousePosition = new Point(newMousePosition.X, newMousePosition.Y);
        }
    }
}