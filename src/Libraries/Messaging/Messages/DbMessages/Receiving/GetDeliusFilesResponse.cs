using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Receiving;

public class GetDeliusFilesResponse : DbResponseMessage
{
    public string[] FileNames { get; set; } = [];
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Processed Delius files returned.");
    public override TDbQueue Queue { get; set; } = TDbQueue.ReturnedDeliusFiles;
}