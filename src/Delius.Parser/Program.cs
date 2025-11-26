
using Delius.Parser;
using Delius.Parser.Core;
using Delius.Parser.Services;
using EnvironmentSetup;
using Messaging.Interfaces;
using Messaging.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

    builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

    builder.Services.ConfigureServices(builder.Configuration);

    builder.Services.AddSingleton<IStagingMessagingService, RabbitService>();
    builder.Services.AddSingleton<IDbMessagingService, RabbitService>();
    builder.Services.AddSingleton<IStatusMessagingService, RabbitService>();


    builder.Services.AddSingleton<IParsingStrategy, SequentialParsingStrategy>();
        
    //Ordering here v. important.
    builder.Services.AddSingleton<PostParser>();
    builder.Services.AddSingleton<DeliusOutputter>();
    builder.Services.AddSingleton<DeliusProcessor>();
    builder.Services.AddSingleton<IFileProcessor, TextFileProcessor>();

    builder.Services.AddHostedService<DeliusParserBackgroundService>();

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