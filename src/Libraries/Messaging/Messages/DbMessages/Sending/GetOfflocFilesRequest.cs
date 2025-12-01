using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Sending;

public class GetOfflocFilesRequest : DbRequestMessage<GetOfflocFilesResponse>
{
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Sent request to get processed offloc files.");
    public override TDbQueue Queue { get; set; } = TDbQueue.GetProcessedOfflocFiles;
}
