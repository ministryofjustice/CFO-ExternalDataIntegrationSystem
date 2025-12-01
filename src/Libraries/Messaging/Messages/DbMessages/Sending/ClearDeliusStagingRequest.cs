
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StagingMessages;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Sending;

public class ClearDeliusStagingRequest : DbRequestMessage<ClearDeliusStagingResponse>
{
	public override StatusUpdateMessage StatusMessage =>
			new StatusUpdateMessage();
    public override TDbQueue Queue { get; set; } = TDbQueue.ClearDeliusStaging;
}

