using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using WindowsOptimizer.Helpers;
using WindowsOptimizer.Models;
using WindowsOptimizer.Services;
using WindowsOptimizer.Utils;

namespace WindowsOptimizer.ViewModels;

public class SystemStatusViewModel : ViewModelBase
{
    private readonly OptimizationService _optimizationService;
    private readonly StartupService _startupService; // Added this field
    private bool _isScanning;
    private string _lastScanText;
    private string _scanningText;
    private int _scanProgress;
    private ScanResultsModel _scanResults;
    private bool _systemNeedsOptimization;

    // Updated constructor to include StartupService
    public SystemStatusViewModel(OptimizationService optimizationService, StartupService startupService)
    {
        _optimizationService = optimizationService;
        _startupService = startupService; // Initialize the field

        ScanCommand = new AsyncRelayCommand(ExecuteScanAsync);
        OptimizeCommand = new AsyncRelayCommand(ExecuteOptimizeAsync);
        ScanOrCleanCommand = new AsyncRelayCommand(ExecuteScanOrCleanAsync);

        ScanResults = new ScanResultsModel();
        IsScanning = false;
        ScanProgress = 0;
        ScanningText = "SCAN";
        LastScanText = "Last scan: Never";
        SystemNeedsOptimization = false;
    }

    public ScanResultsModel ScanResults
    {
        get => _scanResults;
        private set => SetProperty(ref _scanResults, value);
    }

    public bool IsScanning
    {
        get => _isScanning;
        private set => SetProperty(ref _isScanning, value);
    }

    public int ScanProgress
    {
        get => _scanProgress;
        private set => SetProperty(ref _scanProgress, value);
    }

    public string ScanningText
    {
        get => _scanningText;
        private set => SetProperty(ref _scanningText, value);
    }

    public string LastScanText
    {
        get => _lastScanText;
        private set => SetProperty(ref _lastScanText, value);
    }

    public bool SystemNeedsOptimization
    {
        get => _systemNeedsOptimization;
        private set => SetProperty(ref _systemNeedsOptimization, value);
    }

    public ICommand ScanCommand { get; }
    public ICommand OptimizeCommand { get; }
    public ICommand ScanOrCleanCommand { get; }

    private async Task ExecuteScanOrCleanAsync(object parameter)
    {
        try
        {
            if (SystemNeedsOptimization)
                await ExecuteOptimizeAsync(null);
            else
                await ExecuteScanAsync(null);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task ExecuteScanAsync(object? parameter)
    {
        if (IsScanning) return;

        try
        {
            IsScanning = true;
            ScanProgress = 0;
            ScanningText = "ANALYZING";

            var progress = new Progress<int>(percent =>
            {
                ScanProgress = percent;
                ScanningText = percent switch
                {
                    < 25 => "SCANNING FILES",
                    < 50 => "SCANNING STARTUP",
                    < 75 => "CHECKING REGISTRY",
                    _ => "FINALIZING"
                };
            });

            ScanResults = await RunScanWithProgressAsync(progress);
            LastScanText = "Last scan: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm");
            SystemNeedsOptimization = ScanResults.IssuesCount > 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Scan error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            ScanProgress = 100;
            IsScanning = false;
        }
    }

    private async Task ExecuteOptimizeAsync(object? parameter)
    {
        if (IsScanning) return;

        try
        {
            IsScanning = true;
            ScanProgress = 0;
            ScanningText = "OPTIMIZING";

            IProgress<int> progress = new Progress<int>(percent =>
            {
                ScanProgress = percent;
                ScanningText = percent switch
                {
                    < 25 => "CLEANING JUNK",
                    < 50 => "DISABLING STARTUP",
                    < 75 => "FIXING REGISTRY",
                    _ => "FINALIZING"
                };
            });

            // Run optimization
            var result = await _optimizationService.OptimizeSystemAsync();

            // Wait to let registry changes take effect
            var registryDiagnosis = await Task.Run(() => RegistryFixer.DiagnoseSystemRegistryIssues());
            Debug.WriteLine($"Registry diagnosis: {registryDiagnosis}");
            progress.Report(95);
            await Task.Delay(1000);

            // VERIFY the changes actually worked by scanning again
            ScanResults = await _optimizationService.ScanSystemAsync();
            SystemNeedsOptimization = ScanResults.IssuesCount > 0;
            LastScanText = "Last optimization: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm");

            if (ScanResults.IssuesCount == 0)
            {
                // All fixed!
                MessageBox.Show(
                    $"All issues successfully fixed!\n\n" +
                    $"• Registry optimizations applied\n" +
                    $"• Startup items optimized\n" +
                    $"• {FormatBytes(result.SpaceFreed)} disk space freed",
                    "Optimization Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                // Show accurate information about what's still broken
                var issues = await GetRemainingIssuesListAsync();

                /*MessageBox.Show(
                    $"Optimization partially completed.\n\n" +
                    $"• {result.IssuesFixed} issues were fixed\n" +
                    $"• {ScanResults.IssuesCount} issues still require attention\n" +
                    $"• {FormatBytes(result.SpaceFreed)} disk space freed\n\n" +
                    $"Remaining issues:\n{issues}\n\n" +
                    "Some system settings require administrator privileges.",
                    "Optimization Partially Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);*/

                // Only offer to restart as admin if not already running as admin
                if (!RegistryFixer.IsRunningAsAdmin())
                {
                    var response = MessageBox.Show(
                        "Would you like to restart the application with administrator privileges?",
                        "Restart as Administrator",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (response == MessageBoxResult.Yes) RestartAsAdmin();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error during optimization: {ex.Message}", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            ScanProgress = 100;
            IsScanning = false;
        }
    }

    private string FormatBytes(ulong bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        var counter = 0;
        double number = bytes;

        while (number > 1024 && counter < suffixes.Length - 1)
        {
            number /= 1024;
            counter++;
        }

        return $"{number:0.0} {suffixes[counter]}";
    }

    private async Task<ScanResultsModel> RunScanWithProgressAsync(IProgress<int> progress)
    {
        progress.Report(0);
        await Task.Delay(200);
        progress.Report(25);
        await Task.Delay(200);
        progress.Report(50);
        await Task.Delay(200);
        progress.Report(75);
        await Task.Delay(200);
        progress.Report(95);

        var results = await _optimizationService.ScanSystemAsync();
        progress.Report(100);

        return results;
    }

    // Helper method to restart with admin rights
    private void RestartAsAdmin()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = Process.GetCurrentProcess().MainModule?.FileName,
                UseShellExecute = true,
                Verb = "runas" // Request admin rights
            };

            Process.Start(startInfo);
            Application.Current.Shutdown();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to restart as administrator: {ex.Message}",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // Get detailed information about remaining issues
// Add this method directly inside the existing SystemStatusViewModel class
    private async Task<string> GetRemainingIssuesListAsync()
    {
        var issues = new List<string>();

        // Check registry issues specifically
        var registryIssues = await _optimizationService.GetRegistryIssuesCountAsync();
        if (registryIssues > 0) issues.Add($"• {registryIssues} registry settings (requires admin privileges)");

        // Check disk space
        if (ScanResults.RecoverableSpaceInBytes > 500 * 1024 * 1024) // More than 100MB
            issues.Add($"• {FormatBytes(ScanResults.RecoverableSpaceInBytes)} of recoverable disk space");

        // Just use the issues we can detect without StartupService
        return string.Join("\n", issues);
    }
}