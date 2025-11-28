using Messaging.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EnvironmentSetup;
using Blocking;
using Blocking.ConfigurationModels;
using Serilog;

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.AddDmsCoreWorkerService();

    builder.Services.Configure<StoredProceduresConfig>(
        builder.Configuration.GetRequiredSection("StoredProceduresConfig"));
    builder.Services.Configure<BlockingQueriesConfig>(
        builder.Configuration.GetRequiredSection("BlockingQueriesConfig"));

    builder.Services.AddSingleton<DatabaseInsert>();
    builder.Services.AddDmsRabbitMQ(builder.Configuration);

    builder.Services.AddHostedService<BlockingBackgroundService>();

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
