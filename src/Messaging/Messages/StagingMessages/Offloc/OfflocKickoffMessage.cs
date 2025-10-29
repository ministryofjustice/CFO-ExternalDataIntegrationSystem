using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.StagingMessages;

public class OfflocKickoffMessage : StagingMessage
{
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage();

    [JsonConstructor]
    public OfflocKickoffMessage()
    {
        routingKey = TStagingQueue.OfflocFileDownload;
        
    }
}
