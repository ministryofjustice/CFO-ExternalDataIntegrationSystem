
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Receiving;

public class ResultClearDeliusStaging : DbResponseMessage
{
	public override StatusUpdateMessage StatusMessage =>
		new StatusUpdateMessage("Delius staging cleared.");

    public ResultClearDeliusStaging()
    {
        Queue = TDbQueue.ResultClearDelius;
    }
}
