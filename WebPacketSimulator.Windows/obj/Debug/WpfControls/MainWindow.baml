   M S B A M L   `   `   `  ����Q  MWebPacketSimulator.Wpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:WebPacketSimulator.WpfWebPacketSimulator.Wpf  5   	   '    !WebPacketSimulator.Wpf.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   6local$clr-namespace:WebPacketSimulator.Wpf     5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)   $��16(�5	   	    � 	Resources 5      &    WebPacketSimulator.Wpf.Component �� 
TargetType �� Property&    WebPacketSimulator.Wpf.WpfRouter   RouterImageWidth �� Value   RouterImageHeight �� RelativeSource �� Mode	 �� AncestorType
 �� Path!   WebPacketSimulator.Wpf.Menu��%�  '        '_�      '  	    �� 5   
   "  6   .�5      ��  �@ 
ImageWidth# Z 6+   �� 5       �@ ImageHeight# Z 6,   5      �� 5   
   " _�6   .�5      ��  � �� 6)   ��  �� $ FindAncestor=�$	 local:MainWindowL�$
 FontSize�5      �� 5   
   "  6   .�5      ��  � �� 6)   ��  �� $ FindAncestor=�$	 local:MainWindowL�$
 FontSize�5      5       J� InputBindings 5      �� 5   
   
 �� Key$ H��6    �� 	Modifiers$
 Ctrlf�5       (   "WebPacketSimulator.Wpf.HelpCommand   Instance �� Command# Z 5!      �� 5"   
   $ Delete��6   *   $WebPacketSimulator.Wpf.DeleteCommand   Instance# Z 5#      �� 5$   
   $ O��6   $
 Ctrlf�5%      (   "WebPacketSimulator.Wpf.OpenCommand   Instance# Z 5&      �� 5'   
   $ S��6   $
 Ctrlf�5(      (   "WebPacketSimulator.Wpf.SaveCommand   Instance# Z 5)      �� 5*   
   $
 Left��6   $
 Ctrlf�5+      5   /WebPacketSimulator.Wpf.SlowDownAnimationCommand   Instance# Z 5,      �� 5-   
   $ Right��6   $
 Ctrlf�5.      4	   .WebPacketSimulator.Wpf.SpeedUpAnimationCommand 	  Instance# Z 5/      �� 50   
   $	 Esc��6   /
  )WebPacketSimulator.Wpf.UnhighlightCommand 
  Instance# Z 51      �� 52   
   $ N��6   $
 Ctrlf�53      +   %WebPacketSimulator.Wpf.NewFileCommand   Instance# Z 54      .��57      � $��5q�6    � RowDefinitions 58   
   �� 59      $
��auto �6   �� 5:      $��* �6   �� 5;      $
��auto �6   �� 5<      $
��auto �6    � ColumnDefinitions 5>   
   �� 5?      $��* �6   �� 5@      $
��auto �6   �� 5A      $
��auto �6   .+�5D   
     +   %WebPacketSimulator.Wpf.AnimationSpeed  5E   
   $��1��6   $��2��6/   $��5 0 0 0q�6C   '   !WebPacketSimulator.Wpf.MainCanvas  5F   
   $��1��6   /   )WebPacketSimulator.Wpf.ComponentSelection  5G   
   $��3��6#   $��3��60   *   $WebPacketSimulator.Wpf.PacketConsole  5H   
   $��3��6   $��3��6+   *   $WebPacketSimulator.Wpf.MenuSelection  5I   
   $��2��6   $��3��6+   '   !WebPacketSimulator.Wpf.RouterData  5J   
   $��1��6   $��2��6(   5K      5L      