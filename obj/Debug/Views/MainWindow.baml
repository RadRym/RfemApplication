   M S B A M L   `   `   `  ����J  FRfemApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:RfemApplicationRfemApplication  5   	   G(clr-namespace:RfemApplication.ConvertersRfemApplication.Converters  5   	        RfemApplication.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:RfemApplication     5   	   ?
converters(clr-namespace:RfemApplication.Converters     5   	     � Title$,  &RFEM Application - Analiza Konstrukcji��5	   	   $	��800��68   $
��1000��6E    � WindowStartupLocation$ CenterScreen=�5
   	    � 	Resources 5      8   2RfemApplication.Converters.InverseBooleanConverter   InverseBoolConverterA   ;RfemApplication.Converters.ConnectionStatusToColorConverter  StatusToColorConverter:   4RfemApplication.Converters.BoolToFontWeightConverter  BoolToFontWeightConverter<   6RfemApplication.Converters.StringToVisibilityConverter  StringToVisibilityConverter=   7RfemApplication.Converters.BooleanToVisibilityConverter   BooleanToVisibilityConverter:   4RfemApplication.Converters.BoolToVisibilityConverter  BoolToVisibilityConverter7   1RfemApplication.Converters.NullToBooleanConverter  NullToBooleanConverter8   2RfemApplication.Converters.BooleanToColorConverter  BooleanToColorConverter �� 
TargetType �� Property �� Value��%  &	        &	      &	 &     &	 9     &	 L     &	 _     &	 r     &	 �     '�� �     '�� )    '�� �      5   
   6-     5   
   66     5   
   6/     5   
   61     5   
   62     5   
   6/     5   
   6,     5   
   6-   �� 5   
   " ��6   .�5      ��  � ��$ 5q�6'   �� 5       � ��$
 10,5q�6(   �� 5       � ��$ 75��6)   5      �� 5   
   " ��6   .�5      ��  � ��$ 2q�6'   �� 5        � ��$ 5q�6(   �� 5!       � ��$ 1q�60   �� 5"       � �� �#CCCCCC6,   5#      �� 5%   
   " ��6   .�5&      ��  � ��$ 5q�6'   �� 5'       � ��$ 10q�6(   �� 5(       � �� �#DDDDDD6,   �� 5)       � ��$ 1q�60   5*      5+      .��5-      � ���#F8F9FA � RowDefinitions 5.   
   �� 5/      $
��Auto �6   �� 51      $
��Auto �6   �� 53      $��* �6   �� 55      $
��Auto �6   .+�5:   
   �� $��0��6   $#��🖥 Zarządzanie RFEM Server��6    $��
10,10,10,5q�6D   .��5;      �  5<      �� 5=      $
��Auto �6$   �� 5>      $
��Auto �6$   �� 5?      $
��Auto �6$   .+�5C      � $��0��6   $��5q�6$    � ColumnDefinitions 5D      �� 5E      $
��Auto �6+   �� 5F      $��* �6+   �� 5G      $
��Auto �6+   .+�5J      �� $��0��6    $��Ścieżka RFEM:��60   $��Center=�6E   $��0,0,10,0q�6`   �� 5K      $��1��6   ��6.   �� **(RfemServerViewModel.ServerExecutablePath+{�5L      �� * RfemServerViewModel.IsStarting+ �� 	Converter# [  �� 5M      $��2��6   $
��📁��6-   $��30��5N      $��5,0,0,0q�6(   	 � ToolTip$	 Przeglądaj��69   ��5M   :   �� *-+RfemServerViewModel.BrowseServerPathCommand+5N   9   5O      � 5R      $��1��6   $��5q�6$    5S      �� 5T      $
��Auto �6+   �� 5U      $	��150 �6+   �� 5V      $
��Auto �6+   �� 5W      $	��100 �6+   �� 5X      $
��Auto �6+   �� 5Y      $	��100 �6+   �� 5Z      $
��Auto �6+   �� 5[      $��60 �6+   �� 5\      $��* �6+   .+�5_      �� $��0��6    $��Email:��60   $��Center=�6>   $��0,0,5,0q�6Y   �� 5`      $��1��6   $��0,0,10,0q�6Y   ��6.   �� *RfemServerViewModel.Email+6Y   �� 5b      $��2��6    $��Hasło:��60   $��Center=�6>   $��0,0,5,0q�6Y   G� 5c      -   5        
 G�Name$
 PasswordBox��5c   2   $��3��6"   $��0,0,10,0q�6G   �� 5e      $��4��6    $��	Licencja:��60   $��Center=�6A   $��0,0,5,0q�6\   �� 5f      $��5��6   $��0,0,10,0q�6[   ��6.   �� *RfemServerViewModel.License+6[   �� 5h      $��6��6    $��Port:��60   $��Center=�6=   $��0,0,5,0q�6X   �� 5i      $��7��6   ��6.   �� *RfemServerViewModel.SoapPort+�� 5k      $��8��6   $��Tryb GUI��5l      $��Center=�62   $��20,0,0,0q�6M    d� 	IsChecked 5k   /   �� * RfemServerViewModel.UseGuiMode+5l   M   5m      � 5p      $��2��6   $��5q�6$    5q      �� 5r      $
��Auto �6+   �� 5s      $
��Auto �6+   �� 5t      $
��Auto �6+   �� 5u      $
��Auto �6+   �� 5v      $��* �6+   .+�5y      �� $��0��6   $��🚀 Uruchom Server��6-   ���#28A745
�������$��0,0,10,0q�5z   E   $
��12,6q�6W   $	��140��6f   ��5y   I   �� *(&RfemServerViewModel.StartServerCommand+5z   f   �� 5{      $��1��6   $��⏹ Zatrzymaj��6-   ���#DC3545
�������$��0,0,10,0q�5|   E   $
��12,6q�6W   $	��100��6f   ��5{   C   �� *'%RfemServerViewModel.StopServerCommand+5|   f   �� 5}      $��2��6   $��🔄 Odśwież��6-   ���#6C757D
�������$��0,0,15,0q�5~   E   $
��10,6q�6W   ��5}   B   �� **(RfemServerViewModel.RefreshStatusCommand+5~   W   �� 5�      $��3��6    $��Status:��60   $��Center=�6?   $
��Bold"�6Z   $��0,0,5,0q�6l   �� 5�      $��4��6    $��Center=�5�       $
��Bold"�6;   ��5�   0   �� *" RfemServerViewModel.ServerStatus+��5�      �� 5�       �� Path$) #RfemServerViewModel.IsServerRunning�6&   # [ 5�   &   5�      5�      5�      5�      �� 5�   
   $��1��6   $)��#🔗 Połączenie z RFEM WebService��6    $��	10,5,10,5q�6I   .��5�      �  5�      �� 5�      $
��Auto �6$   �� 5�      $
��Auto �6$   �� 5�      $
��Auto �6$    5�      �� 5�      $
��Auto �6'   �� 5�      $��* �6'   �� 5�      $
��Auto �6'   .+�5�      �� $��0��6   $��0��6)   $��Adres:��69   $��Center=�6G   $��5,0,10,0q�6b   �� 5�      $��0��6   $��1��6'   $��0,0,10,0q�5�      ��5�   7   �� *#!RfemConnectionViewModel.ServerUrl+{�5�      �� *&$RfemConnectionViewModel.IsConnecting+# [  5�      �� 5�      $��0��6   $��2��6*    �� Orientation$ 
Horizontal=�6:   .+�5�      �� $��	🔍 Test��6   ���#17A2B8
�������$��0,0,5,0q�5�   E   $
��15,5q�6V   $��70��6e   ��5�   /   �� */-RfemConnectionViewModel.TestConnectionCommand+5�   e   �� 5�      $��🔌 Połącz��6   ���#007BFF
�������$��0,0,5,0q�5�   E   $
��15,5q�6V   $��80��6e   ��5�   1   �� *(&RfemConnectionViewModel.ConnectCommand+5�   e   �� 5�      $��❌ Rozłącz��6   ���#FD7E14
�������$
��15,5q�5�   E   $��80��6T   ��5�   1   �� *+)RfemConnectionViewModel.DisconnectCommand+5�   T   5�      �� 5�      $��1��6   $��0��6)   $��Status:��69   $��Center=�6H   $��5,5,10,0q�6c   �� 5�      $��1��6   $��1��6)   $��2��69   $��0,5,5,0q�5�      $
��Bold"�6-   ��5�      �� **(RfemConnectionViewModel.ConnectionStatus+��5�      `� 5�       `� 	Converter# [ 6'   .1�5�      �� $) #RfemConnectionViewModel.IsConnected�6&   �� 5�      $* $RfemConnectionViewModel.IsConnecting�6&   5�      5�      �� 5�      $��2��6   $��0��6)   $��Błąd:��69   $	��Top=�6F   $��5,5,10,5q�6^   y�5�      �� *#!RfemConnectionViewModel.LastError+# [ �� 5�      $��2��6   $��1��6)   $��2��69   
���  ��$
��Wrap=�5�   -   $��0,5,5,5q�6A   ��5�      �� *#!RfemConnectionViewModel.LastError+y�5�      �� *#!RfemConnectionViewModel.LastError+# [ 5�      5�      �� 5�   
   $��2��6   $!��📊 Elementy konstrukcyjne��6    $
��10,5q�6C   .��5�      �  5�      �� 5�      $
��Auto �6$   �� 5�      $��* �6$   .+�5�      �� $��0��6   ���#F8F9FA���#DEE2E6$��0,0,0,1q�6Q    �� Padding$ 10q�6k   ._�5�      �  5�      �� 5�      $��* �6/   �� 5�      $
��Auto �6/   .+�5�      �� $��0��6%   $ 
Horizontal=�65   .+�5�      �� $
��🔍��6(   $��16(�62   $��Center=�6@   $��0,0,5,0q�6[   �� 5�      $��Szukaj:��6(   $��Center=�67   $��0,0,8,0q�6R   $��SemiBold"�6c   �� 5�      $	��250��5�   &   $��0,0,15,0q�62   $;	 5Szukaj po ID, materiale, przekroju, typie elementu...��5�   &   $	��8,5q�5�   &   ��5�   &   �� *
SearchText+ �� UpdateSourceTrigger$ PropertyChanged=�5�   &   �� 5�      $��Center=�5�   (   $��Italic$�6C   ���#6C757D��5�   (   �� *FilteredElements.Count+ �� StringFormat$  Znaleziono: {0} elementów�� �� Mode$ OneWay=�5�   V   5�      �� 5�      $��1��6%   $ 
Horizontal=�65   .+�5�      �� $��🔄 Odśwież��6%   $��0,0,8,0q�5�   %   $
��12,6q�66   ���#6C757D
���������5�   :   �� *RefreshCommand+5�   Z   �� 5�      $��📥 Pobierz z RFEM��6%   ���#007BFF
�������$
��12,6q�5�   M   $.	 (Pobierz elementy z aktywnego modelu RFEM��5�   %   ��5�   A   �� *LoadFromRfemCommand+5�   %   5�      5�      5�      � 5�      $��1��6   .+�5�      �� 
������� J� Opacity$	 0.9Q�60    K� ZIndex$ 10��5�      y�5�      �� *	IsLoading+# [ ._�5�      �� $��Center=�6%   $��Center=�6B   .+�5�      �� $	��⏳��6(   $��48(�61   $��Center=�6?   $��0,0,0,10q�6\   �� 5�      $��Ładowanie danych...��6(   $��Center=�6C   $��18(�5�   (   $
��Bold"�66   $��0,0,0,5q�6H   �� 5�      $,��&Pobieranie elementów z modelu RFEM...��6(   $��Center=�6U   $��14(�5�   (   ���#6C757D66   � 5�       � IsIndeterminate . $	��200��6A   $��6��6M   $��0,15,0,0q�6X   5�      5�      &	   System.Windows.Controls.DataGrid	  5�       	  AutoGenerateColumns .   	  GridLinesVisibility$ 
Horizontal=�5�       	  HeadersVisibility$ Column=�5�       	  AlternatingRowBackground �#F0F8FF 	  CanUserDeleteRows .   	  CanUserAddRows .   	  SelectionMode$ Single=�5      	  RowHeaderWidth$ 0Q�5     $��0q�5     ��5�      �� *FilteredElements+ �� SelectedItem 5�      �� *SelectedElement+ 	  ColumnHeaderStyle 5     �� 5     =
  7System.Windows.Controls.Primitives.DataGridColumnHeader" 
 6$   .�5  "   ��  � �� �#E6F3FF6?   �� 5  "    � ��$
 Bold"�6?   �� 5  "    � ��$	 8,5q�6<   �� 5	  "    � �� �#CCCCCC6@   �� 5
  "    � ��$ 0,0,1,1q�6D   5       	  RowStyle  5     �� 5     )  #System.Windows.Controls.DataGridRow"  6$   .�5  "   ��  � ��$ 2q�6<   ! �� Triggers! 5  "   P� 5  &   " P� Property" �@ 
IsSelected# P� Value# . . �5  *   ��  � �� �#4A90E26G   �� 5  *    � ��
 �����6G   5  '   P� 5  &   " �@J�IsMouseOver# . . �5  *   ��  � �� �#E3F2FD6G   5  '   5     $ 	  Columns$ 5     4  .System.Windows.Controls.DataGridCheckBoxColumn  5     ,  &System.Windows.Controls.DataGridColumn%   Width5  /System.Windows.Controls.DataGridLengthConverter$% 40 6T   1  +System.Windows.Controls.DataGridBoundColumn&   Binding& 65   �� *
IsSelected+'   Header' 5  "   �� 5  &   $	��✓��60   $	 Zaznacz element��69   5!     0  *System.Windows.Controls.DataGridTextColumn  5$     $' ID��6H   $% 80 6T   (   SortDirection$( 	AscendingX�6_   & 61   �� *ID+)   ElementStyle) 5%  "   �� 5&  &   " ��6,   .�5'  *   ��  � ��$
 Bold"�6G   �� 5(  *    � ��$ Center=�6P   5)  '   5+       5.     $	' Typ��6Q   $	% 100 6^   & 61   �� *ElementType+) 5/  "   �� 50  &   " ��6,   .�51  *   ��  � ��$ Center=�6P   ! 52  *   P� 53  .   " � ��$
# Beam��6F   . �54  2   ��  � �� �#2E7D326O   55  /   P� 56  .   " � ��$# Column��6F   . �57  2   ��  � �� �#1976D26O   58  /   P� 59  .   " � ��$# Truss��6F   . �5:  2   ��  � �� �#F57C006O   5;  /   5=  '   5?       5B     $' Długość [m]��6e   $	% 120 6z   & 61   �� *Length+$ {0:F2} m��) 5C  "   �� 5D  &   " ��6,   .�5E  *   ��  � ��$ Right=�6P   �� 5F  *    �@��Padding$ 0,0,8,0q�6D   5G  '   5I       5L     $' 	Przekrój��6R   $	% 150 6d   & 61   �� *CrossSection+) 5M  "   �� 5N  &   " ��6,   .�5O  *   ��  � ��$ Consolas)�6G   �� 5P  *    � ��$ Center=�6P   5Q  '   5S       5V     $' 	Materiał��6N   $% * 6`   & 61   �� *
Material+) 5W  "   �� 5X  &   " ��6,   .�5Y  *   ��  � ��$ Center=�6P   �� 5Z  *    � ��$ SemiBold"�6G   5[  '   5]     5_     5`     5a     5b     �� 5e  
   $��3��6   ���#343A40
�������.@�5f     �� .��5g     �� $ 
Horizontal=�6   .+�5h     �� $
��📊��6    $	��5,0q�6*   �� 5i     ��6    �� *FilteredElements.Count+$ Elementów: {0}��$ OneWay=�5j     5k     �� 5l     ���#6C757D6   �� 5m     .��5n     �� $ 
Horizontal=�6   .+�5o     �� $
��🔗��6    $	��5,0q�6*   �� 5p     ��6    �� **(RfemConnectionViewModel.ConnectionStatus+$ OneWay=�5q     5r     �� 5s     ���#6C757D6   �� 5t     .��5u     �� $ 
Horizontal=�6   .+�5v     �� $
��🖥��6    $	��5,0q�6*   �� 5w     ��6    �� *" RfemServerViewModel.ServerStatus+$ OneWay=�5x     5y     �� 5z     $��Right=�6   .��5{     �� $ 
Horizontal=�6   .+�5|     �� $��RFEM Application v1.0��6    $��Italic$�6=   $
��10,0q�6P   5}     5~     5     5�     5�     