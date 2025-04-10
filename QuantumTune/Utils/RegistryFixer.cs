using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace WindowsOptimizer.Utils;

public static class RegistryFixer
{
    public static bool ApplyRegistryFixes()
    {
        try
        {
            // First try direct Registry API for user-level keys
            var userKeysSuccess = ApplyUserRegistryFixes();

            // For system-level keys, use elevated permissions
            var systemKeysSuccess = ApplySystemRegistryFixes();

            return userKeysSuccess && systemKeysSuccess;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Registry fix error: {ex.Message}");
            return false;
        }
    }

    private static bool ApplyUserRegistryFixes()
    {
        try
        {
            // User level keys (HKCU) - these don't need elevation
            using (var desktopKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
            {
                if (desktopKey != null)
                {
                    desktopKey.SetValue("MenuShowDelay", "0", RegistryValueKind.String);
                    desktopKey.SetValue("DragFullWindows", "0", RegistryValueKind.String);
                }
            }

            using (var serializeKey = Registry.CurrentUser.CreateSubKey(
                       @"Software\Microsoft\Windows\CurrentVersion\Explorer\Serialize", true))
            {
                serializeKey.SetValue("StartupDelayInMSec", 0, RegistryValueKind.DWord);
            }

            using (var explorerKey = Registry.CurrentUser.OpenSubKey(
                       @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true))
            {
                if (explorerKey == null) return true;
                explorerKey.SetValue("LaunchTo", 1, RegistryValueKind.DWord);
                explorerKey.SetValue("ShowInfoTip", 0, RegistryValueKind.DWord);
                explorerKey.SetValue("Start_TrackProgs", 0, RegistryValueKind.DWord);
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"User registry fix error: {ex.Message}");
            return false;
        }
    }

    private static bool ApplySystemRegistryFixes()
    {
        try
        {
            // For system-level keys, we'll use an elevated process
            var needsElevation = true;

            // First try direct access - might work if app is already elevated
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(
                           @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", true))
                {
                    if (key != null)
                    {
                        key.SetValue("NetworkThrottlingIndex", 4294967295, RegistryValueKind.DWord);
                        key.SetValue("SystemResponsiveness", 10, RegistryValueKind.DWord);
                        needsElevation = false; // Direct access worked
                    }
                }

                using (var key = Registry.LocalMachine.OpenSubKey(
                           @"SYSTEM\CurrentControlSet\Control\PriorityControl", true))
                {
                    key?.SetValue("Win32PrioritySeparation", 38, RegistryValueKind.DWord);
                }
            }
            catch (UnauthorizedAccessException)
            {
                needsElevation = true;
            }

            if (!needsElevation) return true;
            // We need admin rights - use an elevated process
            var tempFile = Path.Combine(Path.GetTempPath(), "SystemOptimizer_RegFix.reg");
            File.WriteAllText(tempFile, @"Windows Registry Editor Version 5.00

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile]
""NetworkThrottlingIndex""=dword:ffffffff
""SystemResponsiveness""=dword:0000000a

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\PriorityControl]
""Win32PrioritySeparation""=dword:00000026
");

            // Use ProcessStartInfo to request elevation
            var startInfo = new ProcessStartInfo
            {
                FileName = "regedit.exe",
                Arguments = $"/s \"{tempFile}\"",
                UseShellExecute = true,
                Verb = "runas", // Request admin rights
                CreateNoWindow = true
            };

            var process = Process.Start(startInfo);
            process?.WaitForExit(5000); // Wait up to 5 seconds

            try
            {
                File.Delete(tempFile);
            }
            catch
            {
                /* Ignore cleanup errors */
            }

            // Now verify the changes were applied
            return VerifySystemRegistryChanges();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"System registry fix error: {ex.Message}");
            return false;
        }
    }

    public static string DiagnoseSystemRegistryIssues()
    {
        var issues = new List<string>();

        try
        {
            // Check multimedia profile
            using (var key = Registry.LocalMachine.OpenSubKey(
                       @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", false))
            {
                if (key == null)
                {
                    issues.Add("Multimedia profile key doesn't exist");
                }
                else
                {
                    var throttling = key.GetValue("NetworkThrottlingIndex");
                    var responsiveness = key.GetValue("SystemResponsiveness");

                    if (throttling == null || (uint)(int)throttling != 4294967295)
                        issues.Add($"NetworkThrottlingIndex value: {throttling}");

                    if (responsiveness == null || (int)responsiveness != 10)
                        issues.Add($"SystemResponsiveness value: {responsiveness}");
                }
            }

            // Check priority control
            using (var key = Registry.LocalMachine.OpenSubKey(
                       @"SYSTEM\CurrentControlSet\Control\PriorityControl", false))
            {
                if (key == null)
                {
                    issues.Add("PriorityControl key doesn't exist");
                }
                else
                {
                    var priority = key.GetValue("Win32PrioritySeparation");

                    if (priority == null || (int)priority != 38)
                        issues.Add($"Win32PrioritySeparation value: {priority}");
                }
            }

            return issues.Count > 0 ? string.Join(", ", issues) : "No registry issues found";
        }
        catch (Exception ex)
        {
            return $"Error diagnosing registry: {ex.Message}";
        }
    }

    private static bool VerifySystemRegistryChanges()
    {
        try
        {
            var allChangesApplied = true;

            // Check multimedia profile
            using (var key = Registry.LocalMachine.OpenSubKey(
                       @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", false))
            {
                if (key == null)
                {
                    Debug.WriteLine("ISSUE: Multimedia profile key doesn't exist");
                    return false;
                }

                var throttling = key.GetValue("NetworkThrottlingIndex");
                var responsiveness = key.GetValue("SystemResponsiveness");

                if (throttling == null || (uint)(int)throttling != 4294967295)
                {
                    Debug.WriteLine($"ISSUE: NetworkThrottlingIndex value incorrect: {throttling}");
                    allChangesApplied = false;
                }

                if (responsiveness == null || (int)responsiveness != 10)
                {
                    Debug.WriteLine($"ISSUE: SystemResponsiveness value incorrect: {responsiveness}");
                    allChangesApplied = false;
                }
            }

            // Check priority control
            using (var key = Registry.LocalMachine.OpenSubKey(
                       @"SYSTEM\CurrentControlSet\Control\PriorityControl", false))
            {
                if (key == null)
                {
                    Debug.WriteLine("ISSUE: PriorityControl key doesn't exist");
                    return false;
                }

                var priority = key.GetValue("Win32PrioritySeparation");

                if (priority == null || (int)priority != 38)
                {
                    Debug.WriteLine($"ISSUE: Win32PrioritySeparation value incorrect: {priority}");
                    allChangesApplied = false;
                }
            }

            return allChangesApplied;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ISSUE: Registry verification exception: {ex.Message}");
            return false;
        }
    }

    // Add a public method to check if we have admin privileges
    public static bool IsRunningAsAdmin()
    {
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE", true);
            return key != null; // If we can write to HKLM, we're admin
        }
        catch
        {
            return false;
        }
    }
}