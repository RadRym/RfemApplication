   M S B A M L   `   `   `  ����J  FRfemApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:RfemApplication.ConvertersRfemApplication.Converters  5      0    *RfemApplication.Views.ElementsDataGridView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      ?
converters(clr-namespace:RfemApplication.Converters     5        � 	Resources  5
      =   7RfemApplication.Converters.BooleanToVisibilityConverter    BooleanToVisibilityConverter4   .RfemApplication.Converters.IsNegativeConverter  IsNegativeConverter4   .RfemApplication.Converters.IsPositiveConverter  IsPositiveConverter&   System.Windows.Controls.DataGrid �� 
TargetType �� Property �� Value=  7System.Windows.Controls.Primitives.DataGridColumnHeader)  #System.Windows.Controls.DataGridRow �� Triggers P� Property P� Value��%F  &	        &	      &	 &     '  9     '  �    '  �      5   
   62     5   
   6)     5   
   6)   �� 5   
   "  6   .�5      ��  �@ GridLinesVisibility$ 
Horizontal=�64   �� 5       �@ HeadersVisibility$ Column=�62   �� 5        �@ AlternatingRowBackground �#F0F8FF69   �� 5       �@ CanUserDeleteRows .  62   �� 5       �@ CanUserAddRows .  6/   �� 5       �@ SelectionMode$ Single=�6.   �� 5       �@ RowHeaderWidth$ 0Q�6/   5      �� 5   
   "  6   .�5      ��  � �� �#E6F3FF6+   �� 5       � ��$
 Bold"�6+   �� 5       � ��$	 8,5q�6(   �� 5       � �� �#CCCCCC6,   �� 5       � ��$ 0,0,1,1q�60   5      �� 5!   
   "  6   .�5"      ��  � ��$ 2q�6(    5#      P� 5$       �@ 
IsSelected . . �5%      ��  � �� �#4A90E263   �� 5&       � ��
 �����63   5'      P� 5(       �@J�IsMouseOver . . �5)      ��  � �� �#E3F2FD63   5*      5,      5-      .��5/      �  � RowDefinitions 50   
   �� 51      $
��Auto �6   �� 52      $��* �6   .+�56   
   �� $��0��6   ���#F8F9FA���#DEE2E6$��0,0,0,1q�6I    �� Padding$ 10q�6c   ._�57      � 	 � ColumnDefinitions	 58      �� 59      $��* �6'   �� 5:      $
��Auto �6'   .+�5>      �� $��0��6   
 �� Orientation$
 
Horizontal=�6-   .+�5?      �� $
��🔍��6    $��16(�6*   $��Center=�68   $��0,0,5,0q�6S   �� 5@      $��Szukaj:��6    $��Center=�6/   $��0,0,8,0q�6J   $��SemiBold"�6[   �� 5A      $	��250��5B      $��0,0,15,0q�6*    � ToolTip$; 5Szukaj po ID, materiale, przekroju, typie elementu...��5C      $	��8,5q�5D      ��5A      �� *
SearchText+ �� UpdateSourceTrigger$ PropertyChanged=�5D      �� 5E      $��Center=�5F       $��Italic$�6;   ���#6C757D��5E       �� *FilteredElements.Count+ �� StringFormat$  Znaleziono: {0} elementów��5F   N   5G      �� 5J      $��1��6   $
 
Horizontal=�6-   .+�5K      �� $��🔄 Odśwież��6   $��0,0,8,0q�5L      $
��12,6q�6.   ���#6C757D
���������5K   2   �� *RefreshCommand+5L   R   �� 5M      $��✓ Zaznacz wszystkie��6   $��0,0,8,0q�5N      $
��12,6q�6.   ���#28A745
���������5M   ;   �� *SelectAllCommand+5N   R   �� 5O      $��📥 Pobierz z RFEM��6   ���#007BFF
�������$
��12,6q�5P   E   $. (Pobierz elementy z aktywnego modelu RFEM��5Q      ��5O   9   �� *LoadFromRfemCommand+5Q      5R      5S      5T      � 5W   
   $��1��6   .+�5Y      �� 
������� J� Opacity$	 0.9Q�6(    K� ZIndex$ 10��5[      y�5Z      �� *	IsLoading+ �� 	Converter# [  ._�5\      �� $��Center=�6   $��Center=�6:   .+�5]      �� $	��⏳��6    $��48(�6)   $��Center=�67   $��0,0,0,10q�6T   �� 5^      $��Ładowanie danych...��6    $��Center=�6;   $��18(�5_       $
��Bold"�6.   $��0,0,0,5q�6@   �� 5`      $,��&Pobieranie elementów z modelu RFEM...��6    $��Center=�6M   $��14(�5a       ���#6C757D6.   � 5b       � IsIndeterminate . $	��200��69   $��6��6E   $��0,15,0,0q�6P   5c      5d        5g         AutoGenerateColumns .  $��0q�5j      ��5g      �� *FilteredElements+ �� SelectedItem 5h      �� *SelectedElement+   Columns 5k      4  .System.Windows.Controls.DataGridCheckBoxColumn  5m      ,  &System.Windows.Controls.DataGridColumn   Width5	  /System.Windows.Controls.DataGridLengthConverter$ 40	 6L   1
  +System.Windows.Controls.DataGridBoundColumn 
  Binding 6-   �� *
IsSelected+   Header 5n      �� 5o      $ Zaznacz/Odznacz wszystkie��6'   5q      0  *System.Windows.Controls.DataGridTextColumn  5t      $ ID��6@   $ 60	 6L      SortDirection$ 	AscendingX�6W    6)   �� *ID+ 
  ElementStyle 5u      �� 5v      " ��6$   .�5w   "   ��  � ��$
 Bold"�6?   �� 5x   "    � ��$ Center=�6H   5y      5{        5~      $	 Typ��6I   $ 80	 6V    6)   �� *ElementType+ 5      �� 5�      " ��6$   .�5�   "   ��  � ��$ Center=�6H    5�   "   P� 5�   &    � ��$
 Beam��6>   . �5�   *   ��  � �� �#2E7D326G   5�   '   P� 5�   &    � ��$ Column��6>   . �5�   *   ��  � �� �#1976D26G   5�   '   P� 5�   &    � ��$ Truss��6>   . �5�   *   ��  � �� �#F57C006G   5�   '   5�      5�        5�      $ 
Długość��6]   $ 85	 6n    6)   �� *Length+$ {0:F2} m�� 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ Right=�6H   �� 5�   "    �@��Padding$ 0,0,8,0q�6<   5�      5�        5�      $ 	Przekrój��6J   $	 120	 6\    6)   �� *CrossSection+ 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ Consolas)�6?   �� 5�   "    � ��$ Center=�6H   5�      5�        5�      $ 	Materiał��6F   $ 80	 6X    6)   �� *
Material+ 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ Center=�6H   �� 5�   "    � ��$ SemiBold"�6?   5�      5�        5�      $ Węzły��6L   $	 100	 6[    6)   �� *NodeConnection+ 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ Center=�6H   �� 5�   "    � ��$ Consolas)�6?   �� 5�   "    � ��$ 11(�6=   5�      5�        5�      $ Przypadek obciążenia��6F   $	 140	 6d    6)   �� *
LoadCase+ 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ 11(�6=   �� 5�   "    �@��Padding$	 4,0q�6<   5�      5�        5�      $ N [kN]��6b   $ 80	 6r    6)   �� *
AxialForce+$ 	{0:F1} kN�� 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ Right=�6H   �� 5�   "    �@��Padding$ 0,0,8,0q�6<   �� 5�   "    � ��$ Consolas)�6?    5�   "   �� 5�   &    �� Binding 62   �� *
AxialForce+# [  �� Value True6�   .a�5�   *   ��  � �� �#D32F2F6G   5�   '   �� 5�   &    62   �� *
AxialForce+# [  True6�   .a�5�   *   ��  � �� �#388E3C6G   5�   '   5�      5�        5�      $ V [kN]��6b   $ 80	 6r    6)   �� *
ShearForce+$ 	{0:F1} kN�� 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ Right=�6H   �� 5�   "    �@��Padding$ 0,0,8,0q�6<   �� 5�   "    � ��$ Consolas)�6?   5�      5�        5�      $ M [kNm]��6f   $ 90	 6w    6)   �� *BendingMoment+$ 
{0:F1} kNm�� 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ Right=�6H   �� 5�   "    �@��Padding$ 0,0,8,0q�6<   �� 5�   "    � ��$ Consolas)�6?   5�      5�       � ContextMenu 5�      �� 5�      .@�5�      m� $��🔍 Pokaż szczegóły��6#    m� Command 6?   �� *ShowElementDetailsCommand+m� 5�      $��📊 Wykres sił��6#    6:   �� *ShowForcesDiagramCommand+�� 5�      m� 5�      $��📋 Kopiuj dane��6#    6;   �� *CopyElementDataCommand+m� 5�      $��📤 Eksportuj do Excel��6#    6B   �� *ExportToExcelCommand+�� 5�      m� 5�      $��🗑 Usuń element��6#    6<   �� *DeleteElementCommand+5�      5�      5�      5�      5�      