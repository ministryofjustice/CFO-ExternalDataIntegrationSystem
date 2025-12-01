using Messaging.Messages.DbMessages.Receiving;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public abstract class DbRequestMessage<TResponse> : DbMessage 
    where TResponse : DbResponseMessage, new()
{
    protected DbRequestMessage()
    {
    }
}
