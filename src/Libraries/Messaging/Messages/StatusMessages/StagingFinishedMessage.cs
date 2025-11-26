
using Messaging.Queues;

namespace Messaging.Messages.StatusMessages;

public class StagingFinishedMessage : StatusUpdateMessage
{
    public StagingFinishedMessage()
    {
        RoutingKey = TStatusQueue.StagingFinished;
    }
}
