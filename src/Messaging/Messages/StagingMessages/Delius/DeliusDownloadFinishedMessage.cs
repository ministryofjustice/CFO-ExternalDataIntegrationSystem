using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.StagingMessages;

public class DeliusDownloadFinishedMessage : StagingMessage
{
    public override StatusUpdateMessage StatusMessage => new();

    public string File { get; }

    [JsonConstructor]
    public DeliusDownloadFinishedMessage(string file)
    {
        routingKey = TStagingQueue.DeliusParser;
        File = file;
    }
    
}
