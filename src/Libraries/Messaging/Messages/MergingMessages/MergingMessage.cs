
using Messaging.Queues;

namespace Messaging.Messages.MergingMessages;

public abstract class MergingMessage : Message
{
    public TMergingQueue routingKey;
    public string fileName = string.Empty;
    
    public override string Exchange => Exchanges.merging;
    public override string RoutingKey => routingKey.ToString();
}
