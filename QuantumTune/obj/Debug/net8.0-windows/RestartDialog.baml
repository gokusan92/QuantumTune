   M S B A M L   `   `   `  ����  QuantumTune*    $WindowsOptimizer.Views.RestartDialog   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  System Restart Required��5   	   $	��250��5   	   $	��450��6    � WindowStartupLocation$ CenterScreen=�5   	   ���#111111���#FFFFFF � 
ResizeMode$ NoResize=�5	   	   .��5
      � $��20q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6    �� Orientation$ 
Horizontal=�6"   $��0,0,0,15q�6;   .+�5      �� $!��/Resources/restart_icon.png��6   $��32��69   $��32��6D   $��0,0,10,0q�6P   �� 5      $ ��System Restart Recommended��6   $��18(�5      $��SemiBold"�6&   $��Center=�5      5      �� 5   
   $��1��6   $
��Wrap=�6!   $��0,0,0,15q�65   �� 5      $���Some optimization changes require a system restart to take full effect. Restarting your computer now will ensure all optimizations are properly applied.��5      �� 5      �� 6   �� 5      $2�,Would you like to restart your computer now?��6   5      � 5"   
   $��2��6    � ColumnDefinitions 5#      �� 5$      $��* �6#   �� 5%      $
��Auto �6#   �� 5&      $
��Auto �6#   .+�5)      �� -   5         ��Name$ RemindLaterButton��5)      $��0��60   $��Remind me later��6@   $
��Left=�5*      $
��10,5q�60   6?   �� 5,      -   5        $ CancelButton��5,      $��1��6+   $��Don't restart��6;   $
��10,0q�5-      $
��10,5q�6#   62   �� 5/      -   5        $ RestartNowButton��5/      $��2��6/   $��Restart now��6?   ���#0078D7
�������$
��10,5q�51      6$   52      53      54      