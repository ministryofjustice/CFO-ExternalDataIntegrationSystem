
using System.Text.RegularExpressions;

namespace EnvironmentSetup;

public class SystemFileSource : FileSource
{
    public override Task RetrieveFileAsync(string source, string target, CancellationToken cancellationToken = default)
    {
        var sourcePath = Path.GetFullPath(source);
        var targetPath = Path.GetFullPath(target);

        if (sourcePath != targetPath)
        {
            File.Copy(sourcePath, targetPath);
        }

        return Task.CompletedTask;
    }

    public override Task<IReadOnlyList<string>> ListDeliusFilesAsync(string source, CancellationToken cancellationToken = default)
        => GetFiles(source, DeliusFilePattern);

    public override Task<IReadOnlyList<string>> ListOfflocFilesAsync(string source, CancellationToken cancellationToken = default)
        => GetFiles(source, OfflocFilePattern);
    
    private static Task<IReadOnlyList<string>> GetFiles(string location, string pattern)
    {
        IReadOnlyList<string> files = Directory.GetFiles(location).Where(file =>
        {
            var fileName = Path.GetFileName(file);
            return Regex.IsMatch(fileName, pattern);
        }).ToList();

        return Task.FromResult(files);
    }
}