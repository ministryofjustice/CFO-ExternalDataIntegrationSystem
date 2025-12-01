
using Messaging.Queues;

namespace Messaging.Messages.MergingMessages;

public abstract class MergingMessage : Message
{
    public TMergingQueue Queue { get; set; }
    public string FileName { get; set; } = string.Empty;
    
    public override string Exchange => Exchanges.merging;
    public override string RoutingKey => Queue.ToString();
}
