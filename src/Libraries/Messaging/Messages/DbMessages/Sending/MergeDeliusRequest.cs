
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Sending;

public class MergeDeliusRequest : DbRequestMessage<MergeDeliusResponse>
{
	public override StatusUpdateMessage StatusMessage => 
		new StatusUpdateMessage("Merging into delius running picture started.");
	public required string FileName { get; set; }
	public override TDbQueue Queue { get; set; } = TDbQueue.MergeDelius;
}
