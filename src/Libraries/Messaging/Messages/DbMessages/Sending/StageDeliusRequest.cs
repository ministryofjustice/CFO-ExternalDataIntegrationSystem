
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Sending;

public class StageDeliusRequest : DbRequestMessage<StageDeliusResponse>
{
    public required string FileName { get; set; }
    public required string FilePath { get; set; }
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Delius Staging started.");
    public override TDbQueue Queue { get; set; } = TDbQueue.StageDelius;
}
