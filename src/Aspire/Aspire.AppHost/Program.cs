using Aspire.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

// Parameters
var password = builder.AddParameter("password", true);
var apiKey = builder.AddParameter("apikey", true);
var isDevelopment = builder.AddParameter("IsDevelopment");

var hostMount = HostExtensions.Create(Path.Combine(builder.AppHostDirectory, "DMS_STAGING"));
var targetMount = "/app/";

// Database setup
var sql = builder.AddDmsSqlServer(password)
    .WithBindMount(hostMount, targetMount);

var databases = builder.AddDmsDatabases(sql, seedData: false);

// API setup
var apiService = builder.AddDmsApi(databases, apiKey, isDevelopment);

// Visualiser setup
builder.AddDmsVisualiser(apiService);

var rabbit = builder
    .AddRabbitMQ("RabbitMQ")
    .WithManagementPlugin();

// MinIO (s3 emulation)
var minio = builder.AddMinioContainer("minio")
    .WithDataVolume("dms-minio-data");

builder.AddDmsServices(
    minio,
    rabbit,
    databases,
    hostMount,
    targetMount);

builder.Build().Run();