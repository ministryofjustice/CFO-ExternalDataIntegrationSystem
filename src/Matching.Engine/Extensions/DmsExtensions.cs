using Messaging.Services;
using EnvironmentSetup;

namespace Matching.Engine.Extensions;

public static class DmsExtensions
{
    public static void AddMessagingServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<RabbitService>(sp =>
        {
            var rabbitContext = sp.GetRequiredService<RabbitHostingContextWrapper>();
            return RabbitService.CreateAsync(rabbitContext).GetAwaiter().GetResult();
        });
        builder.Services.AddSingleton<IBlockingMessagingService>(sp => sp.GetRequiredService<RabbitService>());
        builder.Services.AddSingleton<IMatchingMessagingService>(sp => sp.GetRequiredService<RabbitService>());
        builder.Services.AddSingleton<IStatusMessagingService>(sp => sp.GetRequiredService<RabbitService>());
        builder.Services.AddSingleton<IDbMessagingService>(sp => sp.GetRequiredService<RabbitService>());
    }

}
