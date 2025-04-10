using System.Diagnostics;
using System.Windows;

namespace WindowsOptimizer.Views;

public partial class RestartDialog : Window
{
    public enum RestartResult
    {
        Now,
        Later,
        Cancel
    }

    public RestartDialog()
    {
        InitializeComponent();
    }

    public RestartResult Result { get; private set; } = RestartResult.Cancel;

    private void RestartNowButton_Click(object sender, RoutedEventArgs e)
    {
        Result = RestartResult.Now;
        TryRestartComputer();
        Close();
    }

    private void RemindLaterButton_Click(object sender, RoutedEventArgs e)
    {
        Result = RestartResult.Later;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Result = RestartResult.Cancel;
        Close();
    }

    private void TryRestartComputer()
    {
        try
        {
            // Attempt to restart the computer using shutdown.exe
            Process.Start("shutdown.exe",
                "/r /t 10 /c \"Windows Optimizer is restarting your computer to apply optimizations.\"");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to restart computer: {ex.Message}\n\nPlease restart manually.",
                "Restart Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}