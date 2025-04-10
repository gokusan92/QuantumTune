using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using WindowsOptimizer.Services;

namespace WindowsOptimizer.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly DiskCleanerService _diskCleanerService;

    private readonly OptimizationService _optimizationService;

    private readonly IServiceProvider _services;
    private readonly SettingsService _settingsService;
    private readonly StartupService _startupService;
    private readonly SystemService _systemService;
    private object _currentPage;


    public MainViewModel(IServiceProvider services)
    {
        _services = services;
        CurrentPage = _services.GetRequiredService<SystemStatusViewModel>();
        NavigateCommand = new RelayCommand<string>(Navigate);
    }

    public bool IsPageLoading => (CurrentPage as IPageViewModel)?.IsLoading ?? false;

    public object CurrentPage
    {
        get => _currentPage;
        set
        {
            if (_currentPage != value)
            {
                if (_currentPage is IPageViewModel oldPage)
                    oldPage.PropertyChanged -= OnCurrentPagePropertyChanged;

                _currentPage = value;
                OnPropertyChanged();

                if (_currentPage is IPageViewModel newPage)
                {
                    newPage.PropertyChanged += OnCurrentPagePropertyChanged;
                    newPage.OnNavigatedTo();
                }

                OnPropertyChanged(nameof(IsPageLoading)); // notify manually
            }
        }
    }

    public IRelayCommand<string> NavigateCommand { get; }


    private void OnCurrentPagePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IPageViewModel.IsLoading)) OnPropertyChanged(nameof(IsPageLoading));
    }

    private void Navigate(string pageName)
    {
        // Always create a new instance to ensure fresh data loading
        CurrentPage = pageName switch
        {
            "SystemStatusPage" => _services.GetRequiredService<SystemStatusViewModel>(),
            "SystemInfoPage" => _services.GetRequiredService<SystemInfoViewModel>(),
            "StartupItemsPage" => _services.GetRequiredService<StartupItemsViewModel>(),
            "DiskCleanerPage" => _services.GetRequiredService<DiskCleanerViewModel>(),
            "SettingsPage" => _services.GetRequiredService<SettingsViewModel>(),
            "SupportPage" => _services.GetRequiredService<SupportViewModel>(),
            _ => CurrentPage
        };
    }

    public async void OnNavigatedTo()
    {
        await LoadAsync();
    }


    public virtual async Task LoadAsync()
    {
        // Override this in each VM to load stuff
        await Task.CompletedTask;
    }
}