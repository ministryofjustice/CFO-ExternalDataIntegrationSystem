
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.StagingMessages;

public class OfflocDownloadFinished : StagingMessage
{
    public override StatusUpdateMessage StatusMessage => new();

    public string File { get; }

    [JsonConstructor]
    public OfflocDownloadFinished(string file)
    {
        routingKey = TStagingQueue.OfflocCleaner;
        File = file;
    }
}
