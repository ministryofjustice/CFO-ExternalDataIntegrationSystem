using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.StagingMessages;

public class DeliusDownloadFinishedMessage : StagingMessage
{
    //Make file specific in future.
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage();

    public DeliusDownloadFinishedMessage()
    {
        routingKey = TStagingQueue.DeliusParser;
    }
}
