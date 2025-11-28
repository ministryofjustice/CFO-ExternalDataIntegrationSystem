using Messaging.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Offloc.Cleaner;
using Microsoft.Extensions.Configuration;
using Offloc.Cleaner.Services;
using EnvironmentSetup;
using Serilog;

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.AddDmsCoreWorkerService();
    
    builder.Services.AddSingleton(
        new RedundantFieldsWrapper(
            builder.Configuration.GetValue<string>("RedundantOfflocFields")!
        ));

    builder.Services.AddSingleton<ICleaningStrategy, SequentialCleaningStrategy>();
    builder.Services.AddDmsRabbitMQ(builder.Configuration);

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