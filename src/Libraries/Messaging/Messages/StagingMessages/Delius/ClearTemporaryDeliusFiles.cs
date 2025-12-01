
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.StagingMessages.Delius;

public class ClearTemporaryDeliusFiles : StagingMessage
{
    public override StatusUpdateMessage StatusMessage => new StatusUpdateMessage();

    public ClearTemporaryDeliusFiles()
    {
        Queue = TStagingQueue.DeliusFilesClear;
    }
}
