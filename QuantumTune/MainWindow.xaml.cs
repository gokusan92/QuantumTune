using System.Windows;
using System.Windows.Input;
using WindowsOptimizer.ViewModels;

namespace WindowsOptimizer;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel; // This is essential
    }

    private void CloseWarning_MouseDown(object sender, MouseButtonEventArgs e)
    {
        WarningPanel.Visibility = Visibility.Collapsed;
    }
}