using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Receiving;

public class StageDeliusResponse : DbResponseMessage
{
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Delius Staging completed.");

    [JsonConstructor]
    public StageDeliusResponse()
    {
        Queue = TDbQueue.ResultStageDelius;
    }
}