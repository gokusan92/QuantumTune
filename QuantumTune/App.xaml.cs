using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WindowsOptimizer.Services;
using WindowsOptimizer.ViewModels;

namespace WindowsOptimizer;

public partial class App : Application
{
    private readonly ServiceProvider _serviceProvider;

    public App()
    {
        InitializeComponent();

        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(ServiceCollection services)
    {
        // Register services
        services.AddSingleton<DiskCleanerService>();
        services.AddSingleton<StartupService>();
        services.AddSingleton<SettingsService>();
        services.AddSingleton<SystemService>();
        services.AddSingleton<OptimizationService>();

        // Register viewmodels
        // Update the SystemStatusViewModel registration to include StartupService
        services.AddSingleton<SystemStatusViewModel>(provider => new SystemStatusViewModel(
            provider.GetRequiredService<OptimizationService>(),
            provider.GetRequiredService<StartupService>()));

        services.AddSingleton<SystemInfoViewModel>();
        services.AddSingleton<StartupItemsViewModel>();
        services.AddSingleton<DiskCleanerViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<SupportViewModel>();
        services.AddSingleton<MainViewModel>();

        // Register main window
        services.AddSingleton<MainWindow>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = _serviceProvider.GetService<MainWindow>();
        mainWindow?.Show();
    }
}