using Meow;
using Meow.Features.Participants.Handlers;
using Rebus.Serialization;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(@".\logs\fatal.txt", Serilog.Events.LogEventLevel.Fatal)
    .CreateBootstrapLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);

    var services = builder.Services;
    var configuration = builder.Configuration;

    builder.AddDatabaseServices();

    services.ConfigureServices(configuration);

    builder.Services.AddRebus(configure =>
    {
        var connectionString = configuration.GetConnectionString("RabbitMQ");
        var rabbitSettings = configuration.GetRequiredSection("RabbitSettings");

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

    var host = builder.Build();

    await host.RunAsync();

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