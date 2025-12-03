using Messaging.Extensions;

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.ConfigureContainer(new AutofacServiceProviderFactory(), container =>
    {
        container.RegisterMatchers();
        container.RegisterScorers(builder.Configuration);
    });

    builder.AddDmsCoreWorkerService();
    builder.Services.AddDmsRabbitMQ(builder.Configuration);

    builder.AddApplicationServices();

    builder.Services.AddSingleton<MatchingQueue>();

    builder.Services
        .AddHostedService<ComparatorService>()
        .AddHostedService<ScorerService>()
        .AddHostedService<ClusteringService>();

    var app = builder.Build();

    Log.Information("Starting application");
    await app.RunAsync();

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