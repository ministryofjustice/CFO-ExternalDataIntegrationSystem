using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public class IsOfflocReadyForProcessingMessage : DbRequestMessage
{
    public override StatusUpdateMessage StatusMessage => new("Sent request to check if offloc is ready for processing.");

    [JsonConstructor]
    public IsOfflocReadyForProcessingMessage()
    {
        Queue = TDbQueue.IsOfflocReadyForProcessing;
        ReplyQueue = TDbQueue.IsOfflocReadyForProcessingResult;
    }
}
