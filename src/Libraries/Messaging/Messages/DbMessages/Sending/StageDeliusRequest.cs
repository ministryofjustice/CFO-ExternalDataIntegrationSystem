
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Sending;

public class StageDeliusRequest : DbRequestMessage<StageDeliusResponse>
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;

    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Delius Staging started.");

    [JsonConstructor]
    public StageDeliusRequest()
    {
        Queue = TDbQueue.StageDelius;
    }
    
    public StageDeliusRequest(string fileName, string filePath) : this()
    {
        FileName = fileName;
        FilePath = filePath;
    }
}
