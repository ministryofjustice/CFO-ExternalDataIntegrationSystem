using Messaging.Queues;

namespace Messaging.Messages.StagingMessages;

public abstract class StagingMessage : Message
{
    public TStagingQueue Queue { get; set; }
    public string FileName { get; set; } = string.Empty;
    
    public override string Exchange => Exchanges.staging;
    public override string RoutingKey => Queue.ToString();
}
