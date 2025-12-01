using System.Text.Json.Serialization;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Messages.DbMessages.Receiving;

public class GetLastProcessedDeliusFileResponse : DbResponseMessage
{
    public string? FileName { get; set; }

	public override StatusUpdateMessage StatusMessage => new();

    [JsonConstructor]
    public GetLastProcessedDeliusFileResponse()
    {
        Queue = TDbQueue.ResultLastProcessedDeliusFile;
    }

    public GetLastProcessedDeliusFileResponse(string? fileName) : this()
    {
        FileName = fileName;
    }
}
