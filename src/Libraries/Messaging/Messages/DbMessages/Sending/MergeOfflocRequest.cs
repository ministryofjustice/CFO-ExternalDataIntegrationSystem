using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StatusMessages;
using System.Text.Json.Serialization;
using Messaging.Queues;
using Messaging.Messages.MergingMessages;

namespace Messaging.Messages.DbMessages.Sending;

public class MergeOfflocRequest : DbRequestMessage<MergeOfflocResponse>
{
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Merging into Offloc running picture started.");
    public required string FileName { get; set; }
    public override TDbQueue Queue { get; set; } = TDbQueue.MergeOffloc;
}
