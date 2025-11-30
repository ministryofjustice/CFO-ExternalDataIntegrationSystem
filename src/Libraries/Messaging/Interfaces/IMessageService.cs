using Messaging.Messages;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.DbMessages.Sending;

namespace Messaging.Interfaces;

public interface IMessageService
{
    Task PublishAsync<T>(T message) where T : IMessage;
    Task SubscribeAsync<T>(Action<T> handler, Enum queue) where T : IMessage;
    Task DbPublishResponseAsync<T>(T message) where T : DbResponseMessage;  
    Task<T2> SendDbRequestAndWaitForResponseAsync<T, T2>(T message) where T : DbRequestMessage where T2 : DbResponseMessage;
}