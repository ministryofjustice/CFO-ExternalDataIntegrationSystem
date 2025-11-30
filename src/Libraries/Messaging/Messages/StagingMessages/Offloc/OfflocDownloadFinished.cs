
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.StagingMessages;

public class OfflocDownloadFinished : StagingMessage
{
    public override StatusUpdateMessage StatusMessage => new();

    public string? ArchiveFileName { get; set; }
    public int FileId { get; set; }

    [JsonConstructor]
    public OfflocDownloadFinished(string fileName, int fileId, string? archiveFileName = null)
    {
        Queue = TStagingQueue.OfflocCleaner;
        base.FileName = fileName;
        FileId = fileId;
        ArchiveFileName = archiveFileName;
    }
}
