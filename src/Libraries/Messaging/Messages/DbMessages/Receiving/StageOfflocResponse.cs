using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Receiving;

public class StageOfflocResponse : DbResponseMessage
{
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Offloc Staging completed.");

    [JsonConstructor]
    public StageOfflocResponse()
    {
        Queue = TDbQueue.ResultStageOffloc;
    }
}