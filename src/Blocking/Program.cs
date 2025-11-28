using Messaging.Extensions;
﻿using Messaging.Interfaces;
using Messaging.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EnvironmentSetup;
using Blocking;
using Blocking.ConfigurationModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

try
{
    HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

    builder.AddDmsCoreWorkerService();

    var storedProceduresConfig = builder.Configuration.GetSection("StoredProceduresConfig")!;
    var blockingQueriesGroups = builder.Configuration.GetSection("BlockingQueriesConfig")!;

    builder.Services.AddSingleton(
        new ServerConfiguration
        {
            connectionString = builder.Configuration.GetConnectionString("MatchingDb")!,
            storedProceduresConfig = builder.Configuration.GetRequiredSection("StoredProceduresConfig").Get<StoredProceduresConfig>()!,
            blockingQueriesConfig = builder.Configuration.GetSection("BlockingQueriesConfig").Get<BlockingQueriesConfig>()!
        }
    );

    builder.Services.AddSingleton<DatabaseInsert>();
    builder.Services.AddDmsRabbitMQ(builder.Configuration);

    builder.Services.AddHostedService<BlockingBackgroundService>();

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
