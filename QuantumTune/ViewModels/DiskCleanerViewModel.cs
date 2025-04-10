using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using WindowsOptimizer.Helpers;
using WindowsOptimizer.Services;

namespace WindowsOptimizer.ViewModels;

public class DiskCleanerViewModel : ViewModelBase, IPageViewModel
{
    private readonly DiskCleanerService _diskCleanerService;
    private ObservableCollection<CleanupItemViewModel> _cleanupItems;
    private bool _isCleaning;
    private bool _isLoading;
    private string _lastCleanupText;
    private string _recoverableSpace;
    private string _totalDiskSpace;
    private double _usedSpacePercent;

    public DiskCleanerViewModel(DiskCleanerService diskCleanerService)
    {
        _diskCleanerService = diskCleanerService;

        // Initialize properties
        _cleanupItems = new ObservableCollection<CleanupItemViewModel>();
        _isLoading = false;
        _isCleaning = false;
        _lastCleanupText = "Last cleanup: Never";
        _usedSpacePercent = 0;
        _totalDiskSpace = "0 GB";
        _recoverableSpace = "0 GB";

        // Initialize commands
        LoadCleanupInfoCommand = new AsyncRelayCommand(ExecuteLoadCleanupInfoAsync);
        CleanupCommand = new AsyncRelayCommand(ExecuteCleanupAsync);
    }

    public ObservableCollection<CleanupItemViewModel> CleanupItems
    {
        get => _cleanupItems;
        private set => SetProperty(ref _cleanupItems, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool IsCleaning
    {
        get => _isCleaning;
        private set => SetProperty(ref _isCleaning, value);
    }

    public string LastCleanupText
    {
        get => _lastCleanupText;
        private set => SetProperty(ref _lastCleanupText, value);
    }

    public double UsedSpacePercent
    {
        get => _usedSpacePercent;
        private set => SetProperty(ref _usedSpacePercent, value);
    }

    public string TotalDiskSpace
    {
        get => _totalDiskSpace;
        private set => SetProperty(ref _totalDiskSpace, value);
    }

    public string RecoverableSpace
    {
        get => _recoverableSpace;
        private set => SetProperty(ref _recoverableSpace, value);
    }

    public ICommand LoadCleanupInfoCommand { get; }
    public ICommand CleanupCommand { get; }

    public async void OnNavigatedTo()
    {
        await LoadAsync();
    }

    public async Task LoadAsync()
    {
        await ExecuteLoadCleanupInfoAsync(null);
    }

    private async Task ExecuteLoadCleanupInfoAsync(object parameter)
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;

            // Load cleanup info
            var cleanupInfo = await _diskCleanerService.GetCleanupInfoAsync();

            // Create view models for each cleanup item
            var items = cleanupInfo.Select(kvp => new CleanupItemViewModel
            {
                Name = kvp.Key,
                Description = GetDescriptionForCleanupItem(kvp.Key),
                Size = FormatBytes(kvp.Value.BytesRecoverable),
                IsSelected = true,
                SizeInBytes = kvp.Value.BytesRecoverable
            }).ToList();

            CleanupItems = new ObservableCollection<CleanupItemViewModel>(items);

            // Calculate total recoverable space
            var totalRecoverable = cleanupInfo.Values.Aggregate(0UL, (acc, x) => acc + x.BytesRecoverable);

            RecoverableSpace = FormatBytes(totalRecoverable);

            // Get drive space info (using the C: drive)
            UpdateDriveInfo();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ExecuteCleanupAsync(object parameter)
    {
        if (IsCleaning) return;

        try
        {
            IsCleaning = true;

            // Create dictionary of locations to clean
            var locationsToClean = CleanupItems.ToDictionary(
                item => item.Name,
                item => item.IsSelected);

            // Perform cleanup
            var result = await _diskCleanerService.CleanupAsync(locationsToClean);

            // Update the UI
            LastCleanupText = "Last cleanup: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm");

            // Refresh cleanup info
            await ExecuteLoadCleanupInfoAsync(null);

            // Update drive info
            UpdateDriveInfo();
        }
        finally
        {
            IsCleaning = false;
        }
    }

    private void UpdateDriveInfo()
    {
        try
        {
            var systemDrive = new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory));

            double totalSize = systemDrive.TotalSize;
            double freeSpace = systemDrive.AvailableFreeSpace;
            var usedSpace = totalSize - freeSpace;

            TotalDiskSpace = $"{FormatBytes((ulong)totalSize)}";
            UsedSpacePercent = Math.Round(usedSpace / totalSize * 100);
        }
        catch
        {
            TotalDiskSpace = "Unknown";
            UsedSpacePercent = 0;
        }
    }

    private string GetDescriptionForCleanupItem(string name)
    {
        return name switch
        {
            "Temporary Files" => "Windows and application temporary files",
            "Windows Update Cleanup" => "Old Windows update files",
            "Recycle Bin" => "Files in the Recycle Bin",
            "Browser Cache" => "Cached browser files and images",
            _ => "System files that can be safely removed"
        };
    }

    private string FormatBytes(ulong bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        var counter = 0;
        double number = bytes;

        while (number >= 1024 && counter < suffixes.Length - 1)
        {
            number /= 1024;
            counter++;
        }

        return $"{number:0.0} {suffixes[counter]}";
    }
}