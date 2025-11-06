namespace EnvironmentSetup;

public abstract class FileSource
{
    /// <summary>
    /// Returns a collection of Offloc files (including their paths and extensions) from the file source.
    /// </summary>
    public abstract Task<IReadOnlyList<string>> ListOfflocFilesAsync(string source, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a collection of Delius files (including their paths and extensions) from the file source.
    /// </summary>
    public abstract Task<IReadOnlyList<string>> ListDeliusFilesAsync(string source, CancellationToken cancellationToken = default);

    public abstract Task RetrieveFileAsync(string source, string target, CancellationToken cancellationToken = default);

    public virtual string DeliusFilePattern => @"^.*\.(txt)$";
    public virtual string OfflocFilePattern => @"^.*\.(dat)$";

}