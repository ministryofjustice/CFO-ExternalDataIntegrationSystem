
using Messaging.Queues;

namespace Messaging.Messages.DbMessages;

public abstract class DbMessage : Message
{
    public TDbQueue Queue { get; set; }
}
