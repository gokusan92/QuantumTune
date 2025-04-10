using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using WindowsOptimizer.Models;

namespace WindowsOptimizer.Services;

public class StartupService
{
    private readonly string _allUsersStartupPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup));

    private readonly string _currentUserStartupPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Startup));

    private readonly string[] _essentialItems =
    {
        "Windows Security",
        "Windows Defender",
        "Microsoft Security Client",
        "Explorer",
        "Shell Hardware Detection",
        "Security Center"
    };

    public async Task<List<StartupItemModel>> GetStartupItemsAsync()
    {
        return await Task.Run(() =>
        {
            var startupItems = new List<StartupItemModel>();

            // Get items from registry - Current User Run
            using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false))
            {
                if (key != null)
                    foreach (var name in key.GetValueNames())
                    {
                        var command = key.GetValue(name)?.ToString();
                        startupItems.Add(new StartupItemModel
                        {
                            Name = name,
                            Command = command,
                            Location = "HKCU\\Run",
                            IsEnabled = true,
                            IsEssential = IsEssentialItem(name)
                        });
                    }
            }

            // Get items from registry - Local Machine Run
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false))
            {
                if (key != null)
                    foreach (var name in key.GetValueNames())
                    {
                        var command = key.GetValue(name)?.ToString();
                        startupItems.Add(new StartupItemModel
                        {
                            Name = name,
                            Command = command,
                            Location = "HKLM\\Run",
                            IsEnabled = true,
                            IsEssential = IsEssentialItem(name)
                        });
                    }
            }

            // Get disabled items
            using (var key = Registry.LocalMachine.OpenSubKey(
                       @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", false))
            {
                if (key != null)
                    foreach (var name in key.GetValueNames())
                    {
                        var item = startupItems.FirstOrDefault(i => i.Name == name);
                        if (item != null)
                        {
                            var valueBytes = (byte[])key.GetValue(name);
                            // Check if the item is disabled (first byte is 3 or 2)
                            if (valueBytes != null && valueBytes.Length > 0 &&
                                (valueBytes[0] == 3 || valueBytes[0] == 2)) item.IsEnabled = false;
                        }
                    }
            }

            using (var key = Registry.CurrentUser.OpenSubKey(
                       @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", false))
            {
                if (key != null)
                    foreach (var name in key.GetValueNames())
                    {
                        var item = startupItems.FirstOrDefault(i => i.Name == name);
                        if (item != null)
                        {
                            var valueBytes = (byte[])key.GetValue(name);
                            // Check if the item is disabled
                            if (valueBytes != null && valueBytes.Length > 0 &&
                                (valueBytes[0] == 3 || valueBytes[0] == 2)) item.IsEnabled = false;
                        }
                    }
            }

            // Get items from Startup folders
            GetStartupItemsFromFolder(_currentUserStartupPath, "User Startup Folder", startupItems);
            GetStartupItemsFromFolder(_allUsersStartupPath, "All Users Startup Folder", startupItems);

            // Calculate impact
            foreach (var item in startupItems) item.Impact = CalculateImpact(item.Name, item.Command);

            return startupItems;
        });
    }

    private void GetStartupItemsFromFolder(string folderPath, string location, List<StartupItemModel> items)
    {
        try
        {
            foreach (var file in Directory.GetFiles(folderPath, "*.*")
                         .Where(file => file.EndsWith(".lnk") || file.EndsWith(".url")))
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                items.Add(new StartupItemModel
                {
                    Name = fileName,
                    Command = file,
                    Location = location,
                    IsEnabled = true,
                    IsEssential = IsEssentialItem(fileName)
                });
            }
        }
        catch
        {
            // Folder might not exist or be accessible
        }
    }

    private string CalculateImpact(string name, string command)
    {
        // Let's use a simple heuristic for now - could be expanded with real performance analysis
        if (IsEssentialItem(name)) return "Essential";

        // Large applications typically have a higher impact
        var knownHighImpactApps = new[]
        {
            "steam", "spotify", "adobe", "dropbox", "onedrive", "google"
        };

        if (knownHighImpactApps.Any(app => name.ToLower().Contains(app) ||
                                           (command != null && command.ToLower().Contains(app))))
            return "High Impact";

        // Return medium by default - we could use more sophisticated detection here
        return "Medium Impact";
    }

    public async Task<bool> SetStartupItemEnabledAsync(StartupItemModel item, bool enabled)
    {
        return await Task.Run(() =>
        {
            var success = false;

            try
            {
                if (item.Location == "HKCU\\Run")
                {
                    // For HKCU items, do both - remove from Run and set in StartupApproved
                    if (!enabled)
                    {
                        // First, try to completely remove it from Run
                        using (var runKey =
                               Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                        {
                            if (runKey != null)
                                try
                                {
                                    runKey.DeleteValue(item.Name, false);
                                    success = true;
                                }
                                catch
                                {
                                    // If we can't delete, try to disable it
                                    success = SetRegistryStartupItemEnabled(Registry.CurrentUser, item.Name, false);
                                }
                        }
                    }
                    else
                    {
                        // To enable, add back to Run and clear from StartupApproved
                        using (var runKey =
                               Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                        {
                            if (runKey != null && item.Command != null)
                            {
                                runKey.SetValue(item.Name, item.Command);
                                success = true;
                            }
                        }

                        // Also clear from StartupApproved if present
                        success = SetRegistryStartupItemEnabled(Registry.CurrentUser, item.Name, true);
                    }
                }
                else if (item.Location == "HKLM\\Run")
                {
                    // For HKLM items, do both - remove from Run and set in StartupApproved
                    if (!enabled)
                    {
                        using (var runKey =
                               Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                        {
                            if (runKey != null)
                                try
                                {
                                    runKey.DeleteValue(item.Name, false);
                                    success = true;
                                }
                                catch
                                {
                                    // If we can't delete, try to disable it
                                    success = SetRegistryStartupItemEnabled(Registry.LocalMachine, item.Name, false);
                                }
                        }
                    }
                    else
                    {
                        // To enable, add back to Run and clear from StartupApproved
                        using (var runKey =
                               Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                        {
                            if (runKey != null && item.Command != null)
                            {
                                runKey.SetValue(item.Name, item.Command);
                                success = true;
                            }
                        }

                        // Also clear from StartupApproved if present
                        success = SetRegistryStartupItemEnabled(Registry.LocalMachine, item.Name, true);
                    }
                }
                else if (item.Location.Contains("Startup Folder"))
                {
                    // For startup folder items, we need to rename or delete the file
                    if (!enabled && item.Command != null && File.Exists(item.Command))
                    {
                        try
                        {
                            // Create a disabled version by renaming
                            var disabledFile = item.Command + ".disabled";
                            if (File.Exists(disabledFile))
                                File.Delete(disabledFile);

                            File.Move(item.Command, disabledFile);
                            success = true;
                        }
                        catch
                        {
                            // If rename fails, try to delete
                            try
                            {
                                File.Delete(item.Command);
                                success = true;
                            }
                            catch
                            {
                                success = false;
                            }
                        }
                    }
                    else if (enabled && item.Command != null)
                    {
                        // Check for disabled version
                        var disabledFile = item.Command + ".disabled";
                        if (File.Exists(disabledFile))
                            try
                            {
                                if (File.Exists(item.Command))
                                    File.Delete(item.Command);

                                File.Move(disabledFile, item.Command);
                                success = true;
                            }
                            catch
                            {
                                success = false;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting startup item: {ex.Message}");
                success = false;
            }

            return success;
        });
    }

    private bool SetRegistryStartupItemEnabled(RegistryKey hive, string itemName, bool enabled)
    {
        var approvedKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";

        try
        {
            using var approvedKey = hive.OpenSubKey(approvedKeyPath, true);
            if (approvedKey == null)
            {
                // Try to create the key if it doesn't exist
                using var softwareKey = hive.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer", true);
                if (softwareKey != null)
                {
                    var newKey = softwareKey.CreateSubKey("StartupApproved");
                    if (newKey != null)
                    {
                        var runKey = newKey.CreateSubKey("Run");
                        if (runKey != null)
                        {
                            var value = new byte[12];
                            if (!enabled) value[0] = 0x03; // 0x03 disables
                            runKey.SetValue(itemName, value, RegistryValueKind.Binary);
                            return true;
                        }
                    }
                }

                return false;
            }

            // The flag format is a 12-byte array where first byte determines the state
            // 03 00 00 00 00 00 00 00 00 00 00 00 - Disabled
            // 00 00 00 00 00 00 00 00 00 00 00 00 - Enabled

            var valueData = new byte[12];
            if (!enabled) valueData[0] = 0x03; // 0x03 disables more reliably than 0x02

            approvedKey.SetValue(itemName, valueData, RegistryValueKind.Binary);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error setting startup registry: {ex.Message}");
            return false;
        }
    }

    private bool IsEssentialItem(string name)
    {
        return _essentialItems.Any(item => name.Contains(item, StringComparison.OrdinalIgnoreCase));
    }
}