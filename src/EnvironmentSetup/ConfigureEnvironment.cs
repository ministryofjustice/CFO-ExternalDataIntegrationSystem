﻿using DotNetEnv.Configuration;
using FileStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Logging;
using Serilog;

namespace EnvironmentSetup;

public static class ConfigureEnvironment
{
    private static string envFileBasePath = $"{AppContext.BaseDirectory}..";

    public static ConfigurationManager ConfigureByEnvironment(this ConfigurationManager configManager)
    {
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);

        configManager.SetDotnetEnvironment();

        if (!configManager.GetValue<bool>("RUNNING_IN_CONTAINER"))
        {
            ConfigureEnv(configManager);
        }

        return configManager;
    }

    public static ConfigurationManager ConfigureEnv(this ConfigurationManager configManager)
    {
        string envFilePath = $@"{envFileBasePath}\development.local.env";

        if (File.Exists(envFilePath))
        {
            configManager.AddDotNetEnv(envFilePath);
        }
   
        return configManager;
    }

    public static void SetDotnetEnvironment(this ConfigurationManager config)
    {
        if (config.GetValue<string>("DOTNET_ENVIRONMENT") == null)
        {
            config["DOTNET_ENVIRONMENT"] = "Production";
        }
    }
}

public static class ServiceConfiguration
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, ConfigurationManager configManager)
    {
        if (configManager.GetValue<bool>("RUNNING_IN_CONTAINER"))
        {
            //Path splitter registration goes here.
            services.AddSingleton<IFileLocations, DockerFileLocations>();
        }
        else
        {
            services.AddSingleton<IFileLocations, LocalFileLocations>( 
                f => new LocalFileLocations(configManager.GetValue<string>("DMSFilesBasePath")!)
            );
        }

        if(WindowsServiceHelpers.IsWindowsService())
        {
            services.AddWindowsService();
        }

        services.ConfigureRabbit(configManager);
        services.ConfigureLogging(configManager);

        return services;
    }

    private static IServiceCollection ConfigureRabbit(this IServiceCollection services, IConfiguration config)
    {
        string? hosting = config.GetValue<string>("RABBIT_HOSTING");
        string? username = config.GetValue<string>("RABBIT_USERNAME");
        string? password = config.GetValue<string>("RABBIT_PASSWORD");

        services.AddSingleton(new RabbitHostingContextWrapper(hosting, username, password));

        return services;
    }


    private static IServiceCollection ConfigureLogging(this IServiceCollection services, IConfiguration config)
    {
        Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .WriteTo.File(@".\logs\fatal.txt", Serilog.Events.LogEventLevel.Fatal)
                .CreateLogger();

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSerilog(dispose: true);
        });

        services.AddSingleton(loggerFactory);
        services.AddLogging();

        return services;
    }

}
