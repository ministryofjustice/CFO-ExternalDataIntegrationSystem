using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Receiving;

public class StartDeliusFileProcessingResponse : DbResponseMessage
{
	public override StatusUpdateMessage StatusMessage => new("Delius file processing started");
    public override TDbQueue Queue { get; set; } = TDbQueue.ResultDeliusFileProcessingStarted;
}