   M S B A M L   `   `   `  ����   QuantumTune*    $WindowsOptimizer.Views.RestartDialog   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  System Restart Required��$	��250��$	��450�� � WindowStartupLocation$ CenterScreen=����#111111���#FFFFFF � 
ResizeMode$ NoResize=�.��� $��20q� � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0�� �� Orientation$ 
Horizontal=�$��0,0,0,15q�.+��� $!��/Resources/restart_icon.png��$��32��$��32��$��0,0,10,0q��� $ ��System Restart Recommended��$��18(�$��SemiBold"�$��Center=��� $��1��$
��Wrap=�$��0,0,0,15q��� $���Some optimization changes require a system restart to take full effect. Restarting your computer now will ensure all optimizations are properly applied.���� �� �� $2�,Would you like to restart your computer now?��� $��2�� � ColumnDefinitions �� $��* ��� $
��Auto ��� $
��Auto �.+��� -    ��Name$ RemindLaterButton��$��0��$��Remind me later��$
��Left=�$
��10,5q��� -   $ CancelButton��$��1��$��Don't restart��$
��10,0q�$
��10,5q��� -   $ RestartNowButton��$��2��$��Restart now�����#0078D7
�������$
��10,5q�