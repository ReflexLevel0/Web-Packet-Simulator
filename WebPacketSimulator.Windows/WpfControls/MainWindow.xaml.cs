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
        public static Image PacketImage = new Image()
        {
            Source = new BitmapImage(new Uri("/Images/Packet.png", UriKind.Relative)),
            Width = 24,
            Height = 24
        };
        public static MainWindow CurrentMainWindow;
        Router highlightedRouter;
        public Router HighlightedRouter 
        { 
            get => highlightedRouter; 
            set 
            { 
                highlightedRouter = value; 
                RouterData.UpdateRouterData(HighlightedRouter.Name, HighlightedRouter.Address); 
            } 
        }
        public static string CurrentFilePath;
        #endregion

        public MainWindow()
        {
            CurrentMainWindow = this;
            InitializeComponent();
            DataContext = this;
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
                        SaveCommand.Instance.Equals(null);
                        break;
                }
            }
        }
    }
}