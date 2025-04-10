namespace WindowsOptimizer.Models;

public class SystemInfoModel
{
    public SystemInfoModel()
    {
        GpuList = new List<GpuInfo>();
        RamModules = new List<RamInfo>();
        Disks = new List<DiskInfo>();
    }

    // CPU Information
    public string CpuModel { get; set; } = "Unknown";
    public int CpuCores { get; set; }
    public int CpuThreads { get; set; }
    public string CpuSpeed { get; set; } = "Unknown";

    // GPU Information
    public List<GpuInfo> GpuList { get; set; }

    // RAM Information
    public string TotalRam { get; set; } = "Unknown";
    public List<RamInfo> RamModules { get; set; }

    // Storage Information
    public List<DiskInfo> Disks { get; set; }

    // OS Information
    public string OsName { get; set; } = "Unknown";
    public string OsVersion { get; set; } = "Unknown";
    public string OsBuild { get; set; } = "Unknown";
    public string OsArchitecture { get; set; } = "Unknown";

    // Motherboard Information
    public string MotherboardManufacturer { get; set; } = "Unknown";
    public string MotherboardModel { get; set; } = "Unknown";
}

public class GpuInfo
{
    public string Name { get; set; } = "Unknown";
    public string DriverVersion { get; set; } = "Unknown";
    public string VideoMemory { get; set; } = "Unknown";
}

public class RamInfo
{
    public string Manufacturer { get; set; } = "Unknown";
    public string Capacity { get; set; } = "Unknown";
    public string Speed { get; set; } = "Unknown";
}

public class DiskInfo
{
    public string DriveLetter { get; set; } = "Unknown";
    public string TotalSize { get; set; } = "Unknown";
    public string FreeSpace { get; set; } = "Unknown";
    public double UsedPercent { get; set; }
}