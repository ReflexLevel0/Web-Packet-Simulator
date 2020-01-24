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

namespace WebPacketSimulator.Wpf
{
    /// <summary>
    /// Interaction logic for RouterDataUserControl.xaml
    /// </summary>
    public partial class RouterDataUserControl : UserControl, INotifyPropertyChanged
    {
        #region Variables
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public static RouterDataUserControl RouterDataUC;

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

        public static readonly DependencyProperty RouterNameProperty =
            DependencyProperty.Register(nameof(RouterName), typeof(string), typeof(RouterDataUserControl));
        public string RouterName
        {
            get => (string)GetValue(RouterNameProperty);
            set => SetValue(RouterNameProperty, value);
        }

        public static readonly DependencyProperty RouterAddressProperty =
            DependencyProperty.Register(nameof(RouterAddress), typeof(string), typeof(RouterDataUserControl));
        public string RouterAddress
        {
            get => (string)GetValue(RouterAddressProperty);
            set => SetValue(RouterAddressProperty, value);
        }
        #endregion

        public RouterDataUserControl()
        {
            RouterDataUC = this;
            InitializeComponent();
            var nameBinding = new Binding("Name") 
            {
                Mode = BindingMode.OneWay,
                Source = MainWindow.CurrentMainWindow.HighlightedRouter
            };
            SetBinding(RouterNameProperty, nameBinding);
            MainWindow.CurrentMainWindow.HighlightedRouterNameChanged += delegate 
            {
                MainWindow.CurrentMainWindow.GetBindingExpression(MainWindow.HighlightedRouterProperty).UpdateSource();
            };
            MainWindow.CurrentMainWindow.HighlightedRouterAddressChanged += delegate
            {
                MainWindow.CurrentMainWindow.GetBindingExpression(MainWindow.HighlightedRouterProperty).UpdateSource();
            };
        }

        /// <summary>
        /// This function shows data about the chosen router
        /// </summary>
        public void ShowRouterData()
        {
            AnimateRouterDataOpacity(true);
            MainWindow.CurrentMainWindow.HighlightedRouter = WpfRouter.HighlightedRouters[0].Router;
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
        public static void AnimateRouterDataOpacity(bool show)
        {
            RouterDataUC.RouterDataStackPanel.Opacity = 0;
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = (show == true) ? 0 : 1;
            animation.To = (show == true) ? 1 : 0;
            animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250));
            animation.AccelerationRatio = 1;
            animation.Completed += delegate
            {
                RouterDataUC.RouterDataVisibility = (show == true) ? Visibility.Visible : Visibility.Collapsed;
            };
            if (show == true)
            {
                RouterDataUC.RouterDataVisibility = Visibility.Visible;
            }
            RouterDataUC.RouterDataStackPanel.BeginAnimation(OpacityProperty, animation, HandoffBehavior.SnapshotAndReplace);
        }

        /// <summary>
        /// This function updates the visibility of the router data (the data which is used to modify router data)
        /// </summary>
        public static void UpdateRouterDataVisibility()
        {
            var mainWindow = (Application.Current.MainWindow as MainWindow);
            if (WpfRouter.HighlightedRouters.Count == 1)
            {
                RouterDataUC.ShowRouterData();
            }
            else
            {
                RouterDataUC.HideRouterData();
            }
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var router = (from _router in WpfRouter.Routers
                          where _router.Router == MainWindow.CurrentMainWindow.HighlightedRouter
                          select _router).First();
            router.RouterNameTextBlock.Text = (sender as TextBox).Text;
        }

        private void AddressTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var router = (from _router in WpfRouter.Routers
                          where _router.Router == MainWindow.CurrentMainWindow.HighlightedRouter
                          select _router).First();
            router.RouterAddressTextBlock.Text = (sender as TextBox).Text;
        }
    }
}