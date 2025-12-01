
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Receiving;

public class GetOfflocFilesResponse : DbResponseMessage
{
    public string[] OfflocFiles { get; set; } = [];
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Processed Offloc files returned.");
    public override TDbQueue Queue { get; set; } = TDbQueue.ReturnedOfflocFiles;
}
