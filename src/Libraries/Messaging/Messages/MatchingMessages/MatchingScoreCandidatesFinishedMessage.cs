using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.MatchingMessages;

public class MatchingScoreCandidatesFinishedMessage : MatchingMessage
{
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage($"Matching complete.");

    [JsonConstructor]
    public MatchingScoreCandidatesFinishedMessage()
    {
        routingKey = TMatchingQueue.MatchingScoreCandidatesFinished;
    }
}
