using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Receiving;

public class CheckDeliusReadyResponse : DbResponseMessage
{
    public bool IsReady { get; set; }
    public override StatusUpdateMessage StatusMessage => new StatusUpdateMessage("Received Delius readiness: " + IsReady);
    public override TDbQueue Queue { get; set; } = TDbQueue.IsDeliusReadyForProcessingResult;
}