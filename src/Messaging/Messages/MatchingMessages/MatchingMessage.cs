using Messaging.Queues;

namespace Messaging.Messages.MatchingMessages;

public abstract class MatchingMessage : Message
{
    public TMatchingQueue routingKey { get; set; }
}
