   M S B A M L   `   `   `  ����U  QWebPacketSimulator.Windows, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:WebPacketSimulator.WpfWebPacketSimulator.Wpf  5   	   '    !WebPacketSimulator.Wpf.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   6local$clr-namespace:WebPacketSimulator.Wpf     5   	   -   5          � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)    � 	Resources 5      &    WebPacketSimulator.Wpf.Component �� 
TargetType �� Property&    WebPacketSimulator.Wpf.WpfRouter   RouterImageWidth �� Value   RouterImageHeight��%�   '        �� 5   
   "  6   .�5      ��  �@ 
ImageWidth# Z 6+   �� 5       �@ ImageHeight# Z 6,   5      5      .��5      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��auto �6   �� 5      $
��auto �6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $
��auto �6   .+�5   
   �� -   5        	 ��Name$	 
MainCanvas��5      
�������$��3��5#      5$      �� 5'   
   
 �� Orientation$
 
Horizontal=�6   $��1��5(      $��2��5)      .+�5*      �� -   5        $��
Components��5*      {�5+      �� *IsComponentMenuEnabled+5,      �� 5-      -   5        $��Packet console��5-      {�5.      �� *IsPacketConsoleEnabled+5/      50      �� 53   
   -   5         ��Name$  ComponentSelectionListView��53      $��2��54      $��2��55       �� SelectedIndex$ 0��56      ��58      �� 59      .R�5:      �� $
 
Horizontal=�6!   5;      5<      .@�5>           ImageSource$ /Images/Router.png��6      RouterComponentText   Text# Z 5?        5@      $ /Images/Line.png��6      LineComponentText# Z 5A        5B      $ /Images/Cursor.png��6      SelectComponentText# Z 5C        5D      $ /Images/Packet.png��6      PacketComponentText# Z 5E      5F      �� 5I   
   -   5         ��Name$ PacketConsoleStackPanel��5I      $��2��5J      $��2��5K      .+�5L      �� -   5         ��Name$ PacketConsoleTextBlock��5L      5M      �� 5P   
   -   5        $ RouterDataStackPanel��5P      $
 Vertical=�5Q      $��1��5R      y�5S      �� *RouterDataVisibility+.+�5T      �� $��Name:��6   �� 5U      ��6   �� *HighlightedRouter.Name+ �� Mode$ TwoWay=��� 5V      $��IP address:��6   �� 5W      ��6   �� *HighlightedRouter.Address+$ TwoWay=�5X      5Y      5Z      