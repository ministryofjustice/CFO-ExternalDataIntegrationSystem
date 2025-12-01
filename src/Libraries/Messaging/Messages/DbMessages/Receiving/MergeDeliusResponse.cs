using Messaging.Messages.StatusMessages;
using System.Text.Json.Serialization;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Receiving;

public class MergeDeliusResponse : DbResponseMessage
{
	public override StatusUpdateMessage StatusMessage =>
         new StatusUpdateMessage($"Delius staging database successfully " +
            $"merged into Delius running picture database.");
    public override TDbQueue Queue { get; set; } = TDbQueue.ResultMergeDelius;
}