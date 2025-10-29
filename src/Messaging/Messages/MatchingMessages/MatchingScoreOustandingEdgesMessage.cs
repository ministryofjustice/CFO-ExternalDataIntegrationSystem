using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.MatchingMessages;

public class MatchingScoreOutstandingEdgesMessage : MatchingMessage
{
    [JsonConstructor]
    public MatchingScoreOutstandingEdgesMessage()
    {
        routingKey = TMatchingQueue.MatchingScoreOutstandingEdgesMessage;
    }

    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage($"Scoring outstanding edges...");
}
