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
        #region Variables
        System.Windows.Point previousMousePosition = new System.Windows.Point();
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        enum Components { Select, Router, Line, Packet }
        Components SelectedComponent = Components.Select;
        public static bool IsMessageAnimationRunning;
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

        string currentFilePath = null;

        public static readonly DependencyProperty AnimationSpeedProperty =
            DependencyProperty.Register(nameof(AnimationSpeed), typeof(double), typeof(MainWindow));
        public double AnimationSpeed
        {
            get => (double)GetValue(AnimationSpeedProperty);
            set
            {
                SetValue(AnimationSpeedProperty, value);
            }
        }
        
        public static Image PacketImage = new Image()
        {
            Source = new BitmapImage(new Uri("/Images/Packet.png", UriKind.Relative)),
            Width = 24,
            Height = 24
        };

        public static MainWindow CurrentMainWindow;
        #endregion

        public MainWindow()
        {
            CurrentMainWindow = this;
            InitializeComponent();
            DataContext = this;
            ChangeMenu(Menus.Components);
            var animationSpeedBinding = new Binding("AnimationSpeed") { 
                Source = AnimationSpeedUC, 
                Mode = BindingMode.OneWay
            };
            SetBinding(AnimationSpeedProperty, animationSpeedBinding);
        }

        #region Mouse
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
                currentLine.X1 = WpfRouter.LastClickedRouter.RouterCanvas.Margin.Left + WpfRouter.RouterImageWidth / 2;
                currentLine.Y1 = WpfRouter.LastClickedRouter.RouterCanvas.Margin.Top + WpfRouter.RouterImageHeight / 2;
                currentLine.X2 = newMousePosition.X;
                currentLine.Y2 = newMousePosition.Y;
            }

            previousMousePosition = new System.Windows.Point(newMousePosition.X, newMousePosition.Y);
        }

        private void ChangeMenuToComponents_Click(object sender, RoutedEventArgs e)
        {
            ChangeMenu(Menus.Components);
        }

        private void ChangeMenuToPacketConsole_Click(object sender, RoutedEventArgs e)
        {
            ChangeMenu(Menus.PacketConsole);
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
                    MainCanvas.Children.Remove(currentLine);
                    currentLine = null;
                    WpfRouter.ConnectRouters(WpfRouter.LastClickedRouter, clickedRouter);
                }
            }
            #endregion

            #region Packet
            else if (SelectedComponent == Components.Packet)
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
            var clickPosition = e.GetPosition(MainCanvas);
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
        #endregion

        #region Router data stack panel
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
        #endregion

        #region Packet console
        /// <summary>
        /// This function updates the packet console
        /// </summary>
        /// <param name="destinationRouter"> Router to which the packet is going to </param>
        /// <param name="sourceRouter"> Router from which the packet is going to </param>
        /// <param name="firstAnimation"> If true, new linw will be appended before new text </param>
        public static void UpdatePacketConsole(Router sourceRouter, Router destinationRouter, bool firstAnimation)
        {
            var mainWindow = CurrentMainWindow;
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
                else if (!emptyAddress && emptyName)
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

        /// <summary>
        /// This function send a message animation termination message to the console
        /// </summary>
        public static void UpdatePacketConsole()
        {
            CurrentMainWindow.PacketConsoleTextBlock.Text += "\nMessage animation canceled!\n";
        }

        /// <summary>
        /// This function updates the console to show data summary after packet animation has ended
        /// </summary>
        /// <param name="path"> Packet's path </param>
        public static void UpdatePacketConsole(List<Router> path)
        {
            StringBuilder text = new StringBuilder(128);
            text.AppendLine();
            text.Append(string.Format("Path length: {0}", path.Count));
            CurrentMainWindow.PacketConsoleTextBlock.Text += text.ToString();
        }
        #endregion

        #region Command binding
        private void OpenFileCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //Saving current work
            if (WpfRouter.Routers.Count > 0)
            {
                switch (VisualHelpers.SaveCurrentWorkQuery())
                {
                    case MessageBoxResult.Cancel:
                        return;
                    case MessageBoxResult.Yes:
                        SaveFileCommandBinding_Executed(null, null);
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
            while (WpfRouter.Routers.Count > 0)
            {
                WpfRouter.Routers[0].Delete();
            }
            while (Connection.Connections.Count > 0)
            {
                Connection.Connections[0].Delete();
            }
            FileHandler.LoadFile(dialog.FileName);
            currentFilePath = dialog.FileName;
        }

        private void SaveFileCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (currentFilePath == null)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = FileHandler.FileDialogFilter;
                dialog.ShowDialog();
                currentFilePath = dialog.FileName;
            }
            FileHandler.SaveFile(WpfRouter.Routers, Connection.Connections, currentFilePath);
        }
        #endregion

        #region TextBox
        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var router = (from _router in WpfRouter.Routers
                          where _router.Router == HighlightedRouter
                          select _router).First();
            router.RouterNameTextBlock.Text = (sender as TextBox).Text;
        }

        private void AddressTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var router = (from _router in WpfRouter.Routers
                          where _router.Router == HighlightedRouter
                          select _router).First();
            router.RouterAddressTextBlock.Text = (sender as TextBox).Text;
        }
        #endregion

        private void MenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        /// <summary>
        /// This function animates the opacity property of the router data stack panel
        /// </summary>
        /// <param name="show"> If true, opacity will become 1, otherwise router data will become invisible </param>
        public void AnimateRouterDataOpacity(bool show)
        {
            RouterDataStackPanel.Opacity = 0;
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = (show == true) ? 0 : 1;
            animation.To = (show == true) ? 1 : 0;
            animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250));
            animation.AccelerationRatio = 1;
            animation.Completed += delegate
            {
                RouterDataVisibility = (show == true) ? Visibility.Visible : Visibility.Collapsed;
            };
            if (show == true)
            {
                RouterDataVisibility = Visibility.Visible;
            }
            RouterDataStackPanel.BeginAnimation(StackPanel.OpacityProperty, animation, HandoffBehavior.SnapshotAndReplace);
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

        protected override void OnClosing(CancelEventArgs e)
        {
            if (WpfRouter.Routers.Count > 0)
            {
                switch (VisualHelpers.SaveCurrentWorkQuery())
                {
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        return;
                    case MessageBoxResult.Yes:
                        SaveFileCommandBinding_Executed(null,null);
                        break;
                }
            }
        }
    }
}