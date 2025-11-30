using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.StagingMessages;

public class DeliusKickoffMessage : StagingMessage
{
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage();

    public DeliusKickoffMessage()
    {
        Queue = TStagingQueue.DeliusFileDownload;
    }
}
