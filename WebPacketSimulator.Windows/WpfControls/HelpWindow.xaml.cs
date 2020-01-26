using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace WebPacketSimulator.Wpf
{
    /// <summary>
    /// Interaction logic for Help.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public ObservableCollection<Shortcut> Shortcuts { get; set; } = new ObservableCollection<Shortcut>();

        #region Video variables
        public static readonly DependencyProperty IsRouterTutorialExpandedProperty =
            DependencyProperty.Register(nameof(IsRouterTutorialExpanded), typeof(bool), typeof(HelpWindow),
                                        new PropertyMetadata(false, OnIsRouterSimulationEnabledChanged));
        bool IsRouterTutorialExpanded
        {
            get => (bool)GetValue(IsRouterTutorialExpandedProperty);
            set
            {
                SetValue(IsRouterTutorialExpandedProperty, value);
                if (value)
                {
                    RouterCreationVideo.Play();
                }
                else
                {
                    RouterCreationVideo.Stop();
                }
            }
        }

        public static readonly DependencyProperty IsConnectionTutorialExpandedProperty =
            DependencyProperty.Register(nameof(IsConnectionTutorialExpanded), typeof(bool), typeof(HelpWindow),
                                        new PropertyMetadata(false, OnIsConnectionTutorialExpandedChanged));
        public bool IsConnectionTutorialExpanded
        {
            get => (bool)GetValue(IsConnectionTutorialExpandedProperty);
            set
            {
                SetValue(IsConnectionTutorialExpandedProperty, value);
                if (value)
                {
                    ConnectRoutersTutorialVideo.Play();
                }
                else
                {
                    ConnectRoutersTutorialVideo.Stop();
                }
            }
        }

        public static readonly DependencyProperty IsPacketTutorialExpandedProperty =
            DependencyProperty.Register(nameof(IsPacketTutorialExpanded), 
                                        typeof(bool), typeof(HelpWindow), 
                                        new PropertyMetadata(false, OnIsPacketSimulationExpandedPropertyChanged));
        public bool IsPacketTutorialExpanded
        {
            get => (bool)GetValue(IsPacketTutorialExpandedProperty);
            set
            {
                SetValue(IsPacketTutorialExpandedProperty, value);
                if (value)
                {
                    PacketTutorialVideo.Play();
                }
                else
                {
                    PacketTutorialVideo.Stop();
                }
            }
        }
        #endregion

        public HelpWindow()
        {
            foreach (var shortcut in Shortcut.Shortcuts)
            {
                Shortcuts.Add(shortcut);
            }
            InitializeComponent();
            DataContext = this;
        }

        #region Video variable changed callbacks
        static void OnIsPacketSimulationExpandedPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as HelpWindow).IsPacketTutorialExpanded = (bool)e.NewValue;
        }

        static void OnIsConnectionTutorialExpandedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as HelpWindow).IsConnectionTutorialExpanded = (bool)e.NewValue;
        }

        static void OnIsRouterSimulationEnabledChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as HelpWindow).IsRouterTutorialExpanded = (bool)e.NewValue;
        }
        #endregion

        private void Video_MediaEnded(object sender, RoutedEventArgs e)
        {
            var video = sender as MediaElement;
            video.Stop();
            video.Play();
        }
    }
}