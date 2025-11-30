using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public class OfflocFileProcessingStarted : DbRequestMessage
{
    public override StatusUpdateMessage StatusMessage => new();

    public string FileName { get; set; } = string.Empty;
    public int FileId { get; set; }
    public string? ArchiveName { get; set; }

    [JsonConstructor]
    public OfflocFileProcessingStarted()
    {
        Queue = TDbQueue.OfflocFileProcessingStarted;
        ReplyQueue = TDbQueue.ResultOfflocFileProcessingStarted;
    }

    public OfflocFileProcessingStarted(string fileName, int fileId, string? archiveName = null) : this()
    {
        FileName = fileName;
        FileId = fileId;
        ArchiveName = archiveName;
    }
}