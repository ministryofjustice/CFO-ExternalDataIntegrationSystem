Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(@"./logs/fatal.txt", Serilog.Events.LogEventLevel.Fatal)
    .CreateBootstrapLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.Configuration.ConfigureByEnvironment();

    builder.Services.ConfigureServices(builder.Configuration);

    builder.Services.AddSingleton<IStatusMessagingService, RabbitService>();
    builder.Services.AddSingleton<IMatchingMessagingService, RabbitService>();
    builder.Services.AddSingleton<IDbMessagingService, RabbitService>();

    // builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
    // builder.Services.AddAWSService<IAmazonS3>();

    builder.Services.AddOptions<S3Options>().BindConfiguration("S3");
    builder.Services.AddOptions<SyncOptions>().BindConfiguration("SyncOptions");

    builder.Services.AddHostedService<FileSyncBackgroundService>();

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
