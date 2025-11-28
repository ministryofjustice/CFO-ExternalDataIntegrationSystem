using Messaging.Interfaces;
using Messaging.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Extensions;

public static class DmsRabbitMQExtensions
{
    /// <summary>
    /// Configures DMS RabbitMQ messaging for worker services.
    /// Registers RabbitService and all messaging interfaces it implements.
    /// </summary>
    public static IServiceCollection AddDmsRabbitMQ(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton(sp =>
        {
            var connectionString = config.GetConnectionString("RabbitMQ")!;
            return RabbitService.CreateAsync(new Uri(connectionString)).GetAwaiter().GetResult();
        });

        // Register all messaging interfaces
        services.AddSingleton<IMessageService>(sp => sp.GetRequiredService<RabbitService>());
        services.AddSingleton<IStagingMessagingService>(sp => sp.GetRequiredService<RabbitService>());
        services.AddSingleton<IMergingMessagingService>(sp => sp.GetRequiredService<RabbitService>());
        services.AddSingleton<IStatusMessagingService>(sp => sp.GetRequiredService<RabbitService>());
        services.AddSingleton<IDbMessagingService>(sp => sp.GetRequiredService<RabbitService>());
        services.AddSingleton<IImportMessagingService>(sp => sp.GetRequiredService<RabbitService>());
        services.AddSingleton<IBlockingMessagingService>(sp => sp.GetRequiredService<RabbitService>());
        services.AddSingleton<IMatchingMessagingService>(sp => sp.GetRequiredService<RabbitService>());

        return services;
    }
}
