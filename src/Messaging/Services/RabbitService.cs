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
using EnvironmentSetup;
using Messaging.Messages;
using Messaging.Messages.ImportMessages;
using Messaging.Messages.BlockingMessages;
using Messaging.Messages.MatchingMessages;

namespace Messaging.Services;

public class RabbitService : IMessageService
{
    //Might not need to be a field.
    private RabbitHostingContextWrapper rabbitContext; 

    private ConnectionFactory factory;
    private IModel channel;
    private IConnection connection;

    private SemaphoreSlim dbSemaphore = new SemaphoreSlim(1, 1);
    private JsonSerializerOptions serializerOpts = new JsonSerializerOptions { IncludeFields = true };

    public RabbitService(RabbitHostingContextWrapper hostingContext)
    {
        this.rabbitContext = hostingContext;

        factory = new ConnectionFactory()
        {
            HostName = rabbitContext.Context,
            UserName = rabbitContext.Username,
            Password = rabbitContext.Password
        };

        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        //Declares separate exchanges for each group of messages and binds queue. 
        TStagingQueue[] stagingQueues = Enum.GetValues<TStagingQueue>();
        TMergingQueue[] mergingQueues = Enum.GetValues<TMergingQueue>();
        TDbQueue[] dbQueues = Enum.GetValues<TDbQueue>();
        TStatusQueue[] statusQueues = Enum.GetValues<TStatusQueue>();
        TImportQueue[] importQueues = Enum.GetValues<TImportQueue>();
        TBlockingQueue[] blockingQueues = Enum.GetValues<TBlockingQueue>();
        TMatchingQueue[] matchingQueues = Enum.GetValues<TMatchingQueue>();

        channel.ExchangeDeclare(Exchanges.staging, ExchangeType.Direct);
        channel.ExchangeDeclare(Exchanges.merging, ExchangeType.Direct);
        channel.ExchangeDeclare(Exchanges.database, ExchangeType.Direct); 
        channel.ExchangeDeclare(Exchanges.status, ExchangeType.Direct);
        channel.ExchangeDeclare(Exchanges.import, ExchangeType.Direct);
        channel.ExchangeDeclare(Exchanges.blocking, ExchangeType.Direct);
        channel.ExchangeDeclare(Exchanges.matching, ExchangeType.Direct);

        InitializeQueue(stagingQueues, Exchanges.staging);
        InitializeQueue(mergingQueues, Exchanges.merging);
        InitializeQueue(dbQueues, Exchanges.database);
        InitializeQueue(statusQueues, Exchanges.status);
        InitializeQueue(importQueues, Exchanges.import);
        InitializeQueue(blockingQueues, Exchanges.blocking);
        InitializeQueue(matchingQueues, Exchanges.matching);
    }

    //Avoids code repetition in constructor.
    private void InitializeQueue<T>(T[] queues, string exchange) where T : Enum
    {
        foreach (var queue in queues)
        {
            channel.QueueDeclare(
                queue: queue.ToString(),
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            channel.QueueBind(queue.ToString(), exchange, queue.ToString());
        }
    }

    public void StatusPublish<T>(T message) where T : StatusUpdateMessage
    {
        channel.BasicPublish(
            exchange: Exchanges.status,
            routingKey: message.RoutingKey.ToString(),
            basicProperties: null,
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, serializerOpts))
            );
    }

    public void StatusSubscribe<T>(Action<T> handler, TStatusQueue queue) where T : StatusUpdateMessage
    {
        var consumer = new EventingBasicConsumer(channel);
        channel.BasicConsume(queue.ToString(), true, consumer);

        consumer.Received += (model, ea) => handler.Invoke(JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(ea.Body.ToArray()), new JsonSerializerOptions { IncludeFields = true })!);
    }

    public void StagingPublish<T>(T message) where T : StagingMessage
    {
        channel.BasicPublish(
            exchange: Exchanges.staging,
            routingKey: message.routingKey.ToString(),
            basicProperties: null,
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, serializerOpts))
            );
        AssociatedMessagePublish(message);
    }

    public void StagingSubscribe<T>(Action<T> handler, TStagingQueue queue) where T : StagingMessage
    {
        var consumer = new EventingBasicConsumer(channel);
        channel.BasicConsume(queue.ToString(), true, consumer);

        consumer.Received += (model, ea) => handler.Invoke(JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(ea.Body.ToArray()), new JsonSerializerOptions { IncludeFields = true })!);
    }

    //The IDbInteractionService needs to listen to where these messages are published to.
    private void DbPublishRequest<T>(T message) where T : DbRequestMessage
    {
        //RPC pattern.
        IBasicProperties props = channel.CreateBasicProperties();
        props.ReplyTo = message.ReplyQueue.ToString(); 
         
        channel.BasicPublish(
            exchange: Exchanges.database,
            routingKey: message.Queue.ToString(),
            basicProperties: props,
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, serializerOpts))
            );

		AssociatedMessagePublish(message);
	}

    //The client needs to listen to where these messages are published to.
    public void DbPublishResponse<T>(T message) where T : DbResponseMessage
    {
        channel.BasicPublish(
            exchange: Exchanges.database,
            routingKey: message.Queue.ToString(),
            basicProperties: null,
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, serializerOpts))
            );

		AssociatedMessagePublish(message);
	}

    //Subscriptions when waiting for requests are long-lived.
    public void DbLongSubscribe<T>(Action<T> handler, TDbQueue queue) where T : DbRequestMessage
    {        
        var consumer = new EventingBasicConsumer(channel);
        channel.BasicConsume(queue.ToString(), true, consumer);

        consumer.Received += (model, ea) =>
        {
            var msg = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(ea.Body.ToArray()), serializerOpts)!;
            handler.Invoke(msg);
        };
    }

    public async Task<TResponse> DbTransientSubscribe<TRequest, TResponse>(TRequest message) 
        where TRequest : DbRequestMessage 
        where TResponse : DbResponseMessage 
    {
        await dbSemaphore.WaitAsync();

        var consumer = new EventingBasicConsumer(channel);
        string tempConsumer = channel.BasicConsume(message.ReplyQueue.ToString(), true, consumer);

        var taskCompletion = new TaskCompletionSource<TResponse>(TaskCreationOptions.RunContinuationsAsynchronously);

        consumer.Received += DbMessageReceivedCallback;

        try
        {
            DbPublishRequest(message);
            return await taskCompletion.Task;
        }
        finally
        {
            consumer.Received -= DbMessageReceivedCallback;
            channel.BasicCancel(tempConsumer);
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

    public void MergingPublish<T>(T message) where T : MergingMessage
    {
        channel.BasicPublish(
            exchange: Exchanges.merging,
            routingKey: message.routingKey.ToString(),
            basicProperties: null,
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, serializerOpts))
        );

		AssociatedMessagePublish(message);
	}

    public void MergingSubscribe<T>(Action<T> handler, TMergingQueue queue) where T : MergingMessage
    {
        var consumer = new EventingBasicConsumer(channel);
        channel.BasicConsume(queue.ToString(), true, consumer);

        consumer.Received += (model, ea) => 
        handler.Invoke(JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(ea.Body.ToArray()), new JsonSerializerOptions { IncludeFields = true })!
        );
    }

    public void ImportPublish<T>(T message) where T : ImportMessage
    {
        channel.BasicPublish(
            exchange: Exchanges.import,
            routingKey: message.routingKey.ToString(),
            basicProperties: null,
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, serializerOpts))
            );
        AssociatedMessagePublish(message);
    }

    public void ImportSubscribe<T>(Action<T> handler, TImportQueue queue) where T : ImportMessage
    {
        var consumer = new EventingBasicConsumer(channel);
        channel.BasicConsume(queue.ToString(), true, consumer);

        consumer.Received += (model, ea) => 
        handler.Invoke(JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(ea.Body.ToArray()), new JsonSerializerOptions { IncludeFields = true })!);
    }

    public void BlockingPublish<T>(T message) where T : BlockingMessage
    {
        channel.BasicPublish(
            exchange: Exchanges.blocking,
            routingKey: message.routingKey.ToString(),
            basicProperties: null,
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, serializerOpts))
            );
        AssociatedMessagePublish(message);
    }

    public void BlockingSubscribe<T>(Action<T> handler, TBlockingQueue queue) where T : BlockingMessage
    {
        var consumer = new EventingBasicConsumer(channel);
        channel.BasicConsume(queue.ToString(), true, consumer);

        consumer.Received += (model, ea) =>
        handler.Invoke(JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(ea.Body.ToArray()), new JsonSerializerOptions { IncludeFields = true })!);
    }

    //Used internally to publish status messages associated with core messages.
    private void AssociatedMessagePublish<T>(T message) where T : Message
    {
        if (message.StatusMessage.Message != string.Empty)
        {
            StatusPublish(message.StatusMessage);
        }
    }

    public void MatchingPublish<T>(T message) where T : MatchingMessage
    {
        channel.BasicPublish(
            exchange: Exchanges.matching,
            routingKey: message.routingKey.ToString(),
            basicProperties: null,
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, serializerOpts))
            );
        AssociatedMessagePublish(message);
    }

    public void MatchingSubscribe<T>(Action<T> handler, TMatchingQueue queue) where T : MatchingMessage
    {
        var consumer = new EventingBasicConsumer(channel);
        channel.BasicConsume(queue.ToString(), true, consumer);

        consumer.Received += (model, ea) =>
        handler.Invoke(JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(ea.Body.ToArray()), new JsonSerializerOptions { IncludeFields = true })!);
    }
}