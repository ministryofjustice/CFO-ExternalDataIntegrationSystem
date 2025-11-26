
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Receiving;

public class MergeOfflocReturnMessage : DbResponseMessage
{
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage($"Offloc staging database successfully " +
            $"merged into offloc running picture database.");

    public MergeOfflocReturnMessage()
    {
        Queue = TDbQueue.ResultMergeOffloc;
    }
}
