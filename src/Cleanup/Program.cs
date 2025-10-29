﻿
using Cleanup;
using Cleanup.CleanupServices.LiveCleanup;
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

    builder.Configuration.AddEnvironmentVariables();
    builder.Configuration.ConfigureByEnvironment();

    builder.Services.ConfigureServices(builder.Configuration);

    builder.Services.AddSingleton<OfflocCleanupService>();
    builder.Services.AddSingleton<DeliusCleanupService>();

    builder.Services.AddSingleton<IStagingMessagingService, RabbitService>();
    builder.Services.AddSingleton<IMergingMessagingService, RabbitService>();
    builder.Services.AddSingleton<IStatusMessagingService, RabbitService>();
    builder.Services.AddSingleton<IDbMessagingService, RabbitService>();

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