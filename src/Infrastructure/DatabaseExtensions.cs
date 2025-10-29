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
        builder.Services.AddScoped<AuditSaveChangesInterceptor>();

        builder.AddSqlServerDbContext<AuditContext>("AuditDb");

        builder.AddSqlServerDbContext<DeliusContext>("DeliusRunningPictureDb");
        builder.AddSqlServerDbContext<OfflocContext>("OfflocRunningPictureDb");

        builder.Services.AddDbContext<ClusteringContext>((sp, options) =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("ClusterDb"));
            options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        builder.Services.AddScoped<IClusteringRepository, ClusteringRepository>();
        builder.Services.AddScoped<IVisualisationRepository, VisualisationRepository>();
        builder.Services.AddScoped<IDeliusRepository, DeliusRepository>();
        builder.Services.AddScoped<IOfflocRepository, OfflocRepository>();

        return builder;
    }
}
