
using Messaging.Queues;

namespace Messaging.Messages.MergingMessages;

public abstract class MergingMessage : Message
{
    public TMergingQueue routingKey;
    public string fileName = string.Empty;
}
