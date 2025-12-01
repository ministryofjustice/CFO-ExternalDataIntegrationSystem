using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Sending;

public class StageOfflocRequest : DbRequestMessage<StageOfflocResponse>
{
    public required string FileName { get; set; }
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage();
    public override TDbQueue Queue { get; set; } = TDbQueue.StageOffloc;
}
