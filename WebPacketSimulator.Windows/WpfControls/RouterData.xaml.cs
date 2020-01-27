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
    public partial class RouterData : UserControl, INotifyPropertyChanged
    {
        #region Variables
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public static RouterData Instance;

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

        private string routerName;
        public string RouterName
        {
            get => routerName;
            set
            {
                if (routerName != value)
                {
                    routerName = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(RouterName)));
                }
            }
        }

        private string routerAddress;
        public string RouterAddress
        {
            get => routerAddress;
            set
            {
                if (routerAddress != value)
                {
                    routerAddress = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(RouterAddress)));
                }
            }
        }
        #endregion

        public RouterData()
        {
            Instance = this;
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// This function shows data about the chosen router
        /// </summary>
        public void ShowRouterData()
        {
            MainCanvas.HighlightedRouter = WpfRouter.HighlightedRouters[0].Router;
            AnimateRouterDataOpacity(true);
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
            if ((RouterData.Instance.RouterDataVisibility == Visibility.Visible &&
                show == true) ||
                (RouterData.Instance.RouterDataVisibility != Visibility.Visible &&
                show == false))
            {
                return;
            }
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = (show == true) ? 0 : 1;
            animation.To = (show == true) ? 1 : 0;
            animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250));
            animation.AccelerationRatio = 1;
            animation.Completed += delegate
            {
                Instance.RouterDataVisibility = (show == true) ? Visibility.Visible : Visibility.Collapsed;
            };
            if (show == true)
            {
                Instance.RouterDataVisibility = Visibility.Visible;
            }
            Instance.RouterDataStackPanel.BeginAnimation(OpacityProperty, animation, HandoffBehavior.SnapshotAndReplace);
        }

        /// <summary>
        /// This function updates the visibility of the router data (the data which is used to modify router data)
        /// </summary>
        public static void UpdateRouterDataVisibility()
        {
            if (WpfRouter.HighlightedRouters.Count == 1)
            {
                Instance.ShowRouterData();
            }
            else
            {
                Instance.HideRouterData();
            }
        }

        /// <summary>
        /// This function updates router data based on the given parameters
        /// </summary>
        /// <param name="routerAddress"> New router address </param>
        /// <param name="routerName"> New router name </param>
        public static void UpdateRouterData(string routerName, string routerAddress)
        {
            Instance.RouterName = routerName;
            Instance.RouterAddress = routerAddress;
            Instance.RouterNameTextBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            Instance.RouterAddressTextBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            Instance.RouterNameTextBox.Focus();
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var router = (from _router in WpfRouter.Routers
                          where _router.Router == MainCanvas.HighlightedRouter
                          select _router).First();
            MainCanvas.HighlightedRouter.Name = (sender as TextBox).Text;
        }

        private void AddressTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var router = (from _router in WpfRouter.Routers
                          where _router.Router == MainCanvas.HighlightedRouter
                          select _router).First();
            MainCanvas.HighlightedRouter.Address = (sender as TextBox).Text;
        }
    }
}