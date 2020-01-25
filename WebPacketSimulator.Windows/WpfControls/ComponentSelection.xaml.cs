﻿using System;
using System.Collections.Generic;
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
    /// Interaction logic for ComponentSelectionUserControl.xaml
    /// </summary>
    public partial class ComponentSelection : UserControl
    {
        public static ComponentSelection Instance;

        public ComponentSelection()
        {
            ToolTipService.ShowDurationProperty.OverrideMetadata
                (typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));
            Instance = this;
            InitializeComponent();
        }

        private void MenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WpfRouter.HighlightedRouters.UnhighlightAllRouters();
            WpfRouter.LastClickedRouter = null;
            var selectedValue = ((sender as ListView).SelectedValue as Component).Text.ToString();
            if (selectedValue.CompareTo(Component.RouterComponentText) == 0)
            {
                MainWindow.SelectedComponent = MainWindow.Components.Router;
            }
            else if (selectedValue.CompareTo(Component.LineComponentText) == 0)
            {
                MainWindow.SelectedComponent = MainWindow.Components.Line;
            }
            else if (selectedValue.CompareTo(Component.SelectComponentText) == 0)
            {
                MainWindow.SelectedComponent = MainWindow.Components.Select;
            }
            else
            {
                MainWindow.SelectedComponent = MainWindow.Components.Packet;
            }
        }
    }
}