using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.DbMessages.Sending;
using Messaging.Queues;

namespace Messaging.Interfaces;

public interface IDbMessagingService
{
    void DbPublishResponse<T>(T message) where T : DbResponseMessage;  
    void SubscribeToDbRequest<T>(Action<T> handler, TDbQueue queue) where T : DbRequestMessage;
    Task<T2> SendDbRequestAndWaitForResponse<T, T2>(T message) where T : DbRequestMessage where T2 : DbResponseMessage;
    
    //void DbUnsubscribe<T>(Action<T> handler, TDbQueue queue) where T : DbRequestMessage;
}
