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
        IResourceBuilder<MinioContainerResource> minio,
        IResourceBuilder<RabbitMQServerResource> rabbit,
        DmsDatabaseResources databases,
        string hostMount,
        string targetMount)
    {
        builder.AddDmsService<Blocking>("Blocking", rabbit, databases, hostMount);
        builder.AddDmsService<Cleanup>("Cleanup", rabbit, databases, hostMount);

        builder.AddDmsService<DbInteractions>("DbInteractions", rabbit, databases, hostMount)
            .WithEnvironment("DmsFilesBasePath", targetMount); // Override default with linux (sql container) path
        
        builder.AddDmsService<Delius_Parser>("Delius-Parser", rabbit, databases, hostMount);
        builder.AddDmsService<Import>("Import", rabbit, databases, hostMount);
        builder.AddDmsService<Logging>("Logging", rabbit, databases, hostMount);
        builder.AddDmsService<Matching_Engine>("Matching-Engine", rabbit, databases, hostMount);
        builder.AddDmsService<Meow>("Meow", rabbit, databases, hostMount).WithExplicitStart();
        builder.AddDmsService<Offloc_Cleaner>("Offloc-Cleaner", rabbit, databases, hostMount);
        builder.AddDmsService<Offloc_Parser>("Offloc-Parser", rabbit, databases, hostMount);

        builder.AddDmsService<FileSync>("FileSync", rabbit, databases, hostMount)
            .WithReference(minio).WaitFor(minio)
            .WithExplicitStart()
            .WithEnvironment("MinIO:BucketName", "cfo-dms-files");
            
        return builder;
    }

    private static IResourceBuilder<ProjectResource> AddDmsService<TDmsProject>(
        this IDistributedApplicationBuilder builder,
        string name,
        IResourceBuilder<RabbitMQServerResource> rabbit,
        DmsDatabaseResources databases,
        string hostMount) where TDmsProject : IProjectMetadata, new()
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
            .WithEnvironment("DmsFilesBasePath", hostMount)
            .WithEnvironment("DOTNET_ENVIRONMENT", "Development");

        return project;
    }

    private static IResourceBuilder<ProjectResource> WithDmsDatabaseReference(this IResourceBuilder<ProjectResource> builder, DmsDatabaseResource database)
    {
        builder.WithReference(database.DatabaseResource).WaitForCompletion(database.SqlProjectResource);
        return builder;
    }

}
