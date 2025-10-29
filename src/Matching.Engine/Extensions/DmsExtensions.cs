using Messaging.Services;

namespace Matching.Engine.Extensions;

public static class DmsExtensions
{
    public static void AddMessagingServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IBlockingMessagingService, RabbitService>();
        builder.Services.AddSingleton<IMatchingMessagingService, RabbitService>();
        builder.Services.AddSingleton<IStatusMessagingService, RabbitService>();
        builder.Services.AddSingleton<IDbMessagingService, RabbitService>();
    }

}
