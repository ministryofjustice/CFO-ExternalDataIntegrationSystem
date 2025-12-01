using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.MatchingMessages.Clustering;

public class ClusteringPreProcessingFinishedMessage : MatchingMessage
{
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage($"Clustering (pre-processing) complete.");

    [JsonConstructor]
    public ClusteringPreProcessingFinishedMessage()
    {
        Queue = TMatchingQueue.ClusteringPreProcessingFinished;
    }
}
