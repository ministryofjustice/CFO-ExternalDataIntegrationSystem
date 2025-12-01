using Infrastructure.Middlewares;
using Meow;
using Meow.Features.Participants.Handlers;
using Rebus.Serialization;

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.UseDmsSerilog();

    builder.Services.AddScoped<ICurrentUserService, MeowUserService>();

    builder.AddDatabaseServices();

    builder.Services.AddDmsWindowsService();

    builder.Services.AddRebus(configure =>
    {
        var connectionString = builder.Configuration.GetConnectionString("RabbitMQ");
        var rabbitSettings = builder.Configuration.GetRequiredSection("RabbitSettings");

        string queueName    = rabbitSettings["DmsService"]!,
               exchangeName = rabbitSettings["DirectExchange"]!,
               topicName    = rabbitSettings["TopicExchange"]!;

        return configure.Transport(t => t.UseRabbitMq(connectionString, queueName)
                .ExchangeNames(exchangeName, topicName))
            .Options(o =>
            {
                o.Register<IMessageTypeNameConvention>(_ => new CatsToDmsCustomTypeNameConventionBuilder());
            });

    });

    builder.Services.AutoRegisterHandlersFromAssemblyNamespaceOf<ParticipantCreatedIntegrationEventHandler>();

    builder.Services.AddHostedService<TopicSubscriptionService>();

    var app = builder.Build();
    
    Log.Information("Starting application");
    app.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.Information("Application stopping");
    await Log.CloseAndFlushAsync();
}