using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.DbMessages.Sending;
using Messaging.Queues;

namespace Messaging.Interfaces;

public interface IDbMessagingService
{
    Task DbPublishResponseAsync<T>(T message) where T : DbResponseMessage;  
    // Task SubscribeToDbRequestAsync<T>(Action<T> handler, TDbQueue queue) where T : DbRequestMessage;
    Task<T2> SendDbRequestAndWaitForResponseAsync<T, T2>(T message) where T : DbRequestMessage where T2 : DbResponseMessage;
}
