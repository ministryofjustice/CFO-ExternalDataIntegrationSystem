using Messaging.Messages;

namespace Messaging.Interfaces;

public interface IMessageService : IStatusMessagingService, IDbMessagingService
{
    Task PublishAsync<T>(T message) where T : Message;
    Task SubscribeAsync<T>(Action<T> handler, Enum queue) where T : Message;
}
