using System.IO;
using System.Reflection;
using System.Text.Json;
using Microsoft.Win32;
using WindowsOptimizer.Models;

namespace WindowsOptimizer.Services;

public class SettingsService
{
    private const string SettingsFileName = "appsettings.json";

    public SettingsService()
    {
        // Initialize with default settings
        CurrentSettings = new SettingsModel
        {
            LaunchAtStartup = false,
            SilentMode = false,
            ScheduledScans = false,
            ScanFrequency = ScanFrequency.Weekly
        };

        // Try to load existing settings
        LoadSettingsAsync().Wait();
    }

    private string SettingsFilePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "QuantumTune",
        SettingsFileName);

    public SettingsModel CurrentSettings { get; private set; }

    public async Task<SettingsModel> LoadSettingsAsync()
    {
        try
        {
            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsFilePath));

            if (File.Exists(SettingsFilePath))
            {
                using var stream = File.OpenRead(SettingsFilePath);
                var loadedSettings = await JsonSerializer.DeserializeAsync<SettingsModel>(stream);

                if (loadedSettings != null) CurrentSettings = loadedSettings;
            }
        }
        catch (Exception ex)
        {
            // Log the error or handle it as appropriate for your application
            Console.WriteLine($"Error loading settings: {ex.Message}");
        }

        return CurrentSettings;
    }

    public async Task SaveSettingsAsync(SettingsModel settings)
    {
        try
        {
            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsFilePath));

            using var stream = File.Create(SettingsFilePath);
            await JsonSerializer.SerializeAsync(stream, settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            // Update the current settings
            CurrentSettings = settings;

            // Apply system-level settings
            ApplySystemSettings(settings);
        }
        catch (Exception ex)
        {
            // Log the error or handle it as appropriate for your application
            Console.WriteLine($"Error saving settings: {ex.Message}");
        }
    }

    private void ApplySystemSettings(SettingsModel settings)
    {
        // Example of applying system-level settings
        if (settings.LaunchAtStartup)
            // Add application to Windows startup (using Registry or other method)
            AddToStartup();
        else
            // Remove application from Windows startup
            RemoveFromStartup();

        // Additional system-level setting applications can be added here
    }

    private void AddToStartup()
    {
        try
        {
            // Example implementation for adding to Windows startup
            using var key = Registry.CurrentUser.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            key?.SetValue("QuantumTuneMax",
                Assembly.GetEntryAssembly()?.Location);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding to startup: {ex.Message}");
        }
    }

    private void RemoveFromStartup()
    {
        try
        {
            // Example implementation for removing from Windows startup
            using var key = Registry.CurrentUser.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            key?.DeleteValue("QuantumTuneMax", false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing from startup: {ex.Message}");
        }
    }
}