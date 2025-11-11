
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace FileSync;

public class SystemFileSource(FileSourceOptions options) : FileSource
{
    public override Task RetrieveFileAsync(string source, string target, CancellationToken cancellationToken = default)
    {
        var sourcePath = Path.GetFullPath(source);
        var targetPath = Path.GetFullPath(Path.Combine(target, Path.GetFileName(source)));

        if (sourcePath != targetPath)
        {
            File.Copy(sourcePath, targetPath);
        }

        return Task.CompletedTask;
    }

    public override Task<IReadOnlyList<string>> ListDeliusFilesAsync(CancellationToken cancellationToken = default)
        => GetFiles(options.Source, DeliusFilePattern);

    public override Task<IReadOnlyList<string>> ListOfflocFilesAsync(CancellationToken cancellationToken = default)
        => GetFiles(options.Source, OfflocFilePattern);
    
    private static Task<IReadOnlyList<string>> GetFiles(string location, string pattern)
    {
        IReadOnlyList<string> files = Directory.GetFiles(location, "*", SearchOption.AllDirectories).Where(file =>
        {
            var fileName = Path.GetFileName(file);
            return Regex.IsMatch(fileName, pattern);
        }).ToList();

        return Task.FromResult(files);
    }
}