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
    /// Interaction logic for AnimationSpeedUserControl.xaml
    /// </summary>
    public partial class AnimationSpeed : UserControl, INotifyPropertyChanged
    {
        #region Variables
        public static AnimationSpeed Instance;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.Register(nameof(Speed), typeof(double), typeof(AnimationSpeed), new UIPropertyMetadata((double)1));
        public double Speed
        {
            get => (double)GetValue(SpeedProperty);
            set
            {
                SetValue(SpeedProperty, value);
            }
        }
        #endregion

        public AnimationSpeed()
        {
            Instance = this;
            InitializeComponent();
            DataContext = this;
        }

        private void SlowDownAnimationButton_Click(object sender, RoutedEventArgs e)
        {
            double newSpeed = Speed / 2;
            if (newSpeed < 0.25)
            {
                return;
            }
            Speed = newSpeed;
        }

        private void SpeedUpAnimationButton_Click(object sender, RoutedEventArgs e)
        {
            double newSpeed = Speed * 2;
            if (newSpeed > 4)
            {
                return;
            }
            Speed = newSpeed;
        }
    }
}