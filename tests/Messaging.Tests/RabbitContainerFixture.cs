using Testcontainers.RabbitMq;
using Testcontainers.Xunit;
using Xunit.Abstractions;

namespace Messaging.Tests;

public sealed class RabbitContainerFixture(IMessageSink messageSink) 
    : ContainerFixture<RabbitMqBuilder, RabbitMqContainer>(messageSink)
{
    protected override RabbitMqBuilder Configure(RabbitMqBuilder builder) => builder.WithImage("rabbitmq:3");
}