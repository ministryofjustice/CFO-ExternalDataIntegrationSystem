
using Messaging.Queues;

namespace Messaging.Messages.StatusMessages;

public class StagingFinishedMessage : StatusUpdateMessage
{
    public override string RoutingKey { get; }
    
    public StagingFinishedMessage()
    {
        RoutingKey = TStatusQueue.StagingFinished.ToString();
    }
}
