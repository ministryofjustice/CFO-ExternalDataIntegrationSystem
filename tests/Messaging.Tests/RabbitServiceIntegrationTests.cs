using Messaging.Queues;
using Messaging.Services;
using Messaging.Tests.TestMessages;

namespace Messaging.Tests;

public class RabbitServiceIntegrationTests(RabbitContainerFixture fixture) : IAsyncLifetime, IClassFixture<RabbitContainerFixture>
{
    private RabbitService? _rabbitService;

    public async Task InitializeAsync()
    {
        var connectionString = new Uri(fixture.Container.GetConnectionString());
        _rabbitService = await RabbitService.CreateAsync(connectionString);
    }

    [Fact]
    public async Task StatusPublishAndSubscribe_SerializesAndDeserializesMessage_Correctly()
    {
        // Arrange
        var completionSource = new TaskCompletionSource<TestStatusMessage>();
        var expectedMessage = "Status update message";
        var expectedCustomProperty = "Custom Value";
        var expectedTimestamp = DateTime.UtcNow;

        // Act
        await _rabbitService!.StatusSubscribeAsync<TestStatusMessage>(
            completionSource.SetResult,
            TStatusQueue.StatusUpdate
        );

        var publishedMessage = new TestStatusMessage()
        {
            Message = expectedMessage,
            CustomProperty = expectedCustomProperty,
            Timestamp = expectedTimestamp
        };

        await _rabbitService.StatusPublishAsync(publishedMessage);

        var receivedMessage = await completionSource.Task.WaitAsync(TimeSpan.FromSeconds(5));

        // Assert
        Assert.NotNull(receivedMessage);
        Assert.Equal(publishedMessage.Message, receivedMessage.Message);
        Assert.Equal(publishedMessage.CustomProperty, receivedMessage.CustomProperty);
        Assert.Equal(publishedMessage.Timestamp, receivedMessage.Timestamp);
        Assert.Equal(publishedMessage.RoutingKey, receivedMessage.RoutingKey);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}