using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public class ClearOfflocStagingRequest : DbRequestMessage<ClearOfflocStagingResponse>
{
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Offloc staging database being cleared.");
    public override TDbQueue Queue { get; set; } = TDbQueue.ClearOfflocStaging;
}
