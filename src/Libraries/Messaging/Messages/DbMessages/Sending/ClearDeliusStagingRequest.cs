
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

	[JsonConstructor]
	public ClearDeliusStagingRequest() 
	{
		Queue = TDbQueue.ClearDeliusStaging;
	}
}

