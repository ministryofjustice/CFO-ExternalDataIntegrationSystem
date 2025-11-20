
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
    private readonly SemaphoreSlim bucketInitLock = new(1, 1);
    private bool bucketInitialised;

    public override async Task<IReadOnlyList<DeliusFile>> ListDeliusFilesAsync(CancellationToken cancellationToken = default)
    {
        var files = await GetFilesAsync(options.Source, FileConstants.DeliusFilePattern, cancellationToken);
        return files.Select(file => new DeliusFile(file)).ToList();
    }

    public override async Task<IReadOnlyList<OfflocFile>> ListOfflocFilesAsync(CancellationToken cancellationToken = default)
    {
        var files = await GetFilesAsync(options.Source, FileConstants.OfflocFileOrArchivePattern, cancellationToken);
        return files.Select(file => new OfflocFile(file)).ToList();
    }
    private async Task<IReadOnlyList<string>> GetFilesAsync(string bucketName, string pattern, CancellationToken cancellationToken)
    {
        await EnsureBucketExistsAsync(bucketName, cancellationToken);

        var args = new ListObjectsArgs()
            .WithBucket(bucketName);

        logger.LogInformation("Listing objects in Minio bucket '{Bucket}' matching pattern: {Pattern}", bucketName, pattern);

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
        logger.LogInformation("Retrieving object from Minio: {Source}", source);

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

    private async Task EnsureBucketExistsAsync(string bucketName, CancellationToken cancellationToken)
    {
        if (bucketInitialised)
        {
            return;
        }

        await bucketInitLock.WaitAsync(cancellationToken);
        try
        {
            if (bucketInitialised)
            {
                return;
            }

            var exists = await minioClient.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(bucketName),
                cancellationToken);

            if (!exists)
            {
                logger.LogInformation("Bucket '{Bucket}' does not exist in Minio. Creating it.", bucketName);

                await minioClient.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(bucketName),
                    cancellationToken);

                logger.LogInformation("Created bucket '{Bucket}' in Minio.", bucketName);
            }

            bucketInitialised = true;
        }
        finally
        {
            bucketInitLock.Release();
        }
    }
}