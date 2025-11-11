using System.Text.RegularExpressions;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileSync;

public class S3FileSource(
    ILogger<S3FileSource> logger,
    FileSourceOptions options,
    IAmazonS3 client) : FileSource
{
    public override async Task RetrieveFileAsync(string source, string target, CancellationToken cancellationToken = default)
    {
        var uri = new Uri(source);
        var fileName = uri.AbsolutePath.TrimStart('/');

        await client.DownloadToFilePathAsync(
            bucketName: uri.Host,
            objectKey: fileName,
            filepath: Path.Combine(target, fileName),
            new Dictionary<string, object> { },
            cancellationToken);
    }

    public override Task<IReadOnlyList<string>> ListDeliusFilesAsync(CancellationToken cancellationToken = default)
        => GetFilesAsync(options.Source, DeliusFilePattern, cancellationToken);
        
    public override Task<IReadOnlyList<string>> ListOfflocFilesAsync(CancellationToken cancellationToken = default)
        => GetFilesAsync(options.Source, OfflocFilePattern, cancellationToken);

    private async Task<IReadOnlyList<string>> GetFilesAsync(string bucketName, string pattern, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Attempting to retrieve objects from S3 file store: {bucketName}");

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