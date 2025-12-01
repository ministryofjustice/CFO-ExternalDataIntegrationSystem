
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Sending;

public class MergeDeliusRequest : DbRequestMessage<MergeDeliusResponse>
{
	public override StatusUpdateMessage StatusMessage => 
		new StatusUpdateMessage("Merging into delius running picture started.");

	public string FileName { get; set; } = string.Empty;

	[JsonConstructor]
	public MergeDeliusRequest()
	{
		Queue = TDbQueue.MergeDelius;
	}

	public MergeDeliusRequest(string fileName) : this()
	{
		FileName = fileName;
	}
}
