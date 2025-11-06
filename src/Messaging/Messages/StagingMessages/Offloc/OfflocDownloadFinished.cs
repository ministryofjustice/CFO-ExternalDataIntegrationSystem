
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.StagingMessages;

public class OfflocDownloadFinished : StagingMessage
{
    public override StatusUpdateMessage StatusMessage => new();

    [JsonConstructor]
    public OfflocDownloadFinished()
    {
        routingKey = TStagingQueue.OfflocCleaner;
    }
}
