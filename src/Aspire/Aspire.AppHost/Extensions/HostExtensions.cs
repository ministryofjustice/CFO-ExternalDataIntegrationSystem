namespace Aspire.AppHost.Extensions;

public static class HostExtensions
{
    public static string Create(string hostDir)
    {
        var delius = Path.Combine(hostDir, "Delius");
        var offloc = Path.Combine(hostDir, "Offloc");
        Directory.CreateDirectory(Path.Combine(delius, "Input"));
        Directory.CreateDirectory(Path.Combine(delius, "Output"));
        Directory.CreateDirectory(Path.Combine(offloc, "Input"));
        Directory.CreateDirectory(Path.Combine(offloc, "Output"));
        return hostDir;
    }
} 