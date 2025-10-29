using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Receiving;

public class ResultClearOfflocStaging : DbResponseMessage
{
	public override StatusUpdateMessage StatusMessage =>
		new StatusUpdateMessage("Offloc staging cleared.");

    public ResultClearOfflocStaging()
    {
        Queue = TDbQueue.ResultClearOffloc;
    }
}