using System.Windows;
using Microsoft.Win32;
using WindowsOptimizer.Models;
using WindowsOptimizer.Utils;

namespace WindowsOptimizer.Services;

public class OptimizationService
{
    private readonly DiskCleanerService _diskCleanerService;
    private readonly StartupService _startupService;
    private bool _justOptimized = false;

    public OptimizationService(DiskCleanerService diskCleanerService, StartupService startupService)
    {
        _diskCleanerService = diskCleanerService;
        _startupService = startupService;
    }

    public async Task<ScanResultsModel> ScanSystemAsync()
    {
        // Regular scan code for honest reporting
        var results = new ScanResultsModel
        {
            IssuesCount = 0,
            PerformanceScore = 0,
            RecoverableSpaceInBytes = 0
        };

        // Get cleanable disk space
        var cleanupInfo = await _diskCleanerService.GetCleanupInfoAsync();
        foreach (var info in cleanupInfo.Values) results.RecoverableSpaceInBytes += info.BytesRecoverable;

        // Get startup items that could be optimized
        var startupItems = await _startupService.GetStartupItemsAsync();
        var highImpactStartupItems = startupItems.Count(item =>
            item.IsEnabled && !item.IsEssential && (item.Impact == "High Impact" || item.Impact == "Medium Impact"));

        // Get actual registry issues - no more hardcoded "2"
        var registryIssues = await GetRegistryIssuesCountAsync();

        // Calculate total issues
        results.IssuesCount = (cleanupInfo.Count > 0 && results.RecoverableSpaceInBytes > 1024 * 1024 * 512 ? 1 : 0) +
                              (highImpactStartupItems > 0 ? 1 : 0) +
                              (registryIssues > 0 ? 1 : 0);

        // Calculate performance score
        results.PerformanceScore = CalculatePerformanceScore(
            results.RecoverableSpaceInBytes,
            highImpactStartupItems,
            registryIssues);

        return results;
    }

    public async Task<OptimizationResultModel> OptimizeSystemAsync()
    {
        var result = new OptimizationResultModel
        {
            Success = true,
            IssuesFixed = 0,
            SpaceFreed = 0,
            RegistryFixSuccess = false
        };

        try
        {
            // 1. Clean up disk space
            var locationsToClean = new Dictionary<string, bool>
            {
                { "Temporary Files", true },
                { "Windows Update Cleanup", true },
                { "Recycle Bin", true },
                { "Browser Cache", true }
            };

            var cleanupResult = await _diskCleanerService.CleanupAsync(locationsToClean);
            result.SpaceFreed = cleanupResult.TotalBytesFreed;

            if (cleanupResult.TotalBytesFreed > 0) result.IssuesFixed++;

            // 2. Optimize startup items
            var startupItems = await _startupService.GetStartupItemsAsync();
            var disabledItems = 0;

            foreach (var item in startupItems.Where(item => item.IsEnabled && !item.IsEssential &&
                                                            (item.Impact == "High Impact" ||
                                                             item.Impact == "Medium Impact")))
            {
                var success = await _startupService.SetStartupItemEnabledAsync(item, false);
                if (success) disabledItems++;
            }

            if (disabledItems > 0) result.IssuesFixed++;

            // 3. Apply registry fixes with real validation
            var registrySuccess = RegistryFixer.ApplyRegistryFixes();

            // Verify registry changes with an explicit validation step
            var registryChangesVerified = await VerifyRegistryChangesAsync();

            if (registrySuccess && registryChangesVerified)
            {
                result.IssuesFixed++;
                result.RegistryFixSuccess = true;
            }
            else
            {
                result.RegistryFixSuccess = false;
            }

            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            return result;
        }
    }

    public async Task<bool> VerifyRegistryChangesAsync()
    {
        return await Task.Run(() =>
        {
            var validatedChanges = 0;
            var totalChanges = 5; // Number of registry keys we're checking

            // Check Visual effects settings
            using (var desktopKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", false))
            {
                if (desktopKey != null)
                {
                    var menuShowDelay = desktopKey.GetValue("MenuShowDelay")?.ToString();
                    if (menuShowDelay == "0") validatedChanges++;
                }
            }

            // Check Windows Explorer settings
            using (var explorerKey = Registry.CurrentUser.OpenSubKey(
                       @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", false))
            {
                if (explorerKey != null)
                {
                    var launchTo = explorerKey.GetValue("LaunchTo");
                    if (launchTo != null && (int)launchTo == 1) validatedChanges++;
                }
            }

            // Check Startup delay
            using (var serializeKey = Registry.CurrentUser.OpenSubKey(
                       @"Software\Microsoft\Windows\CurrentVersion\Explorer\Serialize", false))
            {
                if (serializeKey != null &&
                    serializeKey.GetValue("StartupDelayInMSec") != null &&
                    (int)serializeKey.GetValue("StartupDelayInMSec") == 0)
                    validatedChanges++;
            }

            // Check Network throttling
            using (var multimediaKey = Registry.LocalMachine.OpenSubKey(
                       @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", false))
            {
                if (multimediaKey != null)
                {
                    var networkThrottling = multimediaKey.GetValue("NetworkThrottlingIndex");
                    if (networkThrottling != null && (uint)(int)networkThrottling == 4294967295)
                        validatedChanges++;
                }
            }

            // Check Performance settings
            using (var priorityKey = Registry.LocalMachine.OpenSubKey(
                       @"SYSTEM\CurrentControlSet\Control\PriorityControl", false))
            {
                if (priorityKey != null)
                {
                    var prioritySeparation = priorityKey.GetValue("Win32PrioritySeparation");
                    if (prioritySeparation != null && (int)prioritySeparation == 38)
                        validatedChanges++;
                }
            }

            // Return true only if ALL changes were validated
            return validatedChanges == totalChanges;
        });
    }

    public async Task<int> GetRegistryIssuesCountAsync()
    {
        return await Task.Run(() =>
        {
            var issues = 0;

            // Real registry checking that properly checks the values
            try
            {
                // 1. Visual effects settings check
                using var desktopKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", false);
                if (desktopKey != null)
                {
                    var menuShowDelay = desktopKey.GetValue("MenuShowDelay")?.ToString();
                    if (menuShowDelay != "0") issues++;
                }
                else
                {
                    MessageBox.Show("Issue increased.1");
                    issues++;
                }

                // 2. Windows Explorer settings check
                using var explorerKey =
                    Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                        false);
                if (explorerKey != null)
                {
                    var launchTo = explorerKey.GetValue("LaunchTo")?.ToString();
                    if (launchTo is not "1") // Convert to int for exact comparison
                        issues++;
                }
                else
                {
                    issues++;
                }

                // 3. Startup delay check
                using var serializeKey =
                    Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Serialize",
                        false);
                if (serializeKey == null || serializeKey.GetValue("StartupDelayInMSec") == null ||
                    (int)serializeKey.GetValue("StartupDelayInMSec") != 0)
                    issues++;

                // 4. Network throttling check
                using var multimediaKey =
                    Registry.LocalMachine.OpenSubKey(
                        @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", false);
                if (multimediaKey != null)
                {
                    var networkThrottling = multimediaKey.GetValue("NetworkThrottlingIndex");
                    if (networkThrottling == null ||
                        (uint)(int)networkThrottling != 4294967295) // Use uint to handle large value
                        issues++;
                }
                else
                {
                    issues++;
                }

                // 5. Performance settings
                using var priorityKey =
                    Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\PriorityControl", false);
                if (priorityKey != null)
                {
                    var prioritySeparation = priorityKey.GetValue("Win32PrioritySeparation");
                    if (prioritySeparation == null || (int)prioritySeparation != 38) issues++;
                }
                else
                {
                    issues++;
                }
            }
            catch
            {
                // If we can't access the registry, report an issue
                issues = 5; // All issues
            }

            return issues;
        });
    }

    private static int CalculatePerformanceScore(double recoverySpace, int startupIssues, int registryIssues)
    {
        // Start with a good baseline
        var baseScore = 100;

        switch (recoverySpace)
        {
            // Deduct for disk space issues using more robust comparisons
            // > 10 GB
            case > 10_737_418_240.0:
                baseScore -= 15;
                break;
            // > 5 GB
            case > 5_368_709_120.0:
                baseScore -= 10;
                break;
            // > 1 GB
            case > 1_073_741_824.0:
                baseScore -= 5;
                break;
        }

        // Deduct for startup issues
        baseScore -= startupIssues * 2;

        // Deduct for registry issues
        baseScore -= registryIssues * 3;

        // Ensure the score is between 0 and 100
        return Math.Max(0, Math.Min(baseScore, 100));
    }
}