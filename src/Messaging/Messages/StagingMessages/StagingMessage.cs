using Messaging.Queues;

namespace Messaging.Messages.StagingMessages;

public abstract class StagingMessage : Message
{
    public TStagingQueue routingKey { get; set; }
    public string fileName = string.Empty; //To handle processing of multiple files. 
}
