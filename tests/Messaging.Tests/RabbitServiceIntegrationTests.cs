using Messaging.Messages.StatusMessages;
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
    public async Task PublishAndSubscribe_SerializesAndDeserializesMessage_Correctly()
    {
        // Arrange
        var completionSource = new TaskCompletionSource<TestMessage>();

        await _rabbitService!.SubscribeAsync<TestMessage>(
            completionSource.SetResult,
            TMatchingQueue.ClusteringPostProcessingFinished
        );

        var publishedMessage = new TestMessage(TMatchingQueue.ClusteringPostProcessingFinished.ToString(), Exchanges.matching)
        {
            CustomProperty = "Test Value"
        };

        // Act
        await _rabbitService.PublishAsync(publishedMessage);

        var receivedMessage = await completionSource.Task.WaitAsync(TimeSpan.FromSeconds(5));

        // Assert
        Assert.NotNull(receivedMessage);
        Assert.Equal(publishedMessage.CustomProperty, receivedMessage.CustomProperty);
        Assert.Equal(publishedMessage.RoutingKey, receivedMessage.RoutingKey);
    }

    [Fact]
    public async Task PublishAsync_WithStatusMessageSet_PublishesStatusMessage()
    {
        // Arrange
        var messageCompletionSource = new TaskCompletionSource<TestMessage>();
        var statusCompletionSource = new TaskCompletionSource<StatusUpdateMessage>();

        await _rabbitService!.SubscribeAsync<TestMessage>(
            messageCompletionSource.SetResult,
            TStagingQueue.DeliusCleanup
        );

        await _rabbitService.SubscribeAsync<StatusUpdateMessage>(
            statusCompletionSource.SetResult,
            TStatusQueue.StatusUpdate
        );

        var publishedMessage = new TestMessage(TStagingQueue.DeliusCleanup.ToString(), Exchanges.staging)
        {
            CustomProperty = "Test Value",
            Status = "Test completed successfully"
        };

        // Act
        await _rabbitService.PublishAsync(publishedMessage);

        var receivedMessage = await messageCompletionSource.Task.WaitAsync(TimeSpan.FromSeconds(5));
        var receivedStatus = await statusCompletionSource.Task.WaitAsync(TimeSpan.FromSeconds(5));

        // Assert
        Assert.NotNull(receivedMessage);
        Assert.Equal("Test Value", receivedMessage.CustomProperty);
        
        Assert.NotNull(receivedStatus);
        Assert.Equal("Test completed successfully", receivedStatus.Message);
    }

    [Fact]
    public async Task PublishAsync_WithoutStatusMessageSet_DoesNotPublishStatusMessage()
    {
        // Arrange
        var messageCompletionSource = new TaskCompletionSource<TestMessage>();
        var statusCompletionSource = new TaskCompletionSource<StatusUpdateMessage>();

        await _rabbitService!.SubscribeAsync<TestMessage>(
            messageCompletionSource.SetResult,
            TImportQueue.ImportFinished
        );

        await _rabbitService.SubscribeAsync<StatusUpdateMessage>(
            statusCompletionSource.SetResult,
            TStatusQueue.StatusUpdate
        );

        var publishedMessage = new TestMessage(TImportQueue.ImportFinished.ToString(), Exchanges.import)
        {
            CustomProperty = "Test Value"
            // Status is not set (null or empty string)
        };

        // Act
        await _rabbitService.PublishAsync(publishedMessage);

        var receivedMessage = await messageCompletionSource.Task.WaitAsync(TimeSpan.FromSeconds(5));

        // Assert - message should be received
        Assert.NotNull(receivedMessage);
        Assert.Equal("Test Value", receivedMessage.CustomProperty);

        // Assert - status message should NOT be received (timeout should occur)
        await Assert.ThrowsAsync<TimeoutException>(async () =>
        {
            await statusCompletionSource.Task.WaitAsync(TimeSpan.FromSeconds(2));
        });
    }


    [Fact]
    public async Task SendDbRequestAndWaitForResponseAsync_SendsRequestAndReceivesResponse_Successfully()
    {
        // Arrange
        var requestData = "RequestData";
        var expectedResponseData = $"Echo: {requestData}";
        await _rabbitService!.SubscribeAsync<TestDbRequestMessage>(
            msg =>
            {
                Task.Run(async () =>
                {
                    await Task.Delay(250); // Simulate processing delay
                    var response = new TestDbResponseMessage($"Echo: {msg.RequestData}", true);
                    await _rabbitService.PublishAsync(response);
                });
            },
            TDbQueue.GetProcessedDeliusFiles
        );
        // Act
        var response = await _rabbitService.SendDbRequestAndWaitForResponseAsync(new TestDbRequestMessage(requestData));

        // Assert
        Assert.Equal(expectedResponseData, response.ResponseData);
        Assert.True(response.Success);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}