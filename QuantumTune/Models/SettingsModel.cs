namespace WindowsOptimizer.Models;

public enum ScanFrequency
{
    Daily,
    Weekly,
    Monthly
}

public class SettingsModel
{
    public bool LaunchAtStartup { get; set; }
    public bool SilentMode { get; set; }
    public bool ScheduledScans { get; set; }
    public ScanFrequency ScanFrequency { get; set; }
}