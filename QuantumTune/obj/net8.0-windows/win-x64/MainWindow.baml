   M S B A M L   `   `   `  ���� C  ?Learning, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:WindowsOptimizer.ViewModelsWindowsOptimizer.ViewModels  !    WindowsOptimizer.MainWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @
viewmodels)clr-namespace:WindowsOptimizer.ViewModels       � Title$  QuantumTune Max��$	��650��$	��950�� � WindowStartupLocation$ CenterScreen=����#1A1A1A���#252525$��1q�
�������$y�Visible=� � 
ResizeMode$ NoResize=�.���  � ColumnDefinitions �� $	��200 ��� $��* �.+��� $��0�����#202020._��  � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+�� $��0��$	��120��.+��� $��Center=�$��Center=�.+��� $��QuantumTune��
�������$��20(�$
��Bold"�$��Center=��� $��ALPHA�����#9B59B6$��14(�$
��Bold"�$��Center=�$��0,2,0,0q��� $��1��.+��� $��System Status��   NavButtonStyle#��[  $��SystemStatusPage������ *NavigateCommand+���� *CurrentPage+  NavigationColorConverter �� 	Converter# [  �� ConverterParameter$ SystemStatusViewModel���� $��System Information��#��[  $��SystemInfoPage������ *NavigateCommand+���� *CurrentPage+# [ $ SystemInfoViewModel���� $��Startup Items��#��[  $��StartupItemsPage������ *NavigateCommand+���� *CurrentPage+# [ $ StartupItemsViewModel���� $��Disk Cleaner��#��[  $��DiskCleanerPage������ *NavigateCommand+���� *CurrentPage+# [ $ DiskCleanerViewModel���� $��Settings��#��[  $��SettingsPage������ *NavigateCommand+���� *CurrentPage+# [ $ SettingsViewModel���� $��Support��#��[  $��SupportPage������ *NavigateCommand+���� *CurrentPage+# [ $ SupportViewModel���� $��2��$��Alpha Version 1.0.0�����#666666$��10(�$��0,0,0,10q�$��Center=�� $��1��.+�� ���#1A1A1A K� ZIndex$	 999��y��� *IsPageLoading+  BoolToVisibilityConverter# [ .+��� $��Center=�$��Center=�.+�A� $��80��$��80�����#9B59B6$��6�� J� Opacity$	 0.3Q��� $��
Loading...��$��18(�$
��Bold"�
�������$��0,20,0,0q�� -   	 �Name$	 WarningPanel��$��Stretch=�$	��Top=�$ 99999��$��0,60,0,0q����#111$��25�� �� $��* ��� $
��Auto �.+��� $��0��$<��6⚠️ Some optimizations require administrator rights�����#FFA500$��Center=�$��Center=��� -   $��1��$��X��
�������$��5,0,10,0q�$��Center=��� ���� *CurrentPage+
 � 	Resources
  �� DataType7   1WindowsOptimizer.ViewModels.SystemStatusViewModel  InfoPanelStyle  BoolToColorConverter  OptimizationStatusConverter  ScanButtonStyle $  InverseBoolToVisibilityConverter # OptimizationButtonTextConverter &	 "OptimizationButtonSubtextConverter 
 ScanningProgressStyle �� Left
 �� Top �� 
StartPoint �� EndPoint � Color � Offset �� CenterX �� CenterY �� Angle � Triggers :� RoutedEvent �� 
TargetName �� TargetProperty W� From	 W� To g� Duration g� RepeatBehavior	 �� X1	 �� Y1	 �� X2	  �� Y2! �� StringFormat5   /WindowsOptimizer.ViewModels.SystemInfoViewModel  SimpleScrollViewer" � Maximum  CustomProgressBarStyle# � Value7   1WindowsOptimizer.ViewModels.StartupItemsViewModel  ActionButtonStyle$ �� Padding% �� RadiusX& �� RadiusY  ImpactToColorConverter  CustomCheckBoxStyle' d� 	IsChecked( �� Mode  InverseBoolConverter) �� RelativeSource* �� AncestorType6   0WindowsOptimizer.ViewModels.DiskCleanerViewModel3   -WindowsOptimizer.ViewModels.SettingsViewModel+ �� SelectedValuePath, �� SelectedValue
- � Tag2   ,WindowsOptimizer.ViewModels.SupportViewModel��%�<  (��       *, +)7  7  7  7  7  7  7  7 	 7 
 7  7  (�� �    *, +)7  7  7  7  7  7  7  7  7  (�� 5    *, +)7  7  7  7  7  7  7  7  (�� �$    *, +)7  7  7  7  7  (�� �*    *, +)7  7  7  7  7  7  7  7  7  7  (�� V2    *, +)7  7  7  7  7  7  7  7  7  �� "  .R��  �� $��60 ��� $��* �.+��� $��0�����#252525���#333333$��0,0,0,1q�._��� $��SYSTEM ANALYSIS�����#BBBBBB$��16(�$��Medium"�$��Center=�$��20,0,0,0q�� $��1�� �� $��* ��� $
��Auto �.+�� $��0�� �� $��* ��� $
��Auto ��� $��* �.+��� $��0��$��Center=�$��Center=�.+��� 8��  $	��450��$��0,0,0,20q�._��  �� $
��Auto ��� $��* ��� $
��Auto �.+�A� $��0��$��12��$��12��$��0,0,10,0q����� *SystemNeedsOptimization+8  �� $��1��
�������$��Center=����� *SystemNeedsOptimization+8  �� $��2�����#FFA500$
��Bold"�$��Center=����� *ScanResults.IssuesCount+y��� *SystemNeedsOptimization+8  � $��1��$��Center=�.+��� 8�� ���� *ScanOrCleanCommand+y��� *
IsScanning+8  .���� .+��� 
�������$��24(�$
��Bold"�$��Center=����� *SystemNeedsOptimization+8  �� ���#BBBBBB$��12(�$��Center=�$��0,5,0,0q����� *SystemNeedsOptimization+8  �� 8�� y��� *
IsScanning+8 	 .+�A� $	��160��$	��160��$ 0��$ 0������ $	 0,0&�$	 1,1&�.��� $ #8E44AD��$ 0Q�� $ #9B59B6��$ 1Q�A� $	��144��$	��144��$ 8��$ 8�����#252525�� $	��160��$	��160��z��� SpinnerRotation��$ 80Q�$ 80Q�$ 0Q� :� $ Canvas.Loaded��.[��� .���� .��W� $ SpinnerRotation��$ Angle� 0 360$ 0:0:1.5D�$ Forever��.+��� $ 80��$ 10��$ 80��$  30�����#9B59B6$��2���� $	 115��$ 20��$	 105��$  37�����#9B59B6$��2��$	 0.9Q��� $	 140��$ 45��$	 123��$  55�����#9B59B6$��2��$	 0.8Q��� $	 150��$ 80��$	 130��$  80�����#9B59B6$��2��$	 0.7Q��� $	 140��$	 115��$	 123��$	  105�����#9B59B6$��2��$	 0.6Q��� $	 115��$	 140��$	 105��$	  123�����#9B59B6$��2��$	 0.5Q��� $ 80��$	 150��$ 80��$	  130�����#9B59B6$��2��$	 0.4Q��� $ 45��$	 140��$ 55��$	  123�����#9B59B6$��2��$	 0.3Q��� $ 20��$	 115��$ 37��$	  105�����#9B59B6$��2��$	 0.2Q��� $ 10��$ 80��$ 30��$  80�����#9B59B6$��2��$	 0.1Q��� $ 20��$ 45��$ 37��$  55�����#9B59B6$��2��$	 0.1Q��� $ 45��$ 20��$ 55��$  37�����#9B59B6$��2��$	 0.1Q��� $ 0��$ 55��$	��160��.+��� 
�������$��18(�$
��Bold"�$��Center=����� *ScanningText+�� ���#9B59B6$��14(�$��Center=�$��0,5,0,0q����� *ScanProgress+$
! {0}%���� $��2��8��
 $��50,20,50,10q�._��  �� $��40 ��� $��* �.+��� $��0��$��SYSTEM STATUS�����#BBBBBB$��12(�$��Center=�$��0q�� $��1�� �� $��* ��� $��* ��� $��* �.+��� $��0��$��Center=�.+��� ���#FFA500$��36(�$
��Bold"�$��Center=����� *ScanResults.IssuesCount+�� $��Issues Found�����#BBBBBB$��14(�$��Center=�$��0,5,0,0q��� $��1��$��Center=�.+��� ���#9B59B6$��36(�$
��Bold"�$��Center=����� *ScanResults.PerformanceScore+$
! {0}%���� $��Performance�����#BBBBBB$��14(�$��Center=�$��0,5,0,0q��� $��2��$��Center=�.+��� ���#9B59B6$��36(�$
��Bold"�$��Center=����� *'%ScanResults.FormattedRecoverableSpace+�� $��Recoverable�����#BBBBBB$��14(�$��Center=�$��0,5,0,0q��� $��1�����#252525$��30��._��� ���#999999$��11(�$��Center=�$��15,0,0,0q����� *LastScanText+�� "  .R��  �� $��60 ��� $��* �.+��� $��0�����#252525���#333333$��0,0,0,1q�._��� $��SYSTEM INFORMATION�����#BBBBBB$��16(�$��Medium"�$��Center=�$��20,0,0,0q��� $��1��8��  .���� $��20q�.+��� $��PROCESSOR (CPU)�����#9B59B6$��14(�$��Medium"�$��0,0,0,10q��� 8�� ._��  �� $	��120 ��� $��* � �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��0��$��Model:�����#BBBBBB�� $��0��$��1��
����������� *SystemInfo.CpuModel+�� $��1��$��0��$��Speed:�����#BBBBBB$��0,5,0,0q��� $��1��$��1��
�������$��0,5,0,0q����� *SystemInfo.CpuSpeed+�� $��2��$��0��$��Cores:�����#BBBBBB$��0,5,0,0q��� $��2��$��1��
�������$��0,5,0,0q����� *SystemInfo.CpuCores+�� $��3��$��0��$��Threads:�����#BBBBBB$��0,5,0,0q��� $��3��$��1��
�������$��0,5,0,0q����� *SystemInfo.CpuThreads+�� $��GRAPHICS (GPU)�����#9B59B6$��14(�$��Medium"�$��	0,20,0,10q��� ���� *SystemInfo.GpuList+���� .R��� 8�� ._��  �� $	��120 ��� $��* � �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��0��$��Model:�����#BBBBBB�� $��0��$��1��
����������� *Name+�� $��1��$��0��$��Driver:�����#BBBBBB$��0,5,0,0q��� $��1��$��1��
�������$��0,5,0,0q����� *DriverVersion+�� $��2��$��0��$��VRAM:�����#BBBBBB$��0,5,0,0q��� $��2��$��1��
�������$��0,5,0,0q����� *VideoMemory+�� $��MEMORY (RAM)�����#9B59B6$��14(�$��Medium"�$��	0,20,0,10q��� 8�� ._��  �� $	��120 ��� $��* �.+��� $��0��$��
Total RAM:�����#BBBBBB�� $��1��
����������� *SystemInfo.TotalRam+�� ���� *SystemInfo.RamModules+���� .R��� 8�� ._��  �� $	��120 ��� $��* � �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��0��$��Module:�����#BBBBBB�� $��0��$��1��
����������� *Manufacturer+�� $��1��$��0��$��	Capacity:�����#BBBBBB$��0,5,0,0q��� $��1��$��1��
�������$��0,5,0,0q����� *
Capacity+�� $��2��$��0��$��Speed:�����#BBBBBB$��0,5,0,0q��� $��2��$��1��
�������$��0,5,0,0q����� *Speed+�� $��STORAGE�����#9B59B6$��14(�$��Medium"�$��	0,20,0,10q��� ���� *SystemInfo.Disks+���� .R��� 8�� ._��  �� $	��120 ��� $��* � �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��0��$��Drive:�����#BBBBBB�� $��0��$��1��
����������� *DriveLetter+�� $��1��$��0��$��	Capacity:�����#BBBBBB$��0,5,0,0q��� $��1��$��1��
�������$��0,5,0,0q����� *	TotalSize+�� $��2��$��0��$��Free Space:�����#BBBBBB$��0,5,0,0q��� $��2��$��1��
�������$��0,5,0,0q����� *	FreeSpace+�� $��3��$��0��$��Usage:�����#BBBBBB$��0,5,0,0q�� $��3��$��1��$��0,10,0,5q� �� $
��Auto ��� $
��Auto �.+�� $��0��$	" 100Q�8�� # �� *UsedPercent+�� $��1��
�������$��0,5,0,0q����� *UsedPercent+$! {0:0}%���� $��OPERATING SYSTEM�����#9B59B6$��14(�$��Medium"�$��	0,20,0,10q��� 8�� ._��  �� $	��120 ��� $��* � �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��0��$	��OS:�����#BBBBBB�� $��0��$��1��
����������� *SystemInfo.OsName+�� $��1��$��0��$��Version:�����#BBBBBB$��0,5,0,0q��� $��1��$��1��
�������$��0,5,0,0q����� *SystemInfo.OsVersion+�� $��2��$��0��$��Build:�����#BBBBBB$��0,5,0,0q��� $��2��$��1��
�������$��0,5,0,0q����� *SystemInfo.OsBuild+�� $��3��$��0��$��Architecture:�����#BBBBBB$��0,5,0,0q��� $��3��$��1��
�������$��0,5,0,0q����� *SystemInfo.OsArchitecture+�� $��MOTHERBOARD�����#9B59B6$��14(�$��Medium"�$��	0,20,0,10q��� 8�� $��0,0,0,20q�._��  �� $	��120 ��� $��* � �� $
��Auto ��� $
��Auto �.+��� $��0��$��0��$��Manufacturer:�����#BBBBBB�� $��0��$��1��
����������� *$"SystemInfo.MotherboardManufacturer+�� $��1��$��0��$��Model:�����#BBBBBB$��0,5,0,0q��� $��1��$��1��
�������$��0,5,0,0q����� *SystemInfo.MotherboardModel+�� "  .R��  �� $��60 ��� $��* �.+��� $��0�����#252525���#333333$��0,0,0,1q�._��� $��STARTUP ITEMS�����#BBBBBB$��16(�$��Medium"�$��Center=�$��20,0,0,0q�� $��1�� �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��8��  $��20,20,20,10q�._��  �� $��* ��� $
��Auto �.+��� $��0��.+��� $��Optimize Startup Items��
�������$��16(�$��Medium"�$��0,0,0,5q��� $N��HDisabling unnecessary startup items can significantly improve boot time.�����#BBBBBB$
��Wrap=��� $��1��$��OPTIMIZE ALL��8�� $��Center=����� *OptimizeAllCommand+�� $��1��8�� $��20,10,20,10q�$$ 0q�._��� 8�� $��0q�.���� ���� *StartupItems+���� .R��� ���#2A2A2A$$ 15,12q�$��0,0,0,1q�._��  �� $��32 ��� $��* ��� $	��100 ��� $��80 �.+��� $��0��$��20��$��20��$% 3��$& 3������ *Impact+8  �� $��1��.+��� 
�������$��14(����� *Name+�� ���#999999$��12(����� *
Location+�� $��2��$��Center=����� *Impact+���� *Impact+8  �� $��3��8�� $��Center=�' �� *	IsEnabled+$( TwoWay=�{��� *IsEssential+8  ���� *&$DataContext.ToggleStartupItemCommand+) �� * ItemsControl���� �� $��2�����#252525$��30��._��� $=��7Manage startup applications to improve system boot time�����#999999$��11(�$��Center=�$��15,0,0,0q��� "  .R��  �� $��60 ��� $��* �.+��� $��0�����#252525���#333333$��0,0,0,1q�._��� $��DISK CLEANER�����#BBBBBB$��16(�$��Medium"�$��Center=�$��20,0,0,0q�� $��1�� �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��8��  $��20,20,20,10q�._��  �� $
��Auto ��� $��* ��� $
��Auto �.+�� $��0��$	��100��$	��100��.+�A� ���#9B59B6$	��100��$	��100��A� ���#1A1A1A$��60��$��60���� 
�������$��20(�$
��Bold"�$��Center=�$��Center=����� *UsedSpacePercent+$
! {0}%���� $��1��$��20,0,0,0q�$��Center=�.+��� 
�������$��16(�$��Medium"����� *TotalDiskSpace+$! C: Drive - {0}���� ���#9B59B6$��14(�$��0,5,0,0q����� *RecoverableSpace+$! {0} can be recovered���� $��2��$��
CLEAN DISK��8�� ���� *CleanupCommand+�� $��1��8�� $��20,10,20,10q�.���� ���� *CleanupItems+���� .R��� 8�� ._��  �� $��* ��� $
��Auto ��� $
��Auto �.+��� .+��� 
�������$��14(�$��Medium"����� *Name+�� ���#BBBBBB$��12(�$��0,3,0,0q����� *Description+�� $��1�����#9B59B6$��14(�$��Medium"�$��0,0,15,0q�$��Center=����� *Size+�� $��2��8�� $��Center=�' �� *
IsSelected+$( TwoWay=��� $��2�����#252525$��30��._��� ���#999999$��11(�$��Center=�$��15,0,0,0q����� *LastCleanupText+�� "  .R��  �� $��60 ��� $��* �.+��� $��0�����#252525���#333333$��0,0,0,1q�._��� $��SETTINGS�����#BBBBBB$��16(�$��Medium"�$��Center=�$��20,0,0,0q��� $��1��8��  $��20q�.���� .+��� $��GENERAL SETTINGS�����#9B59B6$��14(�$��Medium"�$��0,0,0,10q��� 8�� ._��  �� $��* ��� $
��Auto �.+��� .+��� $��Launch at Startup��
�������$��14(�$��Medium"��� $/��)Start QuantumTune Max when Windows starts�����#BBBBBB$��12(�$��0,3,0,0q��� $��1��8�� $��Center=�' �� *LaunchAtStartup+$( TwoWay=��� 8�� ._��  �� $��* ��� $
��Auto �.+��� .+��� $��Silent Mode��
�������$��14(�$��Medium"��� $'��!Minimize notifications and alerts�����#BBBBBB$��12(�$��0,3,0,0q��� $��1��8�� $��Center=�' �� *
SilentMode+$( TwoWay=��� $��SCANNING SETTINGS�����#9B59B6$��14(�$��Medium"�$��	0,20,0,10q��� 8�� ._��  �� $��* ��� $
��Auto �.+��� .+��� $��Scheduled Scans��
�������$��14(�$��Medium"��� $1��+Automatically scan and optimize your system�����#BBBBBB$��12(�$��0,3,0,0q��� $��1��8�� $��Center=�' �� *ScheduledScans+$( TwoWay=��� 8�� ._��  �� $��* ��� $
��Auto �.+��� .+��� $��Scanning Frequency��
�������$��14(�$��Medium"��� $-��'How often QuantumTune scans your system�����#BBBBBB$��12(�$��0,3,0,0q��� $��1��$	��120��$	+ Tag��$��Center=�, �� *ScanFrequency+$( TwoWay=�.@��� $��Daily��$- Daily���� $��Weekly��$- Weekly���� $��Monthly��$- Monthly���� $��ACCOUNT�����#9B59B6$��14(�$��Medium"�$��	0,20,0,10q��� 8�� $��0q�._��  �� $
��Auto ��� $
��Auto �.+��� $��0��.+��� $��Free Version��
�������$��14(�$��Medium"��� $.��(Upgrade to Premium for advanced features�����#BBBBBB$��12(�$��0,3,0,15q��� $��1��$��UPGRADE TO PREMIUM��8��	 $
��Left=��� "  .R��  �� $��60 ��� $��* �.+��� $��0�����#252525���#333333$��0,0,0,1q�._��� $��SUPPORT�����#BBBBBB$��16(�$��Medium"�$��Center=�$��20,0,0,0q�� $��1�� �� $
��Auto ��� $��* �.+��� $��0��8��  $��
20,20,20,0q�._��� .+��� $#��Get Help with QuantumTune Max��
�������$��18(�$��Medium"�$��0,0,0,15q��� $p��jWe're here to help you optimize your system and resolve any issues. Choose from the support options below:�����#BBBBBB$
��Wrap=�$��0,0,0,15q�� .+��� $��
USER GUIDE��8�� $��0,0,10,0q����� *OpenUserGuideCommand+�� $��CONTACT SUPPORT��8�� $��0,0,10,0q����� *ContactSupportCommand+�� $��VISIT FORUM��8�� ���� *VisitForumCommand+�� $��1��8�� $��20,20,20,20q�.���� .+��� $ ��FREQUENTLY ASKED QUESTIONS�����#9B59B6$��14(�$��Medium"�$��0,0,0,15q��� 8�� ._��� .+��� $(��"Why do I need system optimization?��
�������$��14(�$��Medium"��� $����Over time, your Windows system accumulates temporary files, registry errors, and startup programs that can slow down performance. QuantumTune Max identifies and fixes these issues to keep your computer running smoothly.�����#BBBBBB$��12(�$
��Wrap=�$��0,10,0,0q��� 8�� ._��� .+��� $*��$How often should I run optimization?��
�������$��14(�$��Medium"��� $����We recommend running a full system optimization at least once per week. However, heavily used computers may benefit from more frequent optimization. You can also set up scheduled scans in Settings.�����#BBBBBB$��12(�$
��Wrap=�$��0,10,0,0q��� 8�� ._��� .+��� $*��$Is it safe to disable startup items?��
�������$��14(�$��Medium"��� $����Yes, QuantumTune Max identifies which startup items are safe to disable. Items marked as 'Essential' are recommended to keep enabled. The program will never disable critical system components.�����#BBBBBB$��12(�$
��Wrap=�$��0,10,0,0q��� 8�� $��0q�._��� .+��� $5��/What's the difference between Free and Premium?��
�������$��14(�$��Medium"��� $����The Free version offers basic optimization and cleaning features. Premium unlocks advanced features including automatic optimization, real-time monitoring, privacy protection, and priority customer support.�����#BBBBBB$��12(�$
��Wrap=�$��0,10,0,0q�