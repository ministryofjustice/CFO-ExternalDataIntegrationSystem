using Messaging.Messages.ImportMessages;
using Messaging.Queues;

namespace Messaging.Interfaces
{
    public interface IImportMessagingService
    {
        void ImportPublish<T>(T message) where T : ImportMessage;
        void ImportSubscribe<T>(Action<T> handler, TImportQueue queue) where T : ImportMessage;
    }
}