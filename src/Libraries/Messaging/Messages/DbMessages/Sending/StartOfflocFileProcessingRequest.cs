using System.Text.Json.Serialization;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public class StartOfflocFileProcessingRequest : DbRequestMessage<StartOfflocFileProcessingResponse>
{
    public override StatusUpdateMessage StatusMessage => new();
    public required string FileName { get; set; }
    public required int FileId { get; set; }
    public string? ArchiveName { get; set; }
    public override TDbQueue Queue { get; set; } = TDbQueue.OfflocFileProcessingStarted;
}