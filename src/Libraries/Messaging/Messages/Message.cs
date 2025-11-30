
using Messaging.Messages.StatusMessages;

namespace Messaging.Messages;

public abstract class Message
{
    //Tightly couples each message to a status update as every message signals
    //the completion of the stage.
    public abstract StatusUpdateMessage StatusMessage { get; }
    
    public abstract string Exchange { get; }
    public abstract string RoutingKey { get; }
}
