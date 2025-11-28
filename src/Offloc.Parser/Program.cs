using Messaging.Extensions;
using EnvironmentSetup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Offloc.Parser;
using Offloc.Parser.Services;
using Offloc.Parser.Services.TrimmerContext;
using Serilog;

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.AddDmsCoreWorkerService();
    builder.Services.AddDmsRabbitMQ(builder.Configuration);
    
    builder.Services.AddSingleton<IParsingStrategy, SequentialParsingStrategy>();

    builder.Services.AddSingleton<FieldTrimmerContext>();

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