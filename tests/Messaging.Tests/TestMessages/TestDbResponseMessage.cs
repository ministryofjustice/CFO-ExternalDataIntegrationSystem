using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Text.Json.Serialization;

namespace Messaging.Tests.TestMessages;

public class TestDbResponseMessage : DbResponseMessage
{
    public string ResponseData { get; set; } = string.Empty;
    public bool Success { get; set; }

    public override StatusUpdateMessage StatusMessage => new();

    [JsonConstructor]
    public TestDbResponseMessage()
    {
        Queue = TDbQueue.ReturnedDeliusFiles;
    }

    public TestDbResponseMessage(string responseData, bool success) : this()
    {
        ResponseData = responseData;
        Success = success;
    }
}
