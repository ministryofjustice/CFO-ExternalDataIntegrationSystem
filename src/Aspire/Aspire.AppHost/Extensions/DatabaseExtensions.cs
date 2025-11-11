using Projects;

namespace Aspire.AppHost.Extensions;

public static class DatabaseExtensions
{
    public static IResourceBuilder<SqlServerServerResource> AddDmsSqlServer(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<ParameterResource> password)
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

#pragma warning disable ASPIREPROXYENDPOINTS001
        return builder.AddSqlServer("sql", password, 61749)
            .WithDataVolume("dms-data")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithEndpointProxySupport(false)
            .WithImageTag("2022-latest");
#pragma warning restore ASPIREPROXYENDPOINTS001
    }

    public static DmsDatabaseResources AddDmsDatabases(
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

        var auditSqlProj = builder.AddSqlProject<AuditDb>("Audit")
            .WithReference(audit);

        var offlocStagingSqlProj = builder.AddSqlProject<OfflocStagingDb>("OfflocStaging")
            .WithReference(offlocStaging)
            .WithSkipWhenDeployed();

        var deliusStagingSqlProj = builder.AddSqlProject<DeliusStagingDb>("DeliusStaging")
            .WithReference(deliusStaging)
            .WithSkipWhenDeployed();

        var deliusRunningPictureSqlProj = builder.AddSqlProject<DeliusRunningPictureDb>("DeliusRunningPicture")
            .WithReference(deliusRunningPicture)
            .WithSkipWhenDeployed();

        var offlocRunningPictureSqlProj = builder.AddSqlProject<OfflocRunningPictureDb>("OfflocRunningPicture")
            .WithReference(offlocRunningPicture)
            .WithSkipWhenDeployed();

        var matchingSqlProj = builder.AddSqlProject<MatchingDb>("Matching")
            .WithReference(matching)
            .WithConfigureDacDeployOptions(options => {
                options.SetVariable("DeliusRunningPictureDb", "DeliusRunningPictureDb");
                options.SetVariable("OfflocRunningPictureDb", "OfflocRunningPictureDb");
            })
            .WaitForCompletion(deliusRunningPictureSqlProj)
            .WaitForCompletion(offlocRunningPictureSqlProj)
            .WithSkipWhenDeployed();

        var clusterSqlProj = builder.AddSqlProject<ClusterDb>("Cluster")
            .WithReference(cluster)
            .WithConfigureDacDeployOptions(options =>
            {
                options.SetVariable("MatchingDb", "MatchingDb");
                options.SetVariable("DeliusRunningPictureDb", "DeliusRunningPictureDb");
                options.SetVariable("OfflocRunningPictureDb", "OfflocRunningPictureDb");
                options.SetVariable("PopulateReferenceTables", "True");
            })
            .WaitForCompletion(matchingSqlProj)
            .WaitForCompletion(deliusRunningPictureSqlProj)
            .WaitForCompletion(offlocRunningPictureSqlProj)
            .WithSkipWhenDeployed();

        if (seedData)
        {
            builder.AddProject<FakeDataSeeder>("FakeDataSeeder")
                .WithReference(cluster)
                .WaitForCompletion(clusterSqlProj)
                .WaitForCompletion(offlocRunningPictureSqlProj)
                .WaitForCompletion(deliusRunningPictureSqlProj);
        }

        return new DmsDatabaseResources(
            new DmsDatabaseResource(audit, auditSqlProj),
            new DmsDatabaseResource(offlocStaging, offlocStagingSqlProj),
            new DmsDatabaseResource(deliusStaging, deliusStagingSqlProj),
            new DmsDatabaseResource(deliusRunningPicture, deliusRunningPictureSqlProj),
            new DmsDatabaseResource(offlocRunningPicture, offlocRunningPictureSqlProj),
            new DmsDatabaseResource(matching, matchingSqlProj),
            new DmsDatabaseResource(cluster, clusterSqlProj)
        );
    }
}

public record DmsDatabaseResources(
    DmsDatabaseResource Audit,
    DmsDatabaseResource OfflocStaging,
    DmsDatabaseResource DeliusStaging,
    DmsDatabaseResource DeliusRunningPicture,
    DmsDatabaseResource OfflocRunningPicture,
    DmsDatabaseResource Matching,
    DmsDatabaseResource Cluster
);

public record DmsDatabaseResource(
    IResourceBuilder<SqlServerDatabaseResource> DatabaseResource, 
    IResourceBuilder<SqlProjectResource> SqlProjectResource);