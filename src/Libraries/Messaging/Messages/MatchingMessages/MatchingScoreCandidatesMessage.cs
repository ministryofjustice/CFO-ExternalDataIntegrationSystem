using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.MatchingMessages;

public class MatchingScoreCandidatesMessage : MatchingMessage
{
    [JsonConstructor]
    public MatchingScoreCandidatesMessage()
    {
        Queue = TMatchingQueue.MatchingScoreCandidatesMessage;
    }

    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage($"Scoring candidates...");
}

