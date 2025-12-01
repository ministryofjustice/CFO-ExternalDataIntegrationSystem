using System.Text.Json.Serialization;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public class StartOfflocFileProcessingRequest : DbRequestMessage<StartOfflocFileProcessingResponse>
{
    public override StatusUpdateMessage StatusMessage => new();

    public string FileName { get; set; } = string.Empty;
    public int FileId { get; set; }
    public string? ArchiveName { get; set; }

    [JsonConstructor]
    public StartOfflocFileProcessingRequest()
    {
        Queue = TDbQueue.OfflocFileProcessingStarted;
    }

    public StartOfflocFileProcessingRequest(string fileName, int fileId, string? archiveName = null) : this()
    {
        FileName = fileName;
        FileId = fileId;
        ArchiveName = archiveName;
    }
}