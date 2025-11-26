using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public class GetLastProcessedDeliusFile : DbRequestMessage
{
    public override StatusUpdateMessage StatusMessage => new("Getting last processed delius file.");

    public GetLastProcessedDeliusFile()
    {
        Queue = TDbQueue.GetLastProcessedDeliusFile;
        ReplyQueue = TDbQueue.ResultLastProcessedDeliusFile;
    }
}