using Messaging.Messages.StatusMessages;

namespace Messaging.Tests.TestMessages;

public class TestStatusMessage : StatusUpdateMessage
{
    public required string CustomProperty { get; set; }
    public required DateTime Timestamp { get; set; }
}
