
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace FileSync.Sources.Minio;

public class MinioFileSource(
    ILogger<MinioFileSource> logger,
    FileSourceOptions options,
    IMinioClient minioClient) : FileSource
{    
    public override async Task<IReadOnlyList<DeliusFile>> ListDeliusFilesAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Listing Delius files from Minio bucket '{options.Source}' with pattern: {FileConstants.DeliusFilePattern}");
        var files = await GetFilesAsync(options.Source, FileConstants.DeliusFilePattern, cancellationToken);
        return files.Select(file => new DeliusFile(file)).ToList();
    }

    public override async Task<IReadOnlyList<OfflocFile>> ListOfflocFilesAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Listing Offloc files from Minio bucket '{options.Source}' with pattern: {FileConstants.OfflocFileOrArchivePattern}");
        var files = await GetFilesAsync(options.Source, FileConstants.OfflocFileOrArchivePattern, cancellationToken);
        return files.Select(file => new OfflocFile(file)).ToList();
    }
    private async Task<IReadOnlyList<string>> GetFilesAsync(string bucketName, string pattern, CancellationToken cancellationToken)
    {
        var args = new ListObjectsArgs()
            .WithBucket(bucketName);

        return await minioClient.ListObjectsEnumAsync(args, cancellationToken)
            .Where(obj =>
            {
                var fileName = Path.GetFileName(obj.Key);
                return Regex.IsMatch(fileName, pattern);
            })
            .Select(obj => $"minio://{bucketName}/{obj.Key}")
            .ToListAsync(cancellationToken);
        }

    public override async Task<string> RetrieveFileAsync(string source, string target, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Retrieving object from Minio: {source}");

        var uri = new Uri(source);
        var fileName = uri.AbsolutePath.TrimStart('/');
        var targetPath = Path.Combine(target, fileName);

        var args = new GetObjectArgs()
            .WithBucket(uri.Host)
            .WithObject(fileName)
            .WithFile(targetPath);

        await minioClient.GetObjectAsync(args, cancellationToken);
        
        logger.LogInformation($"Retrieved object from Minio: {targetPath}");
        return targetPath;
    }
}