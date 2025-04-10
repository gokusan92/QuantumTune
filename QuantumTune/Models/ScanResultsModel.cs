namespace WindowsOptimizer.Models;

public class ScanResultsModel
{
    public int IssuesCount { get; set; }
    public int PerformanceScore { get; set; }
    public ulong RecoverableSpaceInBytes { get; set; }

    public string FormattedRecoverableSpace => FormatBytes(RecoverableSpaceInBytes);

    private string FormatBytes(ulong bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        var counter = 0;
        double number = bytes;

        while (number > 1024 && counter < suffixes.Length - 1)
        {
            number /= 1024;
            counter++;
        }

        return $"{number:0.0} {suffixes[counter]}";
    }
}

public class OptimizationResultModel
{
    public bool Success { get; set; }
    public int IssuesFixed { get; set; }
    public ulong SpaceFreed { get; set; }
    public string ErrorMessage { get; set; }
    public bool RegistryFixSuccess { get; set; } // New property
}