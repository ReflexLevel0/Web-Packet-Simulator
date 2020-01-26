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
    /// Interaction logic for MenuUserControl.xaml
    /// </summary>
    public partial class Menu : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        bool deleteMenuItemIsEnabled;
        public bool DeleteMenuItemIsEnabled
        {
            get => deleteMenuItemIsEnabled;
            set
            {
                if (deleteMenuItemIsEnabled != value)
                {
                    deleteMenuItemIsEnabled = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(DeleteMenuItemIsEnabled)));
                }
            }
        }

        public Menu()
        {
            InitializeComponent();
            DataContext = this;
            DeleteCommand.Instance.CanExecuteChanged += (o,e) =>
            {
                DeleteMenuItemIsEnabled = (e as BoolEventArgs).Result;
            };
        }

        private void HelpMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new HelpWindow().Show();
        }
    }
}