namespace FileSync.Core;

public abstract class FileSource
{
    /// <summary>
    /// Returns a collection of Offloc files (including their paths and extensions) from the file source.
    /// </summary>
    public abstract Task<IReadOnlyList<OfflocFile>> ListOfflocFilesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a collection of Delius files (including their paths and extensions) from the file source.
    /// </summary>
    public abstract Task<IReadOnlyList<DeliusFile>> ListDeliusFilesAsync(CancellationToken cancellationToken = default);

    public abstract Task<string> RetrieveFileAsync(string source, string target, CancellationToken cancellationToken = default);
}