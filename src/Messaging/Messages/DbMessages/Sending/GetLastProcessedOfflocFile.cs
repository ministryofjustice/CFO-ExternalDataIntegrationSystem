using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public class GetLastProcessedOfflocFile : DbRequestMessage
{
    public override StatusUpdateMessage StatusMessage => new("Getting last processed offloc file.");

    public GetLastProcessedOfflocFile()
    {
        Queue = TDbQueue.GetLastProcessedOfflocFile;
        ReplyQueue = TDbQueue.ResultLastProcessedOfflocFile;
    }

}