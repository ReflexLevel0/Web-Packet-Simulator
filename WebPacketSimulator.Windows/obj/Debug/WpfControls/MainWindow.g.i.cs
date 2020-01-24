﻿#pragma checksum "..\..\..\WpfControls\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "2984EBAB551111E9AC83FF5C84EDFB40FEA68804311E646AE0920FB2AFBF72E4"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using WebPacketSimulator.Wpf;


namespace WebPacketSimulator.Wpf {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 38 "..\..\..\WpfControls\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal WebPacketSimulator.Wpf.AnimationSpeedUserControl AnimationSpeedUC;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\WpfControls\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas MainCanvas;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\WpfControls\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView ComponentSelectionListView;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\..\WpfControls\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel PacketConsoleStackPanel;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\..\WpfControls\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer PacketConsoleScrollViewer;
        
        #line default
        #line hidden
        
        
        #line 89 "..\..\..\WpfControls\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock PacketConsoleTextBlock;
        
        #line default
        #line hidden
        
        
        #line 96 "..\..\..\WpfControls\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel RouterDataStackPanel;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/WebPacketSimulator.Wpf;component/wpfcontrols/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\WpfControls\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 9 "..\..\..\WpfControls\MainWindow.xaml"
            ((WebPacketSimulator.Wpf.MainWindow)(target)).KeyUp += new System.Windows.Input.KeyEventHandler(this.Window_KeyUp);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 20 "..\..\..\WpfControls\MainWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.OpenFileCommandBinding_Executed);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 22 "..\..\..\WpfControls\MainWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.SaveFileCommandBinding_Executed);
            
            #line default
            #line hidden
            return;
            case 4:
            this.AnimationSpeedUC = ((WebPacketSimulator.Wpf.AnimationSpeedUserControl)(target));
            return;
            case 5:
            this.MainCanvas = ((System.Windows.Controls.Canvas)(target));
            
            #line 42 "..\..\..\WpfControls\MainWindow.xaml"
            this.MainCanvas.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.MainCanvas_MouseLeftButtonUp);
            
            #line default
            #line hidden
            
            #line 43 "..\..\..\WpfControls\MainWindow.xaml"
            this.MainCanvas.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.MainCanvas_MouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 44 "..\..\..\WpfControls\MainWindow.xaml"
            this.MainCanvas.MouseMove += new System.Windows.Input.MouseEventHandler(this.MainCanvas_MouseMove);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 56 "..\..\..\WpfControls\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ChangeMenuToComponents_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 59 "..\..\..\WpfControls\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ChangeMenuToPacketConsole_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.ComponentSelectionListView = ((System.Windows.Controls.ListView)(target));
            
            #line 67 "..\..\..\WpfControls\MainWindow.xaml"
            this.ComponentSelectionListView.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.MenuListView_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 9:
            this.PacketConsoleStackPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 10:
            this.PacketConsoleScrollViewer = ((System.Windows.Controls.ScrollViewer)(target));
            return;
            case 11:
            this.PacketConsoleTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 12:
            this.RouterDataStackPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 13:
            
            #line 102 "..\..\..\WpfControls\MainWindow.xaml"
            ((System.Windows.Controls.TextBox)(target)).TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.NameTextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            case 14:
            
            #line 104 "..\..\..\WpfControls\MainWindow.xaml"
            ((System.Windows.Controls.TextBox)(target)).TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.AddressTextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

