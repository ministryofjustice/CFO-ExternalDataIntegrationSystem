using System.Text.RegularExpressions;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;

namespace FileSync.Sources.S3;

public class S3FileSource(
    ILogger<S3FileSource> logger,
    FileSourceOptions options,
    IAmazonS3 client) : FileSource
{
    public override async Task<string> RetrieveFileAsync(string source, string target, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Retrieving object from S3: {source}");

        var uri = new Uri(source);
        var fileName = uri.AbsolutePath.TrimStart('/');
        var targetPath = Path.Combine(target, fileName);

        await client.DownloadToFilePathAsync(
            bucketName: uri.Host,
            objectKey: fileName,
            filepath: targetPath,
            new Dictionary<string, object> { },
            cancellationToken);

        logger.LogInformation($"Retrieved object from S3: {targetPath}");

        return targetPath;
    }

    public override async Task<IReadOnlyList<DeliusFile>> ListDeliusFilesAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Listing Delius files from S3 bucket '{options.Source}' with pattern: {FileConstants.DeliusFilePattern}");
        var files = await GetFilesAsync(options.Source, FileConstants.DeliusFilePattern, cancellationToken);
        return files.Select(file => new DeliusFile(file)).ToList();
    }
        
    public override async Task<IReadOnlyList<OfflocFile>> ListOfflocFilesAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Listing Offloc files from S3 bucket '{options.Source}' with pattern: {FileConstants.OfflocFileOrArchivePattern}");
        var files = await GetFilesAsync(options.Source, FileConstants.OfflocFileOrArchivePattern, cancellationToken);
        return files.Select(file => new OfflocFile(file)).ToList();
    }

    private async Task<IReadOnlyList<string>> GetFilesAsync(string bucketName, string pattern, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Attempting to list objects from S3 file store: {bucketName}");

        var request = new ListObjectsV2Request
        {
            BucketName = bucketName
        };

        List<string> files = [];

        var paginator = client.Paginators.ListObjectsV2(request);

        await foreach (var response in paginator.Responses.WithCancellation(cancellationToken))
        {
            files.AddRange(response.S3Objects.Where(o =>
            {
                var fileName = Path.GetFileName(o.Key);
                return Regex.IsMatch(fileName, pattern);
            }).Select(o => $"s3://{bucketName}/{o.Key}"));
        }

        logger.LogInformation($"Retrieved {files.Count} object(s) from S3 file store: {bucketName}");

        return files.AsReadOnly();
    }

}