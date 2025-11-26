using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Messaging.Messages.DbMessages.Sending;

public class StandardiseDeliusMessage : DbRequestMessage
{
    public override StatusUpdateMessage StatusMessage =>
            new StatusUpdateMessage();

    [JsonConstructor]
    public StandardiseDeliusMessage()
    {
        Queue = TDbQueue.StandardiseDelius;
        ReplyQueue = TDbQueue.ResultStandardiseDelius;
    }
}


