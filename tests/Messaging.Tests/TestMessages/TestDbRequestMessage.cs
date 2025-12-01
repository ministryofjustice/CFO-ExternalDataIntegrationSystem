using Messaging.Messages.DbMessages.Sending;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Tests.TestMessages;

public class TestDbRequestMessage : DbRequestMessage<TestDbResponseMessage>
{
    public string RequestData { get; set; } = string.Empty;

    public override StatusUpdateMessage StatusMessage => new();

    [JsonConstructor]
    public TestDbRequestMessage()
    {
        Queue = TDbQueue.GetProcessedDeliusFiles;
    }

    public TestDbRequestMessage(string requestData) : this()
    {
        RequestData = requestData;
    }
}
