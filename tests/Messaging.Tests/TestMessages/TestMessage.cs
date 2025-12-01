
using Messaging.Messages;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Messaging.Tests.TestMessages;

public class TestMessage(string routingKey, string exchange) : Message
{
    public required string CustomProperty { get; set; }
    public string? Status { get; set; }

    public override StatusUpdateMessage StatusMessage => new(Status ?? string.Empty);

    public override string Exchange => exchange;

    public override string RoutingKey => routingKey;
}