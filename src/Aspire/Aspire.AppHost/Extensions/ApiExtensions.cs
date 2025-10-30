using Projects;

namespace Aspire.AppHost.Extensions;

public static class ApiExtensions
{
    public static IResourceBuilder<ProjectResource> AddDmsApi(
        this IDistributedApplicationBuilder builder,
        DmsDatabases databases,
        IResourceBuilder<ParameterResource> apiKey,
        IResourceBuilder<ParameterResource> isDevelopment)
    {
        return builder.AddProject<API>("api")
            .WithReference(databases.OfflocRunningPicture)
            .WithReference(databases.DeliusRunningPicture)
            .WithReference(databases.Cluster)
            .WithEnvironment("Authentication__ApiKey", apiKey)
            .WithEnvironment("IsDevelopment", isDevelopment)
            .WaitFor(databases.Audit)
            .WaitFor(databases.OfflocStaging)
            .WaitFor(databases.DeliusStaging)
            .WaitFor(databases.OfflocRunningPicture)
            .WaitFor(databases.DeliusRunningPicture)
            .WaitFor(databases.Matching);
    }

    public static IResourceBuilder<ProjectResource> AddDmsVisualiser(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<ProjectResource> apiService,
        IResourceBuilder<ParameterResource> apiKey)
    {
        return builder.AddProject<Visualiser>("visualiser")
            .WithReference(apiService)
            .WithEnvironment("API__Key", apiKey)
            .WaitFor(apiService);
    }
}
