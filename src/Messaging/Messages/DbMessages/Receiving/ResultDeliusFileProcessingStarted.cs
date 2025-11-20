using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Receiving;

public class ResultDeliusFileProcessingStarted : DbResponseMessage
{
	public override StatusUpdateMessage StatusMessage => new("Delius file processing started");

    public ResultDeliusFileProcessingStarted()
    {
        Queue = TDbQueue.ResultDeliusFileProcessingStarted;
    }
}