
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Offloc.Cleaner;
using Messaging.Services;
using Messaging.Interfaces;
using Microsoft.Extensions.Configuration;
using Offloc.Cleaner.Services;
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

    builder.Services.AddSingleton(
        new RedundantFieldsWrapper(
            builder.Configuration.GetValue<string>("RedundantOfflocFields")!
        ));

    builder.Services.AddSingleton<ICleaningStrategy, SequentialCleaningStrategy>();
    
    builder.Services.ConfigureServices(builder.Configuration);
    builder.Services.AddSingleton<IStagingMessagingService, RabbitService>();
    builder.Services.AddSingleton<IStatusMessagingService, RabbitService>();
    builder.Services.AddSingleton<IDbMessagingService, RabbitService>();

    builder.Services.AddHostedService<OfflocCleanerBackgroundService>();

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