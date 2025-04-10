using System.Management;
using WindowsOptimizer.Models;

namespace WindowsOptimizer.Services;

public class SystemService
{
    public async Task<SystemInfoModel> GetSystemInfoAsync()
    {
        return await Task.Run(() =>
        {
            var systemInfo = new SystemInfoModel();

            // Get CPU information
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                foreach (var obj in searcher.Get())
                {
                    systemInfo.CpuModel = obj["Name"]?.ToString() ?? "Unknown";
                    systemInfo.CpuCores = Convert.ToInt32(obj["NumberOfCores"]);
                    systemInfo.CpuThreads = Convert.ToInt32(obj["NumberOfLogicalProcessors"]);
                    systemInfo.CpuSpeed = $"{Convert.ToDouble(obj["MaxClockSpeed"]) / 1000:F2} GHz";
                    break; // Just get the first CPU
                }
            }
            catch (Exception)
            {
                systemInfo.CpuModel = "Could not retrieve CPU info";
            }

            // Get GPU information
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
                foreach (var obj in searcher.Get())
                {
                    var gpuInfo = new GpuInfo
                    {
                        Name = obj["Name"]?.ToString() ?? "Unknown",
                        DriverVersion = obj["DriverVersion"]?.ToString() ?? "Unknown",
                        VideoMemory = obj["AdapterRAM"] != null
                            ? $"{Convert.ToDouble(obj["AdapterRAM"]) / (1024 * 1024 * 1024):F2} GB"
                            : "Unknown"
                    };
                    systemInfo.GpuList.Add(gpuInfo);
                }
            }
            catch (Exception)
            {
                systemInfo.GpuList.Add(new GpuInfo { Name = "Could not retrieve GPU info" });
            }

            // Get RAM information
            try
            {
                ulong totalMemory = 0;
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
                foreach (var obj in searcher.Get())
                {
                    var capacity = Convert.ToUInt64(obj["Capacity"]);
                    totalMemory += capacity;

                    var memoryInfo = new RamInfo
                    {
                        Manufacturer = obj["Manufacturer"]?.ToString() ?? "Unknown",
                        Capacity = $"{capacity / (1024 * 1024 * 1024):F2} GB",
                        Speed = obj["Speed"] != null ? $"{obj["Speed"]} MHz" : "Unknown"
                    };
                    systemInfo.RamModules.Add(memoryInfo);
                }

                systemInfo.TotalRam = $"{totalMemory / (1024 * 1024 * 1024):F2} GB";
            }
            catch (Exception)
            {
                systemInfo.TotalRam = "Could not retrieve RAM info";
            }

            // Get disk information
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk WHERE DriveType=3");
                foreach (var obj in searcher.Get())
                {
                    var size = Convert.ToUInt64(obj["Size"]);
                    var freeSpace = Convert.ToUInt64(obj["FreeSpace"]);
                    var usedSpace = size - freeSpace;
                    var usedPercent = (double)usedSpace / size * 100;

                    var diskInfo = new DiskInfo
                    {
                        DriveLetter = obj["DeviceID"]?.ToString() ?? "Unknown",
                        TotalSize = $"{size / (1024 * 1024 * 1024):F2} GB",
                        FreeSpace = $"{freeSpace / (1024 * 1024 * 1024):F2} GB",
                        UsedPercent = usedPercent
                    };
                    systemInfo.Disks.Add(diskInfo);
                }
            }
            catch (Exception)
            {
                systemInfo.Disks.Add(new DiskInfo { DriveLetter = "Could not retrieve disk info" });
            }

            // Get OS information
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                foreach (var obj in searcher.Get())
                {
                    systemInfo.OsName = obj["Caption"]?.ToString() ?? "Unknown";
                    systemInfo.OsVersion = obj["Version"]?.ToString() ?? "Unknown";
                    systemInfo.OsBuild = obj["BuildNumber"]?.ToString() ?? "Unknown";
                    systemInfo.OsArchitecture = obj["OSArchitecture"]?.ToString() ?? "Unknown";
                    break;
                }
            }
            catch (Exception)
            {
                systemInfo.OsName = "Could not retrieve OS info";
            }

            // Get motherboard information
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
                foreach (var obj in searcher.Get())
                {
                    systemInfo.MotherboardManufacturer = obj["Manufacturer"]?.ToString() ?? "Unknown";
                    systemInfo.MotherboardModel = obj["Product"]?.ToString() ?? "Unknown";
                    break;
                }
            }
            catch (Exception)
            {
                systemInfo.MotherboardManufacturer = "Could not retrieve motherboard info";
            }

            return systemInfo;
        });
    }
}