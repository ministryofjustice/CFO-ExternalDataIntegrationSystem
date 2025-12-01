using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Receiving;

public class StartOfflocFileProcessingResponse : DbResponseMessage
{
	public override StatusUpdateMessage StatusMessage => new("Offloc file processing started");
    public override TDbQueue Queue { get; set; } = TDbQueue.ResultOfflocFileProcessingStarted;
}