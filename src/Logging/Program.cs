using Messaging.Extensions;
﻿try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.AddDmsCoreWorkerService();
    builder.Services.AddDmsRabbitMQ(builder.Configuration);

    builder.Services.AddHostedService<LoggingBackgroundService>();

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