
using Messaging.Messages.StagingMessages;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Sending;

public class ClearDeliusStaging : DbRequestMessage
{
	public override StatusUpdateMessage StatusMessage =>
			new StatusUpdateMessage();

	[JsonConstructor]
	public  ClearDeliusStaging() 
	{
		Queue = TDbQueue.ClearDeliusStaging;
		ReplyQueue = TDbQueue.ResultClearDelius;
	}
}

