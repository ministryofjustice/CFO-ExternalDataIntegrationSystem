using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Messaging.Queues;
using Messaging.Messages.StagingMessages;
using Messaging.Messages.MergingMessages;
using Messaging.Messages.StatusMessages;
using Messaging.Interfaces;
using Messaging.Messages.DbMessages.Sending;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages;
using Messaging.Messages.ImportMessages;
using Messaging.Messages.BlockingMessages;
using Messaging.Messages.MatchingMessages;

namespace Messaging.Services;

public class RabbitService : IMessageService
{
    private IChannel channel;
    private IConnection connection;

    private SemaphoreSlim dbSemaphore = new SemaphoreSlim(1, 1);
    private JsonSerializerOptions serializerOpts = new JsonSerializerOptions { IncludeFields = true };

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

        //Declares separate exchanges for each group of messages and binds queue. 
        TStagingQueue[] stagingQueues = Enum.GetValues<TStagingQueue>();
        TMergingQueue[] mergingQueues = Enum.GetValues<TMergingQueue>();
        TDbQueue[] dbQueues = Enum.GetValues<TDbQueue>();
        TStatusQueue[] statusQueues = Enum.GetValues<TStatusQueue>();
        TImportQueue[] importQueues = Enum.GetValues<TImportQueue>();
        TBlockingQueue[] blockingQueues = Enum.GetValues<TBlockingQueue>();
        TMatchingQueue[] matchingQueues = Enum.GetValues<TMatchingQueue>();

        await channel.ExchangeDeclareAsync(Exchanges.staging, ExchangeType.Direct);
        await channel.ExchangeDeclareAsync(Exchanges.merging, ExchangeType.Direct);
        await channel.ExchangeDeclareAsync(Exchanges.database, ExchangeType.Direct); 
        await channel.ExchangeDeclareAsync(Exchanges.status, ExchangeType.Direct);
        await channel.ExchangeDeclareAsync(Exchanges.import, ExchangeType.Direct);
        await channel.ExchangeDeclareAsync(Exchanges.blocking, ExchangeType.Direct);
        await channel.ExchangeDeclareAsync(Exchanges.matching, ExchangeType.Direct);

        await InitializeQueueAsync(stagingQueues, Exchanges.staging);
        await InitializeQueueAsync(mergingQueues, Exchanges.merging);
        await InitializeQueueAsync(dbQueues, Exchanges.database);
        await InitializeQueueAsync(statusQueues, Exchanges.status);
        await InitializeQueueAsync(importQueues, Exchanges.import);
        await InitializeQueueAsync(blockingQueues, Exchanges.blocking);
        await InitializeQueueAsync(matchingQueues, Exchanges.matching);
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

    public async Task StatusPublishAsync<T>(T message) where T : StatusUpdateMessage
    {
        await channel.BasicPublishAsync(
            exchange: Exchanges.status,
            routingKey: message.RoutingKey.ToString(),
            mandatory: false,
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, serializerOpts))
            );
    }

    public async Task StatusSubscribeAsync<T>(Action<T> handler, TStatusQueue queue) where T : StatusUpdateMessage
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        await channel.BasicConsumeAsync(queue.ToString(), true, consumer);

        consumer.ReceivedAsync += (model, ea) => { handler.Invoke(JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(ea.Body.ToArray()), new JsonSerializerOptions { IncludeFields = true })!); return Task.CompletedTask; };
    }

    public async Task StagingPublishAsync<T>(T message) where T : StagingMessage
    {
        await channel.BasicPublishAsync(
            exchange: Exchanges.staging,
            routingKey: message.routingKey.ToString(),
            mandatory: false,
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, serializerOpts))
            );
        await AssociatedMessagePublishAsync(message);
    }

    public async Task StagingSubscribeAsync<T>(Action<T> handler, TStagingQueue queue) where T : StagingMessage
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        await channel.BasicConsumeAsync(queue.ToString(), true, consumer);

        consumer.ReceivedAsync += (model, ea) => { handler.Invoke(JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(ea.Body.ToArray()), new JsonSerializerOptions { IncludeFields = true })!); return Task.CompletedTask; };
    }

    private async Task DbPublishRequestAsync<T>(T message) where T : DbRequestMessage
    {
        var props = new BasicProperties
        {
            ReplyTo = message.ReplyQueue.ToString()
        };
         
        await channel.BasicPublishAsync(
            exchange: Exchanges.database,
            routingKey: message.Queue.ToString(),
            mandatory: false,
            basicProperties: props,
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, serializerOpts))
            );

		await AssociatedMessagePublishAsync(message);
	}

    public async Task DbPublishResponseAsync<T>(T message) where T : DbResponseMessage
    {
        await channel.BasicPublishAsync(
            exchange: Exchanges.database,
            routingKey: message.Queue.ToString(),
            mandatory: false,
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, serializerOpts))
            );

		await AssociatedMessagePublishAsync(message);
	}

    public async Task SubscribeToDbRequestAsync<T>(Action<T> handler, TDbQueue queue) where T : DbRequestMessage
    {        
        var consumer = new AsyncEventingBasicConsumer(channel);
        await channel.BasicConsumeAsync(queue.ToString(), true, consumer);

        consumer.ReceivedAsync += (model, ea) =>
        {
            var msg = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(ea.Body.ToArray()), serializerOpts)!;
            handler.Invoke(msg);
            return Task.CompletedTask;
        };
    }

    public async Task<TResponse> SendDbRequestAndWaitForResponseAsync<TRequest, TResponse>(TRequest message) 
        where TRequest : DbRequestMessage 
        where TResponse : DbResponseMessage 
    {
        await dbSemaphore.WaitAsync();

        var consumer = new AsyncEventingBasicConsumer(channel);
        string tempConsumer = await channel.BasicConsumeAsync(message.ReplyQueue.ToString(), true, consumer);

        var taskCompletion = new TaskCompletionSource<TResponse>(TaskCreationOptions.RunContinuationsAsynchronously);

        AsyncEventHandler<BasicDeliverEventArgs> eventHandler = (model, ea) =>
        {
            DbMessageReceivedCallback(model, ea);
            return Task.CompletedTask;
        };

        consumer.ReceivedAsync += eventHandler;

        try
        {
            await DbPublishRequestAsync(message);
            return await taskCompletion.Task;
        }
        finally
        {
            consumer.ReceivedAsync -= eventHandler;
            await channel.BasicCancelAsync(tempConsumer);
            dbSemaphore.Release();
        }

        void DbMessageReceivedCallback(object? model, BasicDeliverEventArgs ea)
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
        }
    }

    public async Task MergingPublishAsync<T>(T message) where T : MergingMessage
    {
        await channel.BasicPublishAsync(
            exchange: Exchanges.merging,
            routingKey: message.routingKey.ToString(),
            mandatory: false,
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, serializerOpts))
        );

		await AssociatedMessagePublishAsync(message);
	}

    public async Task MergingSubscribeAsync<T>(Action<T> handler, TMergingQueue queue) where T : MergingMessage
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        await channel.BasicConsumeAsync(queue.ToString(), true, consumer);

        consumer.ReceivedAsync += (model, ea) =>
        {
            handler.Invoke(JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(ea.Body.ToArray()), new JsonSerializerOptions { IncludeFields = true })!);
            return Task.CompletedTask;
        };
    }

    public async Task ImportPublishAsync<T>(T message) where T : ImportMessage
    {
        await channel.BasicPublishAsync(
            exchange: Exchanges.import,
            routingKey: message.routingKey.ToString(),
            mandatory: false,
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, serializerOpts))
            );
        await AssociatedMessagePublishAsync(message);
    }

    public async Task ImportSubscribeAsync<T>(Action<T> handler, TImportQueue queue) where T : ImportMessage
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        await channel.BasicConsumeAsync(queue.ToString(), true, consumer);

        consumer.ReceivedAsync += (model, ea) =>
        {
            handler.Invoke(JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(ea.Body.ToArray()), new JsonSerializerOptions { IncludeFields = true })!);
            return Task.CompletedTask;
        };
    }

    public async Task BlockingPublishAsync<T>(T message) where T : BlockingMessage
    {
        await channel.BasicPublishAsync(
            exchange: Exchanges.blocking,
            routingKey: message.routingKey.ToString(),
            mandatory: false,
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, serializerOpts))
            );
        await AssociatedMessagePublishAsync(message);
    }

    public async Task BlockingSubscribeAsync<T>(Action<T> handler, TBlockingQueue queue) where T : BlockingMessage
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        await channel.BasicConsumeAsync(queue.ToString(), true, consumer);

        consumer.ReceivedAsync += (model, ea) =>
        {
            handler.Invoke(JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(ea.Body.ToArray()), new JsonSerializerOptions { IncludeFields = true })!);
            return Task.CompletedTask;
        };
    }

    private async Task AssociatedMessagePublishAsync<T>(T message) where T : Message
    {
        if (message.StatusMessage.Message != string.Empty)
        {
            await StatusPublishAsync(message.StatusMessage);
        }
    }

    public async Task MatchingPublishAsync<T>(T message) where T : MatchingMessage
    {
        await channel.BasicPublishAsync(
            exchange: Exchanges.matching,
            routingKey: message.routingKey.ToString(),
            mandatory: false,
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, serializerOpts))
            );
        await AssociatedMessagePublishAsync(message);
    }

    public async Task MatchingSubscribeAsync<T>(Action<T> handler, TMatchingQueue queue) where T : MatchingMessage
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        await channel.BasicConsumeAsync(queue.ToString(), true, consumer);

        consumer.ReceivedAsync += (model, ea) =>
        {
            handler.Invoke(JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(ea.Body.ToArray()), new JsonSerializerOptions { IncludeFields = true })!);
            return Task.CompletedTask;
        };
    }
}