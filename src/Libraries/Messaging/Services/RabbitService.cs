using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Messaging.Queues;
using Messaging.Interfaces;
using Messaging.Messages.DbMessages.Sending;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages;

namespace Messaging.Services;

public class RabbitService : IMessageService
{
    private IChannel channel;
    private IConnection connection;

    private SemaphoreSlim dbSemaphore = new SemaphoreSlim(1, 1);
    private JsonSerializerOptions serializerOpts = new JsonSerializerOptions() { IncludeFields = true };

    private RabbitService()
    {
    }

    public static async Task<RabbitService> CreateAsync(Uri connectionUri)
    {
        var service = new RabbitService();
        await service.InitializeAsync(connectionUri);
        return service;
    }

    private async Task InitializeAsync(Uri connectionUri)
    {
        var factory = new ConnectionFactory() { Uri = connectionUri };

        connection = await factory.CreateConnectionAsync();
        channel = await connection.CreateChannelAsync();

        string[] exchanges = [Exchanges.staging, Exchanges.merging, Exchanges.database, 
            Exchanges.status, Exchanges.import, Exchanges.blocking, Exchanges.matching];

        foreach (var exchange in exchanges)
        {
            await channel.ExchangeDeclareAsync(exchange, ExchangeType.Direct);
        }

        await InitializeQueueAsync(Enum.GetValues<TStagingQueue>(), Exchanges.staging);
        await InitializeQueueAsync(Enum.GetValues<TMergingQueue>(), Exchanges.merging);
        await InitializeQueueAsync(Enum.GetValues<TDbQueue>(), Exchanges.database);
        await InitializeQueueAsync(Enum.GetValues<TStatusQueue>(), Exchanges.status);
        await InitializeQueueAsync(Enum.GetValues<TImportQueue>(), Exchanges.import);
        await InitializeQueueAsync(Enum.GetValues<TBlockingQueue>(), Exchanges.blocking);
        await InitializeQueueAsync(Enum.GetValues<TMatchingQueue>(), Exchanges.matching);
    }

    private async Task InitializeQueueAsync<T>(T[] queues, string exchange) where T : Enum
    {
        foreach (var queue in queues)
        {
            await channel.QueueDeclareAsync(
                queue: queue.ToString(),
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            await channel.QueueBindAsync(queue.ToString(), exchange, queue.ToString());
        }
    }

    public async Task PublishAsync<T>(T message) where T : IMessage
    {
        await channel.BasicPublishAsync(
            exchange: message.Exchange,
            routingKey: message.RoutingKey,
            mandatory: false,
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, message!.GetType(), serializerOpts))
        );

        await PublishStatusAsync(message);
    }
    
    public async Task SubscribeAsync<T>(Action<T> handler, Enum queue) where T : IMessage
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        await channel.BasicConsumeAsync(queue.ToString(), true, consumer);

        consumer.ReceivedAsync += (model, ea) =>
        {
            handler.Invoke(JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(ea.Body.ToArray()), serializerOpts)!);
            return Task.CompletedTask;
        };
    }

    public async Task<TResponse> SendDbRequestAndWaitForResponseAsync<TResponse>(DbRequestMessage<TResponse> message) 
        where TResponse : DbResponseMessage, new() 
    {
        await dbSemaphore.WaitAsync();
        var responseQueue = new TResponse().Queue.ToString();
        var taskCompletion = new TaskCompletionSource<TResponse>(TaskCreationOptions.RunContinuationsAsynchronously);

        // Subscribe to response queue temporarily
        var consumer = new AsyncEventingBasicConsumer(channel);
        
        consumer.ReceivedAsync += DbMessageReceivedCallback;
        string consumerTag = await channel.BasicConsumeAsync(responseQueue, true, consumer);

        try
        {
            await DbPublishRequestAsync(message, responseQueue);
            return await taskCompletion.Task;
        }
        finally
        {
            consumer.ReceivedAsync -= DbMessageReceivedCallback;
            await channel.BasicCancelAsync(consumerTag);
            dbSemaphore.Release();
        }

        Task DbMessageReceivedCallback(object? model, BasicDeliverEventArgs ea)
        {
            try
            {
                var response = JsonSerializer.Deserialize<TResponse>(Encoding.UTF8.GetString(ea.Body.ToArray()), serializerOpts)!;
                taskCompletion.SetResult(response);
            }
            catch(Exception ex)
            {
                taskCompletion.SetException(ex);
            }
            return Task.CompletedTask;
        }
    }

    private async Task DbPublishRequestAsync<TResponse>(DbRequestMessage<TResponse> message, string replyQueue) where TResponse : DbResponseMessage, new()
    {
        var props = new BasicProperties
        {
            ReplyTo = replyQueue
        };

        var json = JsonSerializer.Serialize(message, message.GetType(), serializerOpts);
         
        await channel.BasicPublishAsync(
            exchange: Exchanges.database,
            routingKey: message.Queue.ToString(),
            mandatory: false,
            basicProperties: props,
            body: Encoding.UTF8.GetBytes(json));

		await PublishStatusAsync(message);
	}

    private async Task PublishStatusAsync<T>(T message) where T : IMessage
    {
        if (message is Message { StatusMessage.Message: not "" } status)
        {
            await PublishAsync(status.StatusMessage);
        }
    }

}