using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public class GetLastProcessedDeliusFileRequest : DbRequestMessage<GetLastProcessedDeliusFileResponse>
{
    public override StatusUpdateMessage StatusMessage => new("Getting last processed delius file.");
    public override TDbQueue Queue { get; set; } = TDbQueue.GetLastProcessedDeliusFile;
}