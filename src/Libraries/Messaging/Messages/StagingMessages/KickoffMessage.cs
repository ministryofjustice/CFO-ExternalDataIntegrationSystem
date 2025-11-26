
using Messaging.Messages.StatusMessages;
using System.Text.Json.Serialization;
using Messaging.Queues;

namespace Messaging.Messages.StagingMessages;

public class KickoffMessage : StagingMessage
{
	public override StatusUpdateMessage StatusMessage => new StatusUpdateMessage();

    [JsonConstructor]
    public KickoffMessage()
    {
        routingKey = TStagingQueue.Kickoff;
    }
}
