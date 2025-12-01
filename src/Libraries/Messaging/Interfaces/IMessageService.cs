using Messaging.Messages;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.DbMessages.Sending;

namespace Messaging.Interfaces;

public interface IMessageService
{
    Task PublishAsync<T>(T message) where T : IMessage;
    Task SubscribeAsync<T>(Action<T> handler, Enum queue) where T : IMessage;
    Task<TResponse> SendDbRequestAndWaitForResponseAsync<TResponse>(DbRequestMessage<TResponse> message) where TResponse : DbResponseMessage, new();
}