
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Receiving;

public class OfflocFilesReturnMessage : DbResponseMessage
{
    public string[] OfflocFiles { get; set; } = Array.Empty<string>();
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Processed Offloc files returned.");

    [JsonConstructor]
    public OfflocFilesReturnMessage()
    {
        Queue = TDbQueue.ReturnedOfflocFiles;
    }

    public OfflocFilesReturnMessage(string[] offlocFiles) : this()
    {
        OfflocFiles = offlocFiles;
    }
}
