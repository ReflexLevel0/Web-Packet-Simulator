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
    public partial class AnimationSpeedUserControl : UserControl, INotifyPropertyChanged
    {
        #region Variables
        public static AnimationSpeedUserControl AnimationSpeedUC;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public static readonly DependencyProperty AnimationSpeedProperty =
            DependencyProperty.Register(nameof(AnimationSpeed), typeof(double), typeof(AnimationSpeedUserControl), new UIPropertyMetadata((double)1));
        public double AnimationSpeed
        {
            get => (double)GetValue(AnimationSpeedProperty);
            set
            {
                SetValue(AnimationSpeedProperty, value);
            }
        }
        #endregion

        public AnimationSpeedUserControl()
        {
            AnimationSpeedUC = this;
            InitializeComponent();
            DataContext = this;
        }

        private void SlowDownAnimationButton_Click(object sender, RoutedEventArgs e)
        {
            double newSpeed = AnimationSpeed / 2;
            if (newSpeed < 0.25)
            {
                return;
            }
            AnimationSpeed = newSpeed;
        }

        private void SpeedUpAnimationButton_Click(object sender, RoutedEventArgs e)
        {
            double newSpeed = AnimationSpeed * 2;
            if (newSpeed > 4)
            {
                return;
            }
            AnimationSpeed = newSpeed;
        }
    }
}