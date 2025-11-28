using Messaging.Extensions;
using Delius.Parser;
using Delius.Parser.Core;
using Delius.Parser.Services;
using EnvironmentSetup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.AddDmsCoreWorkerService();
    builder.Services.AddDmsRabbitMQ(builder.Configuration);

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