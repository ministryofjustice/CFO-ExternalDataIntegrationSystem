using System.Text.Json.Serialization;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public class StartDeliusFileProcessingRequest : DbRequestMessage<StartDeliusFileProcessingResponse>
{
    public override StatusUpdateMessage StatusMessage => new();
    public required string FileName { get; set; }
    public required string FileId { get; set; }
    public override TDbQueue Queue { get; set; } = TDbQueue.DeliusFileProcessingStarted;
}