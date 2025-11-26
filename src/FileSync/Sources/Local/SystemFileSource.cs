
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace FileSync.Sources.Local;

public class SystemFileSource(
    ILogger<SystemFileSource> logger,
    FileSourceOptions options) : FileSource
{
    public override Task<string> RetrieveFileAsync(string source, string target, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Retrieving file from local system: {source}");

        var sourcePath = Path.GetFullPath(source);
        var targetPath = Path.GetFullPath(Path.Combine(target, Path.GetFileName(source)));

        if (sourcePath != targetPath)
        {
            File.Copy(sourcePath, targetPath);
        }

        logger.LogInformation($"Retrieved file from local system: {targetPath}");

        return Task.FromResult(targetPath);
    }

    public override async Task<IReadOnlyList<DeliusFile>> ListDeliusFilesAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Listing Delius files from local system at '{options.Source}' with pattern: {FileConstants.DeliusFilePattern}");
        var files = await GetFiles(options.Source, FileConstants.DeliusFilePattern);
        return files.Select(file => new DeliusFile(file)).ToList();
    }

    public override async Task<IReadOnlyList<OfflocFile>> ListOfflocFilesAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Listing Offloc files from local system at '{options.Source}' with pattern: {FileConstants.OfflocFileOrArchivePattern}");
        var files = await GetFiles(options.Source, FileConstants.OfflocFileOrArchivePattern);
        return files.Select(file => new OfflocFile(file)).ToList();
    }
    
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