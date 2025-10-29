
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.MergingMessages.CleanupMessages;

public class DeliusFilesCleanupMessage : MergingMessage
{
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage($"File cleanup started for delius file {fileName}");

    [JsonConstructor]
    public DeliusFilesCleanupMessage()
    {
        routingKey = TMergingQueue.DeliusFilesCleanupQueue;
    }

    public DeliusFilesCleanupMessage(string fileName) : this()
    {
        this.fileName = fileName;
    }
}
