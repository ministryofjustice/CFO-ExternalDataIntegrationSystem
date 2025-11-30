using System.Text.Json.Serialization;
using Messaging.Messages.BlockingMessages;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.BlockingFinishedMessages;

public class BlockingFinishedMessage : BlockingMessage
{    
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage($"Blocking complete.");

    [JsonConstructor]
    public BlockingFinishedMessage()
    {
        Queue = TBlockingQueue.BlockingFinished;
    }
}