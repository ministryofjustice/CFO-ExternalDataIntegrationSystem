using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public class DeliusFileProcessingStarted : DbRequestMessage
{
    public override StatusUpdateMessage StatusMessage => new();

    public string fileName = string.Empty;
    public string fileId = string.Empty;

    [JsonConstructor]
    public DeliusFileProcessingStarted()
    {
        Queue = TDbQueue.DeliusFileProcessingStarted;
        ReplyQueue = TDbQueue.ResultDeliusFileProcessingStarted;
    }

    public DeliusFileProcessingStarted(string fileName, string fileId) : this()
    {
        this.fileName = fileName;
        this.fileId = fileId;
    }
}