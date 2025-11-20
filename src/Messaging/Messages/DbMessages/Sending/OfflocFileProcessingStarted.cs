using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public class OfflocFileProcessingStarted : DbRequestMessage
{
    public override StatusUpdateMessage StatusMessage => new();

    public string fileName = string.Empty;
    public int fileId;
    public string? archiveName;

    [JsonConstructor]
    public OfflocFileProcessingStarted()
    {
        Queue = TDbQueue.OfflocFileProcessingStarted;
        ReplyQueue = TDbQueue.ResultOfflocFileProcessingStarted;
    }

    public OfflocFileProcessingStarted(string fileName, int fileId, string? archiveName = null) : this()
    {
        this.fileName = fileName;
        this.fileId = fileId;
        this.archiveName = archiveName;
    }
}