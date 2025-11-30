
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Messages.DbMessages.Sending;

public class MergeDeliusRunningPictureMessage : DbRequestMessage
{
	public override StatusUpdateMessage StatusMessage => 
		new StatusUpdateMessage("Merging into delius running picture started.");

	public string FileName { get; set; } = string.Empty;

	[JsonConstructor]
	public MergeDeliusRunningPictureMessage()
	{
		Queue = TDbQueue.MergeDelius;
		ReplyQueue = TDbQueue.ResultMergeDelius;
	}

	public MergeDeliusRunningPictureMessage(string fileName) : this()
	{
		FileName = fileName;
	}
}
