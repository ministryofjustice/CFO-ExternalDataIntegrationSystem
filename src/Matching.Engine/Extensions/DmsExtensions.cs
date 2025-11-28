using Messaging.Extensions;
using Messaging.Services;
using EnvironmentSetup;

namespace Matching.Engine.Extensions;

public static class DmsExtensions
{
    public static void AddMessagingServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDmsRabbitMQ(builder.Configuration);
    }

}
