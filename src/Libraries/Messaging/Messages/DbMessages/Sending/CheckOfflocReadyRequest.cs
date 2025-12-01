using System.Text.Json.Serialization;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public class CheckOfflocReadyRequest : DbRequestMessage<CheckOfflocReadyResponse>
{
    public override StatusUpdateMessage StatusMessage => new("Sent request to check if offloc is ready for processing.");
    public override TDbQueue Queue { get; set; } = TDbQueue.IsOfflocReadyForProcessing;
}
