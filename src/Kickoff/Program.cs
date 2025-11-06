
using EnvironmentSetup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Messaging.Interfaces;
using Messaging.Services;
using Kickoff;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(@".\logs\fatal.txt", Serilog.Events.LogEventLevel.Fatal)
    .CreateBootstrapLogger();

try
{
	var builder = Host.CreateApplicationBuilder(args);

	builder.Configuration.ConfigureByEnvironment();

	builder.Services.ConfigureServices(builder.Configuration);

	builder.Services.AddSingleton<IStagingMessagingService, RabbitService>();
	builder.Services.AddSingleton<IStatusMessagingService, RabbitService>();
	builder.Services.AddSingleton<IDbMessagingService, RabbitService>();

    builder.Services.AddHostedService<KickoffService>();

    var host = builder.Build();

    Log.Information("Starting application");
    await host.RunAsync();

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