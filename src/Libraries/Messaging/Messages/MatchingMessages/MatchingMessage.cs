using Messaging.Queues;

namespace Messaging.Messages.MatchingMessages;

public abstract class MatchingMessage : Message
{
    public TMatchingQueue Queue { get; set; }
    
    public override string Exchange => Exchanges.matching;
    public override string RoutingKey => Queue.ToString();
}
