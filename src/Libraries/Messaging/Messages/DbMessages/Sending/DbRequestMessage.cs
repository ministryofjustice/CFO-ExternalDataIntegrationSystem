using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public abstract class DbRequestMessage : DbMessage
{
    public TDbQueue ReplyQueue { get; set; }
}
