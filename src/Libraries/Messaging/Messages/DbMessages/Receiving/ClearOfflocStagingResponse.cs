using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Receiving;

public class ClearOfflocStagingResponse : DbResponseMessage
{
	public override StatusUpdateMessage StatusMessage =>
		new StatusUpdateMessage("Offloc staging cleared.");
    public override TDbQueue Queue { get; set; } = TDbQueue.ResultClearOffloc;
}