namespace WindowsOptimizer.Models;

public class CleanupInfoModel
{
    public string Name { get; set; }
    public List<string> Paths { get; set; }
    public ulong BytesRecoverable { get; set; }
    public int FilesCount { get; set; }
}

public class CleanupResultModel
{
    public ulong TotalBytesFreed { get; set; }
    public int TotalFilesRemoved { get; set; }
    public Dictionary<string, (ulong BytesFreed, int FilesRemoved)> LocationResults { get; set; }
}