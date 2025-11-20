public record DeliusFile(string Path)
{
    public string Name => System.IO.Path.GetFileName(Path);
}