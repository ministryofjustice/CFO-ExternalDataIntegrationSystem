using Messaging.Messages.StatusMessages;
using System.Text.Json.Serialization;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Receiving;

public class MergeDeliusReturnMessage : DbResponseMessage
{
	public override StatusUpdateMessage StatusMessage =>
         new StatusUpdateMessage($"Delius staging database successfully " +
            $"merged into Delius running picture database.");

    [JsonConstructor]
    public MergeDeliusReturnMessage()
    {
        Queue = TDbQueue.ResultMergeDelius;
    }
}