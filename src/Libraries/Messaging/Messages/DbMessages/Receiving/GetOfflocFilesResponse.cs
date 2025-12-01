
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Receiving;

public class GetOfflocFilesResponse : DbResponseMessage
{
    public string[] OfflocFiles { get; set; } = Array.Empty<string>();
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Processed Offloc files returned.");

    [JsonConstructor]
    public GetOfflocFilesResponse()
    {
        Queue = TDbQueue.ReturnedOfflocFiles;
    }

    public GetOfflocFilesResponse(string[] offlocFiles) : this()
    {
        OfflocFiles = offlocFiles;
    }
}
