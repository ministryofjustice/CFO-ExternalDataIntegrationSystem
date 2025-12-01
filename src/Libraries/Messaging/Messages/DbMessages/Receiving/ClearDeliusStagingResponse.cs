
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Receiving;

public class ClearDeliusStagingResponse : DbResponseMessage
{
	public override StatusUpdateMessage StatusMessage =>
		new StatusUpdateMessage("Delius staging cleared.");

    public ClearDeliusStagingResponse()
    {
        Queue = TDbQueue.ResultClearDelius;
    }
}
