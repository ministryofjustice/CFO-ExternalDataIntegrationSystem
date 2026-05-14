
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.StagingMessages.Offloc;

public class ClearTemporaryOfflocFiles : StagingMessage
{
    public override StatusUpdateMessage StatusMessage => 
        new StatusUpdateMessage();

    [JsonConstructor]
    public ClearTemporaryOfflocFiles()
    {
        Queue = TStagingQueue.OfflocFilesClear;
    }
}
