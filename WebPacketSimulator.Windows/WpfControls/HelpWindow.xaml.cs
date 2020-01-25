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
        public HelpWindow()
        {
            foreach (var shortcut in Shortcut.Shortcuts)
            {
                Shortcuts.Add(shortcut);
            }
            InitializeComponent();
            DataContext = this;
        }
    }
}