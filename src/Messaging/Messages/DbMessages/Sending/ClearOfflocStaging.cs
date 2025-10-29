using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public class ClearOfflocStaging : DbRequestMessage
{
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Offloc staging database being cleared.");

    public ClearOfflocStaging()
    {
        Queue = TDbQueue.ClearOfflocStaging;
        ReplyQueue = TDbQueue.ResultClearOffloc;
    }
}
