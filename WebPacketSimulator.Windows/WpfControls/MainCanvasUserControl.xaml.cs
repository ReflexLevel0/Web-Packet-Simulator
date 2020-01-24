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

namespace WebPacketSimulator.Wpf
{
    /// <summary>
    /// Interaction logic for MainCanvasUserControl.xaml
    /// </summary>
    public partial class MainCanvasUserControl : UserControl
    {
        public static MainCanvasUserControl MainCanvasUC { get; set; }

        System.Windows.Point previousMousePosition = new System.Windows.Point();
        public static bool IsMessageAnimationRunning;
        public static Line CurrentLine = null;

        public MainCanvasUserControl()
        {
            MainCanvasUC = this;
            InitializeComponent();
        }

        private async void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var location = e.GetPosition(MainCanvas);

            //Highlighting the router on the click position
            WpfRouter clickedRouter = location.GetRouterOnLocation();

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
                    MainCanvas.Children.Remove(CurrentLine);
                    CurrentLine = null;
                    WpfRouter.ConnectRouters(WpfRouter.LastClickedRouter, clickedRouter);
                }
            }
            #endregion

            #region Packet
            else if (MainWindow.SelectedComponent == MainWindow.Components.Packet)
            {
                if (WpfRouter.LastClickedRouter == null)
                {
                    WpfRouter.LastClickedRouter = clickedRouter;
                }
                else
                {
                    await VisualHelpers.SendPacket(WpfRouter.LastClickedRouter, clickedRouter);
                    WpfRouter.LastClickedRouter = null;
                }
            }
            #endregion

            #region Router
            //Unhighlighting other routers and higlighting the new router
            else if (MainWindow.SelectedComponent == MainWindow.Components.Router && clickedRouter == null)
            {
                WpfRouter.HighlightedRouters.UnhighlightAllRouters();
                WpfRouter.CreateRouter(location).HighlightRouter();
            }
            #endregion
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var clickPosition = e.GetPosition(MainCanvas);
            var clickedRouter = clickPosition.GetRouterOnLocation();
            if (clickedRouter != null && MainWindow.SelectedComponent == MainWindow.Components.Router)
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
                //Highlighting the clicked router
                else
                {
                    if (clickedRouter.IsHighlighted == false)
                    {
                        clickedRouter.HighlightRouter();
                    }
                }
            }
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var newMousePosition = e.GetPosition(MainCanvas);

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
                    var offsetAmmount = new System.Windows.Point(
                                            newMousePosition.X - previousMousePosition.X,
                                            newMousePosition.Y - previousMousePosition.Y
                                        );
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
                    MainCanvas.Children.Add(CurrentLine);
                }
                CurrentLine.X1 = WpfRouter.LastClickedRouter.RouterCanvas.Margin.Left + WpfRouter.RouterImageWidth / 2;
                CurrentLine.Y1 = WpfRouter.LastClickedRouter.RouterCanvas.Margin.Top + WpfRouter.RouterImageHeight / 2;
                CurrentLine.X2 = newMousePosition.X;
                CurrentLine.Y2 = newMousePosition.Y;
            }

            previousMousePosition = new System.Windows.Point(newMousePosition.X, newMousePosition.Y);
        }
    }
}