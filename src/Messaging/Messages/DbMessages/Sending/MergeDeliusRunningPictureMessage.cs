
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Sending;

public class MergeDeliusRunningPictureMessage : DbRequestMessage
{
	public override StatusUpdateMessage StatusMessage => 
		new StatusUpdateMessage("Merging into delius running picture started.");

	public string fileName { get; set; }

	[JsonConstructor]
	public MergeDeliusRunningPictureMessage(string fileName)
	{
		Queue = TDbQueue.MergeDelius;
		ReplyQueue = TDbQueue.ResultMergeDelius;
		this.fileName = fileName;
	}
}
