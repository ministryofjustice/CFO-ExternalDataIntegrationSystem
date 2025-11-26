using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Receiving;

public class IsOfflocReadyForProcessingReturnMessage : DbResponseMessage
{
    public bool isReady;
    public override StatusUpdateMessage StatusMessage => new StatusUpdateMessage("Received Offloc readiness: " + isReady);

    [JsonConstructor]
    public IsOfflocReadyForProcessingReturnMessage()
    { }

    public IsOfflocReadyForProcessingReturnMessage(bool isReady)
    {
        Queue = TDbQueue.IsOfflocReadyForProcessingResult;
        this.isReady = isReady;
    }
}