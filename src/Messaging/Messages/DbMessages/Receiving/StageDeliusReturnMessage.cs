using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Receiving;

public class StageDeliusReturnMessage : DbResponseMessage
{
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Delius Staging completed.");

    [JsonConstructor]
    public StageDeliusReturnMessage()
    {
        Queue = TDbQueue.ResultStageDelius;
    }
}