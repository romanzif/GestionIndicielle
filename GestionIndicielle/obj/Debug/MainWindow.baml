   M S B A M L   `   `   `  ����L  HGestionIndicielle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:GestionIndicielle.InfrastrucureGestionIndicielle.Infrastrucure  5   	   "    GestionIndicielle.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   W SOxyPlot.Wpf, Version=2014.1.464.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0%oxyhttp://oxyplot.codeplex.com  5   	   Ainfrastrucure-clr-namespace:GestionIndicielle.Infrastrucure  5   	   $��671.5��5   	   $
��1194��6     J� SnapsToDevicePixels  . ��5      �� 5	   
    �� ImageSource$ Images/design.jpg��6   .��5      �  J� RenderTransformOrigin$ -0.097,0.046&�6   $��0,0,-304,-1q�60    � RowDefinitions 5   
   �� 5      $��0* �6   �� 5      .+�5   
   �� $��1q�6   
�������$
��Left=�68   $	��530��6S   $��
317,89,0,0q�6`   $��2��6t   $	��Top=�6�   $	��845��6�   $ 0.055,-0.19&�6�    J� OpacityMask �	#FFC73939��5      �� 5       �� EndPoint$ 0.5,1&�6&    �� 
StartPoint$ 0.5,0&�67   .��5      �  � Color$ Black��6#    � Offset$ 0Q�61   � 5      $ 	#FF1A89E6��6#   $ 1Q�65   5      5      �� 5   
   $��1q�6   
�������$
��Left=�68   $	��560��6S   $��	23,59,0,0q�6`   $��2��6s   $	��Top=�6�   $	��267��6�   $ 0.055,-0.19&�6�    �	#FFC73939��5      �� 5      $ 0.5,1&�6&   $ 0.5,0&�67   .��5      � $ Black��6#   $ 0Q�61   � 5      $ 	#FF1A89E6��6#   $ 1Q�65   5      5!      (  "System.Windows.Controls.DatePicker  5#   
   -   5        	  Name$	 Date_de_début��5#      $
��Left=�6,   
   Text$
 Date de début��5%      $��
71,227,0,0q�5&      $	��Top=�5'      $ 0.497,1.083&�5(      ���	#FF1B3281   DisplayDateStart$ 
2006-01-02X�6B   $��1��6`   $��24��6m   $	��150��6y      SelectedDate 5$      �� *TDebut+ �� Mode$ TwoWay=���5)      �� 5*      $ 0.5,0&�6&   $ 0.5,1&�67   .��5+      � $ 	#FFA3AEB9��6#   $ 0Q�65   � 5,      $ 	#FF8399A9��6#   $ 0.375Q�65   � 5-      $ 	#FF718597��6#   $ 0.375Q�65   � 5.      $ 	#FF657D8F��6#   $ 1Q�65   5/      51        52   
   -   5        $	 Date_de_fin��52      $
��Left=�6*   $��
71,285,0,0q�54      $
 Date de fin��55      $	��Top=�56      $ 0.497,1.083&�57      ���	#FF112B83$ 
2006-01-17X�6B   $��1��6`   $��24��6m   $	��150��6y    53      �� *TFin+$ TwoWay=�57   y   �� 59   
    � ToolTip$ rendement espéré��6   $
��Left=�6/   $��24��6J   $��
50,102,0,0q�6V   $��2��6j   $
��Wrap=�6{   $��Target Return��6�   $	��Top=�6�   �� 5:   
   $ rendement espéré��6   $
��Left=�6-   $��21��6H   $��
197,99,0,0q�6T   $��2��6h    �� TextWrapping$
 Wrap=�6y   $	��Top=�6�   $��65��6�   ��6�   �� *RelativeTargetReturn+$ TwoWay=�6�   �� 5;   
   $  en nombre de jours ouvrés��6   $
��Left=�66   $��21��6Q   $��197,178,0,0q�6]   $
 Wrap=�6r   $	��Top=�6�   $��65��6�   $ 0.405,-3.083&�6�   $��1��6  ��6�   �� *PeriodeRebalancement+$ TwoWay=�6  �� 5<   
   $  en nombre de jours ouvrés��6   $
��Left=�66   $��21��6Q   $��197,152,0,0q�6]   $
 Wrap=�6r   $	��Top=�6�   $��65��6�   $ 0.483,-0.619&�6�   $��1��6�   ��6�   �� *PeriodeEstimation+$ TwoWay=�6�   �� 5=   
   $U Oaprès avoir saisi toutes vos configurations, validez pour afficher les courbes��6   $��OK��6j   $
��Left=�6w   $��26��6�   $��172,582,0,0q�6�   $	��Top=�6�   $��90��6�   $ -2.585,-1.393&�6�   $��1��6  ��6�   �� *	OkCommand+6  �� 5>   
   $  en nombre de jours ouvrés��6   $
��Wrap=�68   $��21��6L   $��Periode d'estimation��6X   $	��Top=�6t   $��50,152,1311,0q�6�   $��1��6�   �� 5?   
   $  en nombre de jours ouvrés��6   $
��Left=�68   $��24��6S   $��
50,178,0,0q�6_   $
��Wrap=�6s   $��Période de balancement��6�   $	��Top=�6�   $��1��6�   $	��129��6�   �� 5@   
   $2 ,saisissez votre budget initial (ex : 105,47)��6   $
��Left=�6K   $��21��6f   $��
50,126,0,0q�6r   $
��Wrap=�6�   $��Budget Initial��6�   $	��Top=�6�   $��1��6�   $��98��6�   $ 0.653,0.762&�6�   �� 5B   
   $2 ,saisissez votre budget initial (ex : 105,47)��6   $
��Left=�6I   $��21��6d   $��197,126,0,0q�6p   $
 Wrap=�6�   $	��Top=�6�   $��65��6�   $��1��6�   $ -6.762,-4.74&�6�   ��6�   ��  �� Path$ Budget�$ TwoWay=�6�   �� 5C   
   $A ;valeur de la tracking error pour les configurations saisies��6   $
��Left=�6Z   $��24��6u   $��945,107,0,0q�6�   $��1��6�   $
��Wrap=�6�   $��Tracking Error��6�   $	��Top=�6�   $��77��6�   $ 0.864,0.458&�6�   �� 5D   
   $C =valeur du ratio d'information pour les configurations saisies��6   $
��Left=�6\   $��27��6w   $��738,107,0,0q�6�   $��1��6�   $
��Wrap=�6�   $��Ratio d'informations��6�   $	��Top=�6�   $	��113��6�   $ 0.736,0.556&�6�   �� 5E   
   $C =valeur du ratio d'information pour les configurations saisies��6   $
��Left=�6Z   $��25��6u   $��856,105,0,0q�6�   $��1��6�   $
 Wrap=�6�   $	��Top=�6�   $��75��6   �� 
IsReadOnly . $ 1.126,0.333&�6  ��6�   �� $ RatioInformation�$ TwoWay=�6  /   )GestionIndicielle.Infrastrucure.ListBoxEx  5F   
   $" liste des titres disponibles��6"   $	��150��6k   $	��237��6w   $
��Left=�6�   $��
71,328,0,0q�6�    �� SelectionMode$ Multiple=�6�   $��1��6�   $	��Top=�6�   $ 0.5,0.5&�6�   ��6I   �� *	AssetList+   SelectedItems 6  �� *SelectedItems+�� 5G   
   $
��Left=�6   $��22��6-   $A ;valeur de la tracking error pour les configurations saisies��69   $��1027,107,0,0q�6   $��1��6�   $
 Wrap=�6�   $	��Top=�6�   $��78��6�    . $ 1.268,0.458&�6  ��6�   �� $ TrackingError�$ TwoWay=�6  �� 5H   
   $
��Left=�6   $��59��6/   $��	351,0,0,0q�6;   $��2��6N   $
��Wrap=�6_   $��Gestion Indicielle��6s   $	��Top=�6�   $	��696��6�   $��Impact)�6�   $��48(�6�    �� TextAlignment$ Center=�6�   &   System.Windows.Media.TextOptions   TextHintingMode$ Animated=�6�   � 5L   
   $
��Left=�6   $	��248��6*   $��328,145,0,0q�67   $��2��6L   $	��Top=�6]   $	��804��6u   $ 	0.5,0.615&�6�    5M      �� 5N      $��7* �6    �� 5O      $
��184* �6     � ColumnDefinitions 5Q      �� 5R      $��0* �6#   �� 5S      $��4* �6#   �� 5T      $
��307* �6#   �� 5U      $
��416* �6#   .+�5W        OxyPlot.Wpf.Plot  -   5          Name$ Plot1��5W        OxyPlot.Wpf.PlotView   Title$ A Graph��6&   $��0,0,10,0q�6R   $��2��6d   $��2��6v   $��Right=�6�   $	��756��6�   $��2��6�      Model 66   �� *	PlotModel+6�   5X      � 5Z   
   $
��Left=�6   $	��210��6*   $��328,398,0,0q�67   $��2��6L   $	��Top=�6]   $	��804��6u    5[      �� 5\      $��7* �6    �� 5]      $
��184* �6     5_      �� 5`      $��0* �6#   �� 5a      $��3* �6#   �� 5b      $
��503* �6#   .+�5d        -   5        $ Plot2��5d      $ B Graph��6&   $��	33,0,10,0q�6S   $��2��6f   $��2��6x    66   �� *
PlotModel2+6x   5f      �� 5g   
   $
��Left=�6   $��21��6/   $��
136,69,0,0q�6;   $
��Wrap=�6O   $	��Top=�6c   $��1��6{   $��43��6�   $ 0.653,0.762&�6�   �� 6�   $
��Bold"�6�   $�Setups��6�    �� TextDecorations$ 	Underline{�6�   6�   �� 5h   
   $
��Left=�6   $��15��6/   $��
76,207,0,0q�6;   $��2��6O   $
��Wrap=�6`   $��Date de début��6t   $	��Top=�6�   $	��145��6�   $
��Bold"�6�   $ Center=�6�   �� 5i   
   $
��Left=�6   $��21��6/   $��
71,264,0,0q�6;   $��2��6O   $
��Wrap=�6`   $��Date de fin��6t   $	��Top=�6�   $	��150��6�   $
��Bold"�6�   $ Center=�6�   �� 5j   
   $ rendement espéré��6   $
��Left=�6/   $��24��6J   $��267,102,0,0q�6V   $��2��6k   $
��Wrap=�6|   $	��Top=�6�   �� 6�   $�%��6�   �� 6�   �� 6�   6�   5l      5n      