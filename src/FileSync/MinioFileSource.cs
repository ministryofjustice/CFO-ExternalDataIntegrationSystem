
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace FileSync;

public class MinioFileSource(
    FileSourceOptions options,
    IMinioClient minioClient) : FileSource
{    
    public override Task<IReadOnlyList<string>> ListDeliusFilesAsync(CancellationToken cancellationToken = default)
        => GetFilesAsync(options.Source, DeliusFilePattern, cancellationToken);

    public override Task<IReadOnlyList<string>> ListOfflocFilesAsync(CancellationToken cancellationToken = default)
        => GetFilesAsync(options.Source, OfflocFilePattern, cancellationToken);

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

    public override async Task RetrieveFileAsync(string source, string target, CancellationToken cancellationToken = default)
    {
        var uri = new Uri(source);
        var fileName = uri.AbsolutePath.TrimStart('/');

        var args = new GetObjectArgs()
            .WithBucket(uri.Host)
            .WithObject(fileName)
            .WithFile(Path.Combine(target, fileName));

        await minioClient.GetObjectAsync(args, cancellationToken);
    }
}