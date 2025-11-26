
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Sending;

public class GetOfflocFileDatesMessage : DbRequestMessage
{
    public override StatusUpdateMessage StatusMessage =>
        new StatusUpdateMessage("Request for processed offloc files sent.");

    [JsonConstructor]
    public GetOfflocFileDatesMessage()
    {
        Queue = TDbQueue.GetOfflocFileDates;
        ReplyQueue = TDbQueue.ReturnedOfflocFileDates;
    }
}
