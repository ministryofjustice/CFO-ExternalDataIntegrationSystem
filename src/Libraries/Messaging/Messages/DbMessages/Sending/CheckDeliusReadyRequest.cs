using System.Text.Json.Serialization;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public class CheckDeliusReadyRequest : DbRequestMessage<CheckDeliusReadyResponse>
{
    public override StatusUpdateMessage StatusMessage => new("Sent request to check if delius is ready for processing.");

    [JsonConstructor]
    public CheckDeliusReadyRequest()
    {
        Queue = TDbQueue.IsDeliusReadyForProcessing;
    }
}
