
using Messaging.Queues;

namespace Messaging.Messages.DbMessages;

public abstract class DbMessage : Message
{
    public TDbQueue Queue { get; set; }
    
    public override string Exchange => Exchanges.database;
    public override string RoutingKey => Queue.ToString();
}
