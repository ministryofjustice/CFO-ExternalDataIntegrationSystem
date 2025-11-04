using Projects;

namespace Aspire.AppHost.Extensions;

public static class AppExtensions
{
    public static IResourceBuilder<ProjectResource> AddDmsApi(
        this IDistributedApplicationBuilder builder,
        DmsDatabaseResources databases,
        IResourceBuilder<ParameterResource> apiKey,
        IResourceBuilder<ParameterResource> isDevelopment)
    {
        return builder.AddProject<API>("api")
            .WithDmsDatabaseReference(databases.OfflocRunningPicture)
            .WithDmsDatabaseReference(databases.DeliusRunningPicture)
            .WithDmsDatabaseReference(databases.Cluster)
            .WithDmsDatabaseReference(databases.Audit)
            .WithEnvironment("Authentication__ApiKey", apiKey)
            .WithEnvironment("IsDevelopment", isDevelopment);
    }

    public static IResourceBuilder<ProjectResource> AddDmsVisualiser(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<ProjectResource> apiService)
    {
        return builder.AddProject<Visualiser>("visualiser")
            .WithReference(apiService).WaitFor(apiService);
    }

    public static IDistributedApplicationBuilder AddDmsServices(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<RabbitMQServerResource> rabbit,
        DmsDatabaseResources databases)
    {
        builder.AddDmsService<Blocking>("Blocking", rabbit, databases);
        builder.AddDmsService<Cleanup>("Cleanup", rabbit, databases);
        builder.AddDmsService<DbInteractions>("DbInteractions", rabbit, databases);
        builder.AddDmsService<Delius_Parser>("Delius-Parser", rabbit, databases);
        builder.AddDmsService<Import>("Import", rabbit, databases);
        builder.AddDmsService<Logging>("Logging", rabbit, databases);
        builder.AddDmsService<Matching_Engine>("Matching-Engine", rabbit, databases);
        //builder.AddDmsService<Meow>("Meow", rabbit, databases);
        builder.AddDmsService<Offloc_Cleaner>("Offloc-Cleaner", rabbit, databases);
        builder.AddDmsService<Offloc_Parser>("Offloc-Parser", rabbit, databases);
        //builder.AddDmsService<Orchestrator>("Orchestrator", rabbit, databases);

        // Entry point - start the app to 'kick off' DMS
        builder.AddDmsService<Kickoff>("Kickoff", rabbit, databases)
            .WithExplicitStart();

        return builder;
    }

    private static IResourceBuilder<ProjectResource> AddDmsService<TDmsProject>(
        this IDistributedApplicationBuilder builder, 
        string name,
        IResourceBuilder<RabbitMQServerResource> rabbit,
        DmsDatabaseResources databases) where TDmsProject : IProjectMetadata, new()
    {
        var project = builder.AddProject<TDmsProject>(name)
            .WithReference(rabbit).WaitFor(rabbit)
            .WithDmsDatabaseReference(databases.OfflocStaging)
            .WithDmsDatabaseReference(databases.OfflocRunningPicture)
            .WithDmsDatabaseReference(databases.DeliusStaging)
            .WithDmsDatabaseReference(databases.DeliusRunningPicture)
            .WithDmsDatabaseReference(databases.Audit)
            .WithDmsDatabaseReference(databases.Cluster)
            .WithDmsDatabaseReference(databases.Matching)
            .WithEnvironment("DmsFilesBasePath", "~/DMS/");

        return project;
    }

    private static IResourceBuilder<ProjectResource> WithDmsDatabaseReference(this IResourceBuilder<ProjectResource> builder, DmsDatabaseResource database)
    {
        builder.WithReference(database.DatabaseResource).WaitForCompletion(database.SqlProjectResource);
        return builder;
    }

}
