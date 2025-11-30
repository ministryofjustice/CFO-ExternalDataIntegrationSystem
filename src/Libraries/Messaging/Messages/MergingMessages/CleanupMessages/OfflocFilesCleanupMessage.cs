
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.MergingMessages.CleanupMessages;

public class OfflocFilesCleanupMessage : MergingMessage
{
    public override StatusUpdateMessage StatusMessage => 
        new StatusUpdateMessage($"Cleanup started for offloc file {FileName}");

    [JsonConstructor]
    public OfflocFilesCleanupMessage()
    {
        Queue = TMergingQueue.OfflocFilesCleanupQueue;
    }

    public OfflocFilesCleanupMessage(string fileName) : this()
    {
        FileName = fileName;
    }
}
