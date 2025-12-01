using Infrastructure.Contexts;
using Infrastructure.Middlewares;
using Infrastructure.Repositories.Clustering;
using Infrastructure.Repositories.Delius;
using Infrastructure.Repositories.Offloc;
using Infrastructure.Repositories.Visualisation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class DatabaseExtensions
{
    public static IHostApplicationBuilder AddDatabaseServices(this IHostApplicationBuilder builder)
    {
        if (!builder.Services.Any(x => x.ServiceType == typeof(ICurrentUserService)))
        {
            throw new InvalidOperationException(
                "ICurrentUserService must be registered before calling AddDatabaseServices. " +
                "Register an implementation using builder.Services.AddScoped<ICurrentUserService, YourImplementation>()");
        }

        builder.Services.AddScoped<AuditSaveChangesInterceptor>();

        builder.Services.AddDbContext<AuditContext>((sp, options) =>
        {
            options.UseSqlServer(GetRequiredConnectionString(builder.Configuration, "AuditDb"));
        });

        builder.Services.AddDbContext<DeliusContext>((sp, options) =>
        {
            options.UseSqlServer(GetRequiredConnectionString(builder.Configuration, "DeliusRunningPictureDb"));
        });
        
        builder.Services.AddDbContext<OfflocContext>((sp, options) =>
        {
            options.UseSqlServer(GetRequiredConnectionString(builder.Configuration, "OfflocRunningPictureDb"));
        });

        builder.Services.AddDbContext<ClusteringContext>((sp, options) =>
        {
            options.UseSqlServer(GetRequiredConnectionString(builder.Configuration, "ClusterDb"));
            options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        builder.Services.AddScoped<IClusteringRepository, ClusteringRepository>();
        builder.Services.AddScoped<IVisualisationRepository, VisualisationRepository>();
        builder.Services.AddScoped<IDeliusRepository, DeliusRepository>();
        builder.Services.AddScoped<IOfflocRepository, OfflocRepository>();

        return builder;
    }

    private static string GetRequiredConnectionString(IConfiguration configuration, string name) =>
        configuration.GetConnectionString(name) ?? throw new InvalidOperationException($"Connection string '{name}' is not configured.");
}
