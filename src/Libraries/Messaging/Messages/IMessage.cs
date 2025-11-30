namespace Messaging.Messages;

public interface IMessage
{
    public abstract string RoutingKey { get; }
    public abstract string Exchange { get; }
}