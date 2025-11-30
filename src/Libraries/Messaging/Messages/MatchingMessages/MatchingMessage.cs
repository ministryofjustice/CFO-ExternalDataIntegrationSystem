using Messaging.Queues;

namespace Messaging.Messages.MatchingMessages;

public abstract class MatchingMessage : Message
{
    public TMatchingQueue routingKey { get; set; }
    
    public override string Exchange => Exchanges.matching;
    public override string RoutingKey => routingKey.ToString();
}
