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
    public partial class MainWindow : Window
    {
        #region Variables
        public enum Components { Select, Router, Line, Packet }
        public static Components SelectedComponent = Components.Select;
        string currentFilePath = null;
        public static Image PacketImage = new Image()
        {
            Source = new BitmapImage(new Uri("/Images/Packet.png", UriKind.Relative)),
            Width = 24,
            Height = 24
        };
        public static MainWindow CurrentMainWindow;

        #region Highlighted router
        public EventHandler HighlightedRouterNameChanged;
        public EventHandler HighlightedRouterAddressChanged;

        public static readonly DependencyProperty HighlightedRouterProperty =
            DependencyProperty.Register(nameof(HighlightedRouter), typeof(Router), typeof(MainWindow));
        public Router HighlightedRouter
        {
            get => (Router)GetValue(HighlightedRouterProperty);
            set
            {
                SetValue(HighlightedRouterProperty, value);
                HighlightedRouter.AddressChanged = HighlightedRouterAddressChanged;
                HighlightedRouter.NameChanged = HighlightedRouterNameChanged;
            }
        }
        #endregion
        #endregion

        public MainWindow()
        {
            CurrentMainWindow = this;
            InitializeComponent();
            DataContext = this;
        }

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
                if (MainCanvasUserControl.CurrentLine != null)
                {
                    MainCanvasUserControl.MainCanvasUC.MainCanvas.Children.Remove(MainCanvasUserControl.CurrentLine);
                    WpfRouter.LastClickedRouter = null;
                    MainCanvasUserControl.CurrentLine = null;
                }
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