using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Receiving;

public class GetDeliusFilesResponse : DbResponseMessage
{
    public string[] FileNames { get; set; } = Array.Empty<string>();
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Processed Delius files returned.");

    [JsonConstructor]
    public GetDeliusFilesResponse()
    { }

    public GetDeliusFilesResponse(string[] fileNames)
    {
        FileNames = fileNames;
        Queue = TDbQueue.ReturnedDeliusFiles;
    }
}