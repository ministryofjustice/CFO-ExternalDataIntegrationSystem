using System.Text.Json.Serialization;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Sending;

public class StartDeliusFileProcessingRequest : DbRequestMessage<StartDeliusFileProcessingResponse>
{
    public override StatusUpdateMessage StatusMessage => new();

    public string FileName { get; set; } = string.Empty;
    public string FileId { get; set; } = string.Empty;

    [JsonConstructor]
    public StartDeliusFileProcessingRequest()
    {
        Queue = TDbQueue.DeliusFileProcessingStarted;
    }

    public StartDeliusFileProcessingRequest(string fileName, string fileId) : this()
    {
        FileName = fileName;
        FileId = fileId;
    }
}