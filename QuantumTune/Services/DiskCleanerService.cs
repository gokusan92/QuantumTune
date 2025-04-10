using System.IO;
using System.Runtime.InteropServices;
using WindowsOptimizer.Models;

namespace WindowsOptimizer.Services;

public class DiskCleanerService
{
    private const uint SHERB_NOCONFIRMATION = 0x00000001;
    private const uint SHERB_NOPROGRESSUI = 0x00000002;
    private const uint SHERB_NOSOUND = 0x00000004;

    private readonly Dictionary<string, List<string>> _cleanupLocations = new()
    {
        {
            "Temporary Files", new List<string>
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp"),
                Path.Combine(Path.GetTempPath())
            }
        },
        {
            "Windows Update Cleanup", new List<string>
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SoftwareDistribution",
                    "Download")
            }
        },
        {
            "Recycle Bin", new List<string>
            {
                // Recycle bin requires special handling - placeholder for now
            }
        },
        {
            "Browser Cache", new List<string>
            {
                // Chrome
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Google", "Chrome", "User Data", "Default", "Cache"),
                // Edge
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Microsoft", "Edge", "User Data", "Default", "Cache"),
                // Firefox
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Mozilla", "Firefox", "Profiles")
            }
        }
    };

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern int SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, uint dwFlags);

    public async Task<Dictionary<string, CleanupInfoModel>> GetCleanupInfoAsync()
    {
        return await Task.Run(() =>
        {
            var result = new Dictionary<string, CleanupInfoModel>();

            foreach (var location in _cleanupLocations)
            {
                var cleanupInfo = new CleanupInfoModel
                {
                    Name = location.Key,
                    Paths = location.Value,
                    BytesRecoverable = 0,
                    FilesCount = 0
                };

                if (location.Key == "Recycle Bin")
                    // Special handling for Recycle Bin
                    try
                    {
                        var recBinSize = GetRecycleBinSize();
                        cleanupInfo.BytesRecoverable = recBinSize;
                        cleanupInfo.FilesCount = -1; // We don't count files for Recycle Bin
                    }
                    catch
                    {
                        cleanupInfo.BytesRecoverable = 0;
                    }
                else
                    // Standard folder cleanup info
                    foreach (var path in location.Value)
                        try
                        {
                            if (Directory.Exists(path))
                            {
                                var dirInfo = new DirectoryInfo(path);
                                var files = dirInfo.GetFiles("*", SearchOption.AllDirectories);
                                cleanupInfo.BytesRecoverable += (ulong)files.Sum(f => f.Length);
                                cleanupInfo.FilesCount += files.Length;
                            }
                        }
                        catch
                        {
                            // Continue if we can't access a directory
                        }

                result.Add(location.Key, cleanupInfo);
            }

            return result;
        });
    }

    public async Task<CleanupResultModel> CleanupAsync(Dictionary<string, bool> locationsToClean)
    {
        return await Task.Run(() =>
        {
            var result = new CleanupResultModel
            {
                TotalBytesFreed = 0,
                TotalFilesRemoved = 0,
                LocationResults = new Dictionary<string, (ulong BytesFreed, int FilesRemoved)>()
            };

            foreach (var locationPair in locationsToClean)
            {
                if (!locationPair.Value) continue; // Skip if not selected

                var locationName = locationPair.Key;
                ulong bytesFreed = 0;
                var filesRemoved = 0;

                if (locationName == "Recycle Bin")
                    // Special handling for Recycle Bin
                    try
                    {
                        // Get size before emptying
                        var sizeBefore = GetRecycleBinSize();

                        // Use Windows API to empty recycle bin - this is much more effective
                        var apiResult = SHEmptyRecycleBin(IntPtr.Zero, null,
                            SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND);

                        // If API call succeeded
                        if (apiResult == 0)
                        {
                            bytesFreed = sizeBefore;
                            filesRemoved = 1; // Just indicate something was removed
                        }
                        else
                        {
                            // Fall back to manual method if API fails
                            EmptyRecycleBin();
                            bytesFreed = sizeBefore;
                            filesRemoved = 1;
                        }
                    }
                    catch
                    {
                        // If all else fails, try the original method
                        try
                        {
                            var sizeBefore = GetRecycleBinSize();
                            EmptyRecycleBin();
                            bytesFreed = sizeBefore;
                            filesRemoved = 1;
                        }
                        catch
                        {
                            // Continue if we can't empty the recycle bin
                        }
                    }
                else if (_cleanupLocations.TryGetValue(locationName, out var paths))
                    foreach (var path in paths)
                        try
                        {
                            if (Directory.Exists(path))
                            {
                                var dirInfo = new DirectoryInfo(path);
                                var files = dirInfo.GetFiles("*", SearchOption.AllDirectories);

                                foreach (var file in files)
                                    try
                                    {
                                        // Check if file is in use and try to make it deletable
                                        var isReadOnly = (file.Attributes & FileAttributes.ReadOnly) ==
                                                         FileAttributes.ReadOnly;

                                        if (isReadOnly)
                                            // Clear the read-only flag
                                            file.Attributes &= ~FileAttributes.ReadOnly;

                                        var fileSize = file.Length;
                                        file.Delete();
                                        bytesFreed += (ulong)fileSize;
                                        filesRemoved++;
                                    }
                                    catch
                                    {
                                        // If file is in use, try an alternative approach
                                        try
                                        {
                                            // For locked files, we'll try a different approach with streams
                                            var tempPath = file.FullName + ".tmp";
                                            if (File.Exists(tempPath))
                                                File.Delete(tempPath);

                                            File.Move(file.FullName, tempPath);
                                            File.Delete(tempPath);

                                            bytesFreed += (ulong)file.Length;
                                            filesRemoved++;
                                        }
                                        catch
                                        {
                                            // Skip files we still can't delete
                                        }
                                    }

                                // Try to delete empty subdirectories
                                try
                                {
                                    foreach (var dir in dirInfo.GetDirectories("*", SearchOption.AllDirectories)
                                                 .OrderByDescending(d => d.FullName.Length))
                                        try
                                        {
                                            // Check if directory is empty
                                            if (!dir.GetFiles("*", SearchOption.AllDirectories).Any() &&
                                                !dir.GetDirectories().Any())
                                                dir.Delete(false); // Don't recursively delete
                                        }
                                        catch
                                        {
                                            // Skip directories we can't delete
                                        }
                                }
                                catch
                                {
                                    // Skip if we can't process directories
                                }
                            }
                        }
                        catch
                        {
                            // Continue if we can't access a directory
                        }

                result.LocationResults[locationName] = (bytesFreed, filesRemoved);
                result.TotalBytesFreed += bytesFreed;
                result.TotalFilesRemoved += filesRemoved;
            }

            return result;
        });
    }

    private ulong GetRecycleBinSize()
    {
        ulong size = 0;

        // This is a simplified implementation
        // In a real application, you would use the Shell32 API
        var drives = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed);

        foreach (var drive in drives)
        {
            var recycleBinFolder = Path.Combine(drive.Name, "$Recycle.Bin");
            if (Directory.Exists(recycleBinFolder))
                try
                {
                    foreach (var dir in Directory.GetDirectories(recycleBinFolder))
                        try
                        {
                            var dirInfo = new DirectoryInfo(dir);
                            size += (ulong)dirInfo.GetFiles("*", SearchOption.AllDirectories)
                                .Sum(f => f.Length);
                        }
                        catch
                        {
                            // Continue if we can't access a subdirectory
                        }
                }
                catch
                {
                    // Continue if we can't access the $Recycle.Bin folder
                }
        }

        return size;
    }

    private void EmptyRecycleBin()
    {
        // Improved with multiple methods to try
        try
        {
            // First try the Shell32 API
            SHEmptyRecycleBin(IntPtr.Zero, null, SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND);
        }
        catch
        {
            // If that fails, try the original manual method
            var drives = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed);

            foreach (var drive in drives)
            {
                var recycleBinFolder = Path.Combine(drive.Name, "$Recycle.Bin");
                if (Directory.Exists(recycleBinFolder))
                    try
                    {
                        foreach (var dir in Directory.GetDirectories(recycleBinFolder))
                            try
                            {
                                Directory.Delete(dir, true);
                            }
                            catch
                            {
                                // Continue if we can't delete a subdirectory
                            }
                    }
                    catch
                    {
                        // Continue if we can't access the $Recycle.Bin folder
                    }
            }
        }
    }
}