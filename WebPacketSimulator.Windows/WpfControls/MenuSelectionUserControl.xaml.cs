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
    /// Interaction logic for MenuSelectionUserControl.xaml
    /// </summary>
    public partial class MenuSelectionUserControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public enum Menus { Components, PacketConsole }

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

        public MenuSelectionUserControl()
        {
            InitializeComponent();
            ChangeMenu(Menus.Components);
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
        /// This function changes the currently selected menu
        /// </summary>
        /// <param name="menu"></param>
        void ChangeMenu(Menus menu)
        {
            IsComponentMenuEnabled = true;
            IsPacketConsoleEnabled = true;
            ComponentSelectionUserControl.ComponentSelectionUC.Visibility = Visibility.Collapsed;
            PacketConsoleUserControl.PacketConsoleUC.Visibility = Visibility.Collapsed;
            switch (menu)
            {
                case Menus.Components:
                    IsComponentMenuEnabled = false;
                    ComponentSelectionUserControl.ComponentSelectionUC.Visibility = Visibility.Visible;
                    break;
                case Menus.PacketConsole:
                    IsPacketConsoleEnabled = false;
                    PacketConsoleUserControl.PacketConsoleUC.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}
