using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.StagingMessages;

public class DeliusDownloadFinishedMessage : StagingMessage
{
    public override StatusUpdateMessage StatusMessage => new();

    public string FileId { get; set; }

    [JsonConstructor]
    public DeliusDownloadFinishedMessage(string fileName, string fileId)
    {
        Queue = TStagingQueue.DeliusParser;
        base.FileName = fileName;
        FileId = fileId;
    }
    
}
