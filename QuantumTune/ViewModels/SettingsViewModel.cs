using System.Windows.Input;
using WindowsOptimizer.Helpers;
using WindowsOptimizer.Models;
using WindowsOptimizer.Services;

namespace WindowsOptimizer.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly SettingsService _settingsService;
    private bool _isSaving;

    private bool _launchAtStartup;
    private ScanFrequency _scanFrequency;
    private bool _scheduledScans;
    private bool _silentMode;

    public SettingsViewModel(SettingsService settingsService)
    {
        _settingsService = settingsService;

        // Initialize from current settings
        var currentSettings = _settingsService.CurrentSettings;
        _launchAtStartup = currentSettings.LaunchAtStartup;
        _silentMode = currentSettings.SilentMode;
        _scheduledScans = currentSettings.ScheduledScans;
        _scanFrequency = currentSettings.ScanFrequency;

        // Initialize commands
        LoadSettingsCommand = new AsyncRelayCommand(ExecuteLoadSettingsAsync);
        SaveSettingsCommand = new AsyncRelayCommand(ExecuteSaveSettingsAsync);
    }

    public bool LaunchAtStartup
    {
        get => _launchAtStartup;
        set
        {
            if (SetProperty(ref _launchAtStartup, value)) SaveSettingsCommand.Execute(null);
        }
    }

    public bool SilentMode
    {
        get => _silentMode;
        set
        {
            if (SetProperty(ref _silentMode, value)) SaveSettingsCommand.Execute(null);
        }
    }

    public bool ScheduledScans
    {
        get => _scheduledScans;
        set
        {
            if (SetProperty(ref _scheduledScans, value)) SaveSettingsCommand.Execute(null);
        }
    }

    public ScanFrequency ScanFrequency
    {
        get => _scanFrequency;
        set
        {
            if (SetProperty(ref _scanFrequency, value)) SaveSettingsCommand.Execute(null);
        }
    }

    public bool IsSaving
    {
        get => _isSaving;
        private set => SetProperty(ref _isSaving, value);
    }

    public ICommand LoadSettingsCommand { get; }
    public ICommand SaveSettingsCommand { get; }

    private async Task ExecuteLoadSettingsAsync(object parameter)
    {
        var settings = await _settingsService.LoadSettingsAsync();

        _launchAtStartup = settings.LaunchAtStartup;
        _silentMode = settings.SilentMode;
        _scheduledScans = settings.ScheduledScans;
        _scanFrequency = settings.ScanFrequency;

        OnPropertyChanged(nameof(LaunchAtStartup));
        OnPropertyChanged(nameof(SilentMode));
        OnPropertyChanged(nameof(ScheduledScans));
        OnPropertyChanged(nameof(ScanFrequency));
    }

    private async Task ExecuteSaveSettingsAsync(object parameter)
    {
        if (IsSaving) return;

        try
        {
            IsSaving = true;

            var settings = new SettingsModel
            {
                LaunchAtStartup = LaunchAtStartup,
                SilentMode = SilentMode,
                ScheduledScans = ScheduledScans,
                ScanFrequency = ScanFrequency
            };

            await _settingsService.SaveSettingsAsync(settings);
        }
        finally
        {
            IsSaving = false;
        }
    }
}