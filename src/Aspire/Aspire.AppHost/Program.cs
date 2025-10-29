var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("password", secret: true);
var key = builder.AddParameter("apikey", secret: true);
var isDevelopment = builder.AddParameter("IsDevelopment", false);

#pragma warning disable ASPIREPROXYENDPOINTS001
var db = builder.AddSqlServer("sql", password, 61749)
    .WithDataVolume("dms-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpointProxySupport(false);
#pragma warning restore ASPIREPROXYENDPOINTS001

var delius = db.AddDatabase("DeliusRunningPictureDb");
var offloc = db.AddDatabase("OfflocRunningPictureDb");
var cluster = db.AddDatabase("ClusterDb");
var audit = db.AddDatabase("AuditDb");

var apiService = builder.AddProject<Projects.API>("api")
   .WithReference(delius)
   .WithReference(offloc)
   .WithReference(cluster)
   .WithReference(audit)
   .WithEnvironment("Authentication__ApiKey", key)
   .WithEnvironment("IsDevelopment", isDevelopment)
   .WaitFor(delius)
   .WaitFor(offloc)
   .WaitFor(cluster)
   .WaitFor(audit);

var meowService = builder.AddProject<Projects.Meow>("meow")
   .WithReference(delius)
   .WithReference(offloc)
   .WithReference(cluster)
   .WaitFor(delius)
   .WaitFor(offloc)
   .WaitFor(cluster);

var visualiser = builder.AddProject<Projects.Visualiser>("visualiser")
    .WithReference(apiService)
    .WithEnvironment("API__Key", key)
    .WaitFor(apiService);

builder.Build().Run();
