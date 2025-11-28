using FileStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace EnvironmentSetup;

public static class DmsServiceExtensions
{
    /// <summary>
    /// Configures Serilog logging for all DMS applications.
    /// Works for both ASP.NET Core apps and Worker Services.
    /// Reads configuration from appsettings.json Serilog section.
    /// </summary>
    public static IHostApplicationBuilder UseDmsSerilog(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSerilog(config => config
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.File(Path.Combine("logs", "fatal.txt"), Serilog.Events.LogEventLevel.Fatal));

        return builder;
    }

    /// <summary>
    /// Configures the application to run as a Windows Service.
    /// Required for all DMS worker services running on Windows Server EC2.
    /// </summary>
    public static IServiceCollection AddDmsWindowsService(this IServiceCollection services)
    {
        services.AddWindowsService();
        return services;
    }



    /// <summary>
    /// Configures file location paths for services that process files.
    /// Required by: FileSync, Offloc.Parser, Offloc.Cleaner, DbInteractions, Delius.Parser, Cleanup.
    /// </summary>
    public static IServiceCollection AddDmsFileLocations(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IFileLocations, FileLocations>(
            _ => new FileLocations(config.GetValue<string>("DMSFilesBasePath")!)
        );

        return services;
    }

    /// <summary>
    /// Configures common DMS worker service infrastructure: Serilog logging, Windows Service hosting, 
    /// and file locations.
    /// NOTE: RabbitMQ must be configured separately using AddDmsRabbitMQ() from Messaging library.
    /// This is a convenience method for the core DMS worker services.
    /// </summary>
    /// <param name="builder">The host application builder</param>
    /// <remarks>
    /// Suitable for: FileSync, Offloc.Parser, Offloc.Cleaner, DbInteractions, Delius.Parser, Cleanup, Import, Blocking, Matching.Engine, Logging.
    /// NOT suitable for: Meow (uses CATS RabbitMQ), API (ASP.NET Core), Visualiser (container).
    /// </remarks>
    public static IHostApplicationBuilder AddDmsCoreWorkerService(this IHostApplicationBuilder builder)
    {
        builder.UseDmsSerilog();
        builder.Services.AddDmsWindowsService();
        builder.Services.AddDmsFileLocations(builder.Configuration);

        return builder;
    }
}
