using Messaging.Messages.ImportMessages;
using Messaging.Queues;

namespace Messaging.Interfaces
{
    public interface IImportMessagingService
    {
        Task ImportPublishAsync<T>(T message) where T : ImportMessage;
        Task ImportSubscribeAsync<T>(Action<T> handler, TImportQueue queue) where T : ImportMessage;
    }
}