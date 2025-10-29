using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.MatchingMessages.Clustering;

public class ClusteringPostProcessingFinishedMessage : MatchingMessage
{
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage($"Clustering (post-processing) complete.");

    [JsonConstructor]
    public ClusteringPostProcessingFinishedMessage()
    {
        routingKey = TMatchingQueue.ClusteringPostProcessingFinished;
    }
}
