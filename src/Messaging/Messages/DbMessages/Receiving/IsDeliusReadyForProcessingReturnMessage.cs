using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Receiving;

public class IsDeliusReadyForProcessingReturnMessage : DbResponseMessage
{
    public bool isReady;
    public override StatusUpdateMessage StatusMessage => new StatusUpdateMessage("Received Delius readiness: " + isReady);

    [JsonConstructor]
    public IsDeliusReadyForProcessingReturnMessage()
    { }

    public IsDeliusReadyForProcessingReturnMessage(bool isReady)
    {
        Queue = TDbQueue.IsDeliusReadyForProcessingResult;
        this.isReady = isReady;
    }
}