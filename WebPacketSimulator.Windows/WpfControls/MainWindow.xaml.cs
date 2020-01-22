using Microsoft.Win32;
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
        public static bool IsMessageAnimationRunning;
        bool isRouterDataOpacityAnimationRunning;
        Line currentLine = null;

        Visibility routerDataVisibility = Visibility.Collapsed;
        public Visibility RouterDataVisibility
        {
            get => routerDataVisibility;
            set
            {
                if (routerDataVisibility != value)
                {
                    routerDataVisibility = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(RouterDataVisibility)));
                }
            }
        }

        Router highlightedRouter;
        public Router HighlightedRouter
        {
            get => highlightedRouter;
            set
            {
                if (highlightedRouter != value)
                {
                    highlightedRouter = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(HighlightedRouter)));
                }
            }
        }

        bool isComponentMenuEnabled;
        public bool IsComponentMenuEnabled
        {
            get => isComponentMenuEnabled;
            set
            {
                if (isComponentMenuEnabled != value)
                {
                    isComponentMenuEnabled = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsComponentMenuEnabled)));
                }
            }
        }

        bool isPacketConsoleEnabled;
        public bool IsPacketConsoleEnabled
        {
            get => isPacketConsoleEnabled;
            set
            {
                if (isPacketConsoleEnabled != value)
                {
                    isPacketConsoleEnabled = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsPacketConsoleEnabled)));
                }
            }
        }

        public enum Menus { Components, PacketConsole }
        #endregion

        #region Static variables
        public static Canvas Canvas;
        public static Image PacketImage = new Image()
        {
            Source = new BitmapImage(new Uri("/Images/Packet.png", UriKind.Relative)),
            Width = 24,
            Height = 24
        };
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Canvas = MainCanvas;
            ChangeMenu(Menus.Components);
        }

        public static MainWindow GetCurrentMainWindow() => Application.Current.MainWindow as MainWindow;

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
            else if (SelectedComponent == Components.Line && WpfRouter.LastClickedRouter != null)
            {
                if (currentLine == null)
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

            previousMousePosition = new System.Windows.Point(newMousePosition.X, newMousePosition.Y);
        }

        private async void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var location = e.GetPosition(MainCanvas);

            //Highlighting the router on the click position
            WpfRouter clickedRouter = location.GetRouterOnLocation();

            #region Line
            if (SelectedComponent == Components.Line && IsMessageAnimationRunning == false && clickedRouter != null)
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
            else if (SelectedComponent == Components.Packet && IsMessageAnimationRunning == false)
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
            //Unhighlighting other routers and higlighting the new router
            else if (SelectedComponent == Components.Router && clickedRouter == null)
            {
                WpfRouter.HighlightedRouters.UnhighlightAllRouters();
                WpfRouter.CreateRouter(location).HighlightRouter();
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
                                               "Delete query", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (response == MessageBoxResult.Yes)
                {
                    while (WpfRouter.HighlightedRouters.Count > 0)
                    {
                        WpfRouter.HighlightedRouters[0].Delete();
                    }
                    var highlightedConnections = (from connection in Connections
                                                  where connection.ConnectionLine.Opacity != 1
                                                  select connection).ToList();
                    while (highlightedConnections.Count > 0)
                    {
                        highlightedConnections[0].Delete();
                        highlightedConnections.RemoveAt(0);
                    }
                }
            }
            else if (e.Key == Key.Escape)
            {
                WpfRouter.HighlightedRouters.UnhighlightAllRouters();
                Connection.HighlightedLines.UnhighlightAllLines();
                if (currentLine != null)
                {
                    MainCanvas.Children.Remove(currentLine);
                    WpfRouter.LastClickedRouter = null;
                    currentLine = null;
                }
            }
        }

        private void ChangeMenuToComponents_Click(object sender, RoutedEventArgs e)
        {
            ChangeMenu(Menus.Components);
        }

        private void ChangeMenuToPacketConsole_Click(object sender, RoutedEventArgs e)
        {
            ChangeMenu(Menus.PacketConsole);
        }

        /// <summary>
        /// This function shows data about the chosen router
        /// </summary>
        public void ShowRouterData()
        {
            AnimateRouterDataOpacity(true);
            HighlightedRouter = WpfRouter.HighlightedRouters[0].Router;
        }

        /// <summary>
        /// This function hides data about a router
        /// </summary>
        public void HideRouterData()
        {
            AnimateRouterDataOpacity(false);
            var focusedElement = Keyboard.FocusedElement as TextBox;
            if (focusedElement != null)
            {
                focusedElement.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }

        /// <summary>
        /// This function animates the opacity property of the router data stack panel
        /// </summary>
        /// <param name="show"> If true, opacity will become 1, otherwise router data will become invisible </param>
        public void AnimateRouterDataOpacity(bool show)
        {
            if (isRouterDataOpacityAnimationRunning == true)
            {
                return;
            }
            isRouterDataOpacityAnimationRunning = true;
            RouterDataStackPanel.Opacity = 0;
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = (show == true) ? 0 : 1;
            animation.To = (show == true) ? 1 : 0;
            animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250));
            animation.AccelerationRatio = 1;
            animation.Completed += delegate
            {
                isRouterDataOpacityAnimationRunning = false;
                if (show == false)
                {
                    RouterDataVisibility = Visibility.Collapsed;
                }
            };
            RouterDataStackPanel.BeginAnimation(StackPanel.OpacityProperty, animation);
            if (show == true)
            {
                RouterDataVisibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// This function changes the currently selected menu
        /// </summary>
        /// <param name="menu"></param>
        void ChangeMenu(Menus menu)
        {
            IsComponentMenuEnabled = true;
            IsPacketConsoleEnabled = true;
            ComponentSelectionListView.Visibility = Visibility.Collapsed;
            PacketConsoleStackPanel.Visibility = Visibility.Collapsed;
            switch (menu)
            {
                case Menus.Components:
                    IsComponentMenuEnabled = false;
                    ComponentSelectionListView.Visibility = Visibility.Visible;
                    break;
                case Menus.PacketConsole:
                    IsPacketConsoleEnabled = false;
                    PacketConsoleStackPanel.Visibility = Visibility.Visible;
                    break;
            }
        }

        /// <summary>
        /// This function updates the packet console
        /// </summary>
        /// <param name="destinationRouter"> Router to which the packet is going to </param>
        /// <param name="sourceRouter"> Router from which the packet is going to </param>
        /// <param name="firstAnimation"> If true, new linw will be appended before new text </param>
        public static void UpdatePacketConsole(Router sourceRouter, Router destinationRouter, bool firstAnimation)
        {
            var mainWindow = GetCurrentMainWindow();
            var textBlock = mainWindow.PacketConsoleTextBlock;
            var scroll = mainWindow.PacketConsoleScrollViewer;
            bool automaticScroll = Math.Abs(scroll.ActualHeight + scroll.VerticalOffset - scroll.ExtentHeight) < 1;

            //Making and appending the message
            StringBuilder textToAppend = new StringBuilder(128);
            if (string.IsNullOrEmpty(textBlock.Text) == false)
            {
                textToAppend.AppendLine();
                if (firstAnimation)
                {
                    textToAppend.AppendLine();
                }
            }
            textToAppend.Append("Packet sent");
            for (int i = 0; i < 2; i++)
            {
                textToAppend.Append((i == 0) ? " from " : " to ");
                var currentRouter = (i == 0) ? sourceRouter : destinationRouter;
                var emptyAddress = string.IsNullOrEmpty(currentRouter.Address);
                var emptyName = string.IsNullOrEmpty(currentRouter.Name);
                if (emptyAddress && emptyName)
                {
                    textToAppend.Append("[unknown]");
                }
                else if (emptyAddress && !emptyName)
                {
                    textToAppend.Append(currentRouter.Name);
                }
                else if(!emptyAddress && emptyName)
                {
                    textToAppend.Append(currentRouter.Address);
                }
                else
                {
                    textToAppend.Append(string.Format("{0} ({1})", currentRouter.Address, currentRouter.Name));
                }
            }
            textBlock.Text += textToAppend.ToString();
            if (automaticScroll)
            {
                mainWindow.PacketConsoleScrollViewer.ScrollToEnd();
            }

            //Removing lines from the console if there are too many lines
            int count = textBlock.Text.Count(c => c == '\n');
            while (count > 50)
            {
                textBlock.Text = textBlock.Text.Remove(0, textBlock.Text.IndexOf('\n') + 1);
                count--;
            }
        }

        private void SaveFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = FileHandler.FileDialogFilter;
            dialog.ShowDialog();
            FileHandler.SaveFile(WpfRouter.Routers, Connection.Connections, dialog.FileName);
        }

        private void OpenFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Saving current work
            if (WpfRouter.Routers.Count > 0)
            {
                switch (MessageBox.Show("Do you want to save the current work?", "Save current work", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
                {
                    case MessageBoxResult.Cancel:
                        return;
                    case MessageBoxResult.Yes:
                        SaveFileMenuItem_Click(null, null);
                        break;
                }
            }

            //Opening the new file
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = FileHandler.FileDialogFilter;
            dialog.ShowDialog();
            if (string.IsNullOrEmpty(dialog.FileName))
            {
                return;
            }

            //Deleting current data and loading data from the file
            while(WpfRouter.Routers.Count > 0)
            {
                WpfRouter.Routers[0].Delete();
            }
            while(Connection.Connections.Count > 0)
            {
                Connection.Connections[0].Delete();
            }
            FileHandler.LoadFile(dialog.FileName);
        }
    }
}