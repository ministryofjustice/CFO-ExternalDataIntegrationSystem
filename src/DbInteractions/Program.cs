//This project is so all database interactions occur through a single service and so there's 
//only 1 set of connection strings to maintan. It will also make the examining of logs if 
//something goes wrong with the database easier in debugging.

using DbInteractions;
using DbInteractions.Services;
using Messaging.Interfaces;
using Messaging.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using EnvironmentSetup;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(@".\logs\fatal.txt", Serilog.Events.LogEventLevel.Fatal)
    .CreateBootstrapLogger();

try
{
    HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

    builder.Configuration.AddJsonFile("appsettings.json").AddEnvironmentVariables();
    builder.Configuration.ConfigureByEnvironment();

    builder.Services.ConfigureServices(builder.Configuration);

    builder.Services.Configure<ServerConfiguration>(
        builder.Configuration.GetSection("ServerConfiguration")
    );

    builder.Services.AddSingleton(s => s.GetRequiredService<IOptions<ServerConfiguration>>().Value);

    builder.Services.AddSingleton(
        new ConnectionStrings {
            deliusStagingConnectionString = builder.Configuration.GetConnectionString("DeliusStagingDb")!,
            offlocStagingConnectionString = builder.Configuration.GetConnectionString("OfflocStagingDb")!,
            deliusPictureConnectionString = builder.Configuration.GetConnectionString("DeliusRunningPictureDb")!,
            offlocPictureConnectionString = builder.Configuration.GetConnectionString("OfflocRunningPictureDb")!
        }
    );

    builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

    builder.Services.AddSingleton<RabbitService>(sp =>
    {
        var rabbitContext = sp.GetRequiredService<RabbitHostingContextWrapper>();
        return RabbitService.CreateAsync(rabbitContext).GetAwaiter().GetResult();
    });
    builder.Services.AddSingleton<IDbMessagingService>(sp => sp.GetRequiredService<RabbitService>());
    builder.Services.AddSingleton<IMergingMessagingService>(sp => sp.GetRequiredService<RabbitService>());
    builder.Services.AddSingleton<IStatusMessagingService>(sp => sp.GetRequiredService<RabbitService>());
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