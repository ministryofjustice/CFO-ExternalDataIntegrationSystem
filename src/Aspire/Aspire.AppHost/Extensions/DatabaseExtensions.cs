using Projects;

namespace Aspire.AppHost.Extensions;

public static class DatabaseExtensions
{
    public static IResourceBuilder<SqlServerServerResource> AddDmsSqlServer(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<ParameterResource> password)
    {

#pragma warning disable ASPIREPROXYENDPOINTS001
        return builder.AddSqlServer("sql", password, 61749)
            .WithDataVolume("dms-data")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithEndpointProxySupport(false)
            .WithImageTag("2022-latest");
#pragma warning restore ASPIREPROXYENDPOINTS001
    }

    public static DmsDatabases AddDmsDatabases(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<SqlServerServerResource> sqlServer,
        bool seedData = false)
    {
        var audit = sqlServer.AddDatabase("AuditDb");
        var offlocStaging = sqlServer.AddDatabase("OfflocStagingDb");
        var deliusStaging = sqlServer.AddDatabase("DeliusStagingDb");
        var deliusRunningPicture = sqlServer.AddDatabase("DeliusRunningPictureDb");
        var offlocRunningPicture = sqlServer.AddDatabase("OfflocRunningPictureDb");
        var matching = sqlServer.AddDatabase("MatchingDb");
        var cluster = sqlServer.AddDatabase("ClusterDb");

        builder.AddSqlProject<AuditDb>("Audit")
            .WithReference(audit);

        builder.AddSqlProject<OfflocStagingDb>("OfflocStaging")
            .WithReference(offlocStaging);

        builder.AddSqlProject<DeliusStagingDb>("DeliusStaging")
            .WithReference(deliusStaging);

        builder.AddSqlProject<DeliusRunningPictureDb>("DeliusRunningPicture")
            .WithReference(deliusRunningPicture);

        builder.AddSqlProject<OfflocRunningPictureDb>("OfflocRunningPicture")
            .WithReference(offlocRunningPicture);

        builder.AddSqlProject<MatchingDb>("Matching")
            .WithReference(matching)
            .WithConfigureDacDeployOptions(options => {
                options.SetVariable("DeliusRunningPictureDb", "DeliusRunningPictureDb");
                options.SetVariable("OfflocRunningPictureDb", "OfflocRunningPictureDb");
            });

        var clusterSqlProj = builder.AddSqlProject<ClusterDb>("Cluster")
            .WithReference(cluster)
            .WithConfigureDacDeployOptions(options => {
                options.SetVariable("MatchingDb", "MatchingDb");
                options.SetVariable("DeliusRunningPictureDb", "DeliusRunningPictureDb");
                options.SetVariable("OfflocRunningPictureDb", "OfflocRunningPictureDb");
            });

        if(seedData)
        {
            builder.AddProject<FakeDataSeeder>("FakeDataSeeder")
                .WithReference(cluster)
                .WaitForCompletion(clusterSqlProj);
        }

        return new DmsDatabases(
            audit,
            offlocStaging,
            deliusStaging,
            deliusRunningPicture,
            offlocRunningPicture,
            matching,
            cluster
        );
    }
}

public record DmsDatabases(
    IResourceBuilder<SqlServerDatabaseResource> Audit,
    IResourceBuilder<SqlServerDatabaseResource> OfflocStaging,
    IResourceBuilder<SqlServerDatabaseResource> DeliusStaging,
    IResourceBuilder<SqlServerDatabaseResource> DeliusRunningPicture,
    IResourceBuilder<SqlServerDatabaseResource> OfflocRunningPicture,
    IResourceBuilder<SqlServerDatabaseResource> Matching,
    IResourceBuilder<SqlServerDatabaseResource> Cluster
);