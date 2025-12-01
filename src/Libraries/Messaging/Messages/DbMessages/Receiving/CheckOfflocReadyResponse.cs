using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Receiving;

public class CheckOfflocReadyResponse : DbResponseMessage
{
    public bool IsReady { get; set; }
    public override StatusUpdateMessage StatusMessage => new StatusUpdateMessage("Received Offloc readiness: " + IsReady);
    public override TDbQueue Queue { get; set; } = TDbQueue.IsOfflocReadyForProcessingResult;
}