using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Sending;

public class GetDeliusFilesMessage : DbRequestMessage
{
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Request sent to retrieve processed delius files.");

    [JsonConstructor]
    public GetDeliusFilesMessage()
    {
        Queue = TDbQueue.GetProcessedDeliusFiles;
        ReplyQueue = TDbQueue.ReturnedDeliusFiles;
    }
}
