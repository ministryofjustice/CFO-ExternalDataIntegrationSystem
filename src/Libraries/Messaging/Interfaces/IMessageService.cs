using Messaging.Messages;

namespace Messaging.Interfaces;

public interface IMessageService : IDbMessagingService
{
    Task PublishAsync<T>(T message) where T : IMessage;
    Task SubscribeAsync<T>(Action<T> handler, Enum queue) where T : IMessage;
}
