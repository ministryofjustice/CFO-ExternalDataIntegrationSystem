
using EnvironmentSetup;
using Messaging.Interfaces;
using Messaging.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Offloc.Parser;
using Offloc.Parser.Configuration;
using Offloc.Parser.Processor;
using Offloc.Parser.Services;
using Offloc.Parser.Services.TrimmerContext;
using Offloc.Parser.Writers.Factory;
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

    builder.Services.AddSingleton<IParsingStrategy, SequentialParsingStrategy>();
    
    //Extremely bad practice but just 
    builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

    builder.Services.ConfigureServices(builder.Configuration);
    builder.Services.AddSingleton<RabbitService>(sp =>
    {
        var rabbitContext = sp.GetRequiredService<RabbitHostingContextWrapper>();
        return RabbitService.CreateAsync(rabbitContext).GetAwaiter().GetResult();
    });
    builder.Services.AddSingleton<IStagingMessagingService>(sp => sp.GetRequiredService<RabbitService>());
    builder.Services.AddSingleton<IStatusMessagingService>(sp => sp.GetRequiredService<RabbitService>());

    builder.Services.AddSingleton<FieldTrimmerContext>();
    //builder.Services.AddSingleton<WriterFactory>();
    //builder.Services.AddSingleton<OffLocDefinition>();
    //builder.Services.AddSingleton<OfflocProcessor>();

    builder.Services.AddHostedService<OfflocParserBackgroundService>();

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