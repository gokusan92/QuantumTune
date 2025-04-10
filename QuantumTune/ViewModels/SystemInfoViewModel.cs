using System.Windows.Input;
using WindowsOptimizer.Helpers;
using WindowsOptimizer.Models;
using WindowsOptimizer.Services;

namespace WindowsOptimizer.ViewModels;

public class SystemInfoViewModel : ViewModelBase, IPageViewModel
{
    private readonly SystemService _systemService;
    private bool _isLoading;

    private SystemInfoModel _systemInfo;

    public SystemInfoViewModel(SystemService systemService)
    {
        _systemService = systemService;

        _systemInfo = new SystemInfoModel();
        _isLoading = false;

        LoadSystemInfoCommand = new AsyncRelayCommand(ExecuteLoadSystemInfoAsync);
    }

    public SystemInfoModel SystemInfo
    {
        get => _systemInfo;
        private set => SetProperty(ref _systemInfo, value);
    }

    public ICommand LoadSystemInfoCommand { get; }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public async void OnNavigatedTo()
    {
        await ExecuteLoadSystemInfoAsync(null);
    }

    public async Task LoadAsync()
    {
        await ExecuteLoadSystemInfoAsync(null);
    }

    private async Task ExecuteLoadSystemInfoAsync(object parameter)
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;
            SystemInfo = await _systemService.GetSystemInfoAsync();
        }
        catch (Exception ex)
        {
            // Log or handle error as needed
        }
        finally
        {
            IsLoading = false;
        }
    }
}