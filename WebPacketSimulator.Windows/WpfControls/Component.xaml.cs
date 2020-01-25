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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WebPacketSimulator.Wpf
{
    /// <summary>
    /// Interaction logic for Component.xaml
    /// </summary>
    public partial class Component : UserControl, INotifyPropertyChanged
    {
        #region Local variables
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        string imageSource;
        public string ImageSource
        {
            get => imageSource;
            set
            {
                if (imageSource != value)
                {
                    imageSource = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(ImageSource)));
                }
            }
        }

        string text;
        public string Text
        {
            get => text;
            set
            {
                if (text != value)
                {
                    text = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(Text)));
                }
            }
        }

        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register(nameof(ImageWidth), typeof(double), typeof(Component));
        public double ImageWidth
        {
            get => (double)GetValue(ImageWidthProperty);
            set => SetValue(ImageWidthProperty, value);
        }

        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register(nameof(ImageHeight), typeof(double), typeof(Component));
        public double ImageHeight
        {
            get => (double)GetValue(ImageHeightProperty);
            set => SetValue(ImageHeightProperty, value);
        }
        #endregion

        #region Static variables
        public static string RouterComponentText { get; set; } = "Router";
        public static string LineComponentText { get; set; } = "Line";
        public static string SelectComponentText { get; set; } = "Select";
        public static string PacketComponentText { get; set; } = "Packet";
        #endregion

        public Component()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
