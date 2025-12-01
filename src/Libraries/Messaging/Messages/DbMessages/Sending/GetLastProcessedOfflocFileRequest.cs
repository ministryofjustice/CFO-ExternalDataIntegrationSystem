using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public class GetLastProcessedOfflocFileRequest : DbRequestMessage<GetLastProcessedOfflocFileResponse>
{
    public override StatusUpdateMessage StatusMessage => new("Getting last processed offloc file.");
    public override TDbQueue Queue { get; set; } = TDbQueue.GetLastProcessedOfflocFile;
}