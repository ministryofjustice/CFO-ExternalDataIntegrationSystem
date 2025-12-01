using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Sending;

public class GetDeliusFilesRequest : DbRequestMessage<GetDeliusFilesResponse>
{
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Request sent to retrieve processed delius files.");
    public override TDbQueue Queue { get; set; } = TDbQueue.GetProcessedDeliusFiles;
}
