   M S B A M L   `   `   `  ���� F  BQuantumTune, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:WindowsOptimizerWindowsOptimizer      WindowsOptimizer.App   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     0localclr-namespace:WindowsOptimizer       �� 	Resources  /   )WindowsOptimizer.ProgressToWidthConverter   progressToWidthConverter0   *WindowsOptimizer.BoolToVisibilityConverter  BoolToVisibilityConverter7   1WindowsOptimizer.InverseBoolToVisibilityConverter $  InverseBoolToVisibilityConverter+   %WindowsOptimizer.InverseBoolConverter  InverseBoolConverter+   %WindowsOptimizer.BoolToColorConverter  BoolToColorConverter2   ,WindowsOptimizer.OptimizationStatusConverter  OptimizationStatusConverter6   0WindowsOptimizer.OptimizationButtonTextConverter # OptimizationButtonTextConverter9   3WindowsOptimizer.OptimizationButtonSubtextConverter & "OptimizationButtonSubtextConverter3	   -WindowsOptimizer.OptimizationCommandConverter   OptimizationCommandConverter-
   'WindowsOptimizer.ImpactToColorConverter 	 ImpactToColorConverter/   )WindowsOptimizer.NavigationColorConverter 
 NavigationColorConverter  NavButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType �� Padding �� Triggers P� Property P� Value  ActionButtonStyle	 �� CornerRadius  SimpleScrollViewer  CustomCheckBoxStyle
 � ColumnDefinitions ��Name F�Name �� 
TargetName  ScanButtonStyle A�Name �� 
StartPoint �� EndPoint � Color � Offset :� RoutedEvent �� 
TargetName �� TargetProperty	 W� To g� Duration  CustomProgressBarStyle `� 	Converter �� Path �� RelativeSource  InfoPanelStyle  ScanningProgressStyle��%:  &	        &	      &	 
     &	      &	      &	      &	      &	 #     &	 (     &		 -     &	
 2     &	 7     &	 �    &	 �    &	     &	 �    &	 �    7   &	 �
    &	                      	  
    �� " ��.���  � ��
 ���� ��  � �� �#BBBBBB��  � ��$ 0q���  � ��$ 40����  � ��$ 13(���  � ��$
 Left=���  � ��$ 20,0,0,0q���  �@�FocusVisualStyle Z� ��  � �� �� " ��.R��� #��z��# z��._��� $��Center=� P�  �@J�IsMouseOver$
 True��. ���  � �� �#252525��  � �� �#FFFFFFP�  � ��$
 True��. ���  � �� �#303030�� " ��.���  � �� �#9B59B6��  � ��
 �������  � ��$ SemiBold"���  � ��$ 12(���  � ��$
 15,8q���  � ��$ 0q���  �@�FocusVisualStyle Z� ��  � �� �� " ��.R��� #��z��$	 4��# z��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver$
 True��. ���  � �� �#8E44ADP�  � ��$
 True��. ���  � �� �#7D3C98�� " ��.���  � ��$
 Auto=���  � ��$ Disabled=���  � ��
 ���� ��  � ��$ 0q��� " ��.���  � ��
 �������  � ��
 ���� ��  � ��$ 1q���  � �� �#666666��  �@�FocusVisualStyle Z� ��  � �� �� " ��.R�� 
 �� $
��Auto ��� $��* �.+��� $ CheckBoxBorder��$��18��$��18��
������ ���#666666$��1q�$	 2��._�F� $ 	CheckMark�����#9B59B6$��2��"���� 	= @T� � z @]� р�� @KL 	$y�	Collapsed=��� $��1��$��8,0,0,0q�$��Center=�$
��Left=� P�  �@d�	IsChecked$
 TrueY�. ��� $ 	CheckMark�� � y�$ Visible=��� $ CheckBoxBorder�� � �� �#333333�� $ CheckBoxBorder�� � �� �#9B59B6P�  �@J�IsMouseOver$
 True��. ��� $ CheckBoxBorder�� � �� �#9B59B6�� " ��.���  � ��$	 160����  � ��$	 160����  �@�FocusVisualStyle Z� ��  � �� �� " ��.R�� .+�A� $ OuterCircle��#��z��#��z������ $	 0,0&�$	 1,1&�.��� $ #8E44AD��$ 0Q�� $ #9B59B6��$ 1Q�A� $	��144��$	��144�����#252525�� $��Center=�$��Center=� :� $ 
MouseEnter��.[��� .���� .��W� $ OuterCircle��$ Opacity� 0.8$ 0:0:0.2D�:� $ 
MouseLeave��.[��� .���� .��W� $ OuterCircle��$ Opacity� 1$ 0:0:0.2D��� " �.���  � ��$ 6����  � �� �#333333��  � �� �#9B59B6��  � ��$ 0q���  � �� �� " �.R�� .+��� #��z��$	 3���� $ PART_Indicator��#��z��$	 3��$
��Left=���`� 8   .1��� $ Value� �� *TemplatedParent+�� $ Minimum� �� *TemplatedParent+�� $ Maximum� �� *TemplatedParent+�� $ ActualWidth� �� *TemplatedParent+�� " ��.���  � �� �#252525��  �@��CornerRadius$ 8����  �@��Padding$ 15q���  � ��$ 0,0,0,10q��� " ��.���  � ��$	 160����  � ��$	 160����  � y�$ 	Collapsed=�