//This project is so all database interactions occur through a single service and so there's 
//only 1 set of connection strings to maintan. It will also make the examining of logs if 
//something goes wrong with the database easier in debugging.
using DbInteractions;
using DbInteractions.Services;
using Messaging.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EnvironmentSetup;
using Serilog;

try
{
    HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

    builder.AddDmsCoreWorkerService();

    builder.Services.AddOptions<ServerConfiguration>().BindConfiguration("ServerConfiguration");

    builder.Services.AddDmsRabbitMQ(builder.Configuration);
    builder.Services.AddSingleton<IDbInteractionService, DbInteractionService>();

    builder.Services.AddHostedService<DbBackgroundService>();

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