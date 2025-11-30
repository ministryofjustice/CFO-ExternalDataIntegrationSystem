using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public class DeliusFileProcessingStarted : DbRequestMessage
{
    public override StatusUpdateMessage StatusMessage => new();

    public string FileName { get; set; } = string.Empty;
    public string FileId { get; set; } = string.Empty;

    [JsonConstructor]
    public DeliusFileProcessingStarted()
    {
        Queue = TDbQueue.DeliusFileProcessingStarted;
        ReplyQueue = TDbQueue.ResultDeliusFileProcessingStarted;
    }

    public DeliusFileProcessingStarted(string fileName, string fileId) : this()
    {
        FileName = fileName;
        FileId = fileId;
    }
}