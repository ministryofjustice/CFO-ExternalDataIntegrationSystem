using Messaging.Extensions;
using Cleanup;
using Cleanup.CleanupServices.LiveCleanup;
using EnvironmentSetup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.AddDmsCoreWorkerService();
    builder.Services.AddDmsRabbitMQ(builder.Configuration);

    builder.Services.AddSingleton<OfflocCleanupService>();
    builder.Services.AddSingleton<DeliusCleanupService>();

    builder.Services.AddHostedService<CleanupBackgroundService>();

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