using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Receiving;

public class StartOfflocFileProcessingResponse : DbResponseMessage
{
	public override StatusUpdateMessage StatusMessage => new("Offloc file processing started");

    [JsonConstructor]
    public StartOfflocFileProcessingResponse()
    {
        Queue = TDbQueue.ResultOfflocFileProcessingStarted;
    }
}