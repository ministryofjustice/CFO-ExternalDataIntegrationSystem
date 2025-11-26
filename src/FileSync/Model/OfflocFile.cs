public record OfflocFile(string Path, string? ParentArchiveName = null)
{
    public string Name => System.IO.Path.GetFileName(Path);
    public bool IsArchive => Path.EndsWith(".zip", StringComparison.OrdinalIgnoreCase);
}