using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.MatchingMessages.Clustering;

public class ClusteringPreProcessingStartedMessage : MatchingMessage
{
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage($"Clustering (pre-processing) started...");

    [JsonConstructor]
    public ClusteringPreProcessingStartedMessage()
    {
        routingKey = TMatchingQueue.ClusteringPreProcessingStarted;
    }
}
