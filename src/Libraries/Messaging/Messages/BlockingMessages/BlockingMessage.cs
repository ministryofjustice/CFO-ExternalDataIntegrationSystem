using Messaging.Queues;

namespace Messaging.Messages.BlockingMessages
{
    public abstract class BlockingMessage : Message
    {
        public TBlockingQueue Queue { get; set; }
        public string FileName { get; set; } = string.Empty;
        
        public override string Exchange => Exchanges.blocking;
        public override string RoutingKey => Queue.ToString();
    }
}