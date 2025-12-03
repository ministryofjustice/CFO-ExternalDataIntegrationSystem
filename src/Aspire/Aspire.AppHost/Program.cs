using Aspire.AppHost.Extensions;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

// Parameters
var apiKey = builder.AddParameter("apiKey", true);
var isDevelopment = builder.AddParameter("isDevelopment");
var rabbitPassword = builder.AddParameter("rabbitPassword", true);
var sqlPassword = builder.AddParameter("sqlPassword", true);
var minioPassword = builder.AddParameter("minioPassword", true);
var seedData = builder.Configuration.GetValue<bool>("Parameters:seedData");
var startCoreServices = builder.Configuration.GetValue<bool>("Parameters:startCoreServices");

var hostMount = HostExtensions.Create(Path.Combine(builder.AppHostDirectory, "DMS_STAGING"));
var targetMount = "/app/";

// Database setup
var sql = builder.AddDmsSqlServer(sqlPassword)
    .WithBindMount(hostMount, targetMount);

var databases = builder.AddDmsDatabases(sql, seedData);

// API setup
var apiService = builder.AddDmsApi(databases, apiKey, isDevelopment);

// Visualiser setup
builder.AddDmsVisualiser(apiService);

if (startCoreServices)
{
    var rabbit = builder
    .AddRabbitMQ("RabbitMQ", password: rabbitPassword)
    .WithManagementPlugin(port: 15672);

    // MinIO (s3 emulation)
    var minio = builder
    .AddMinioContainer("minio", rootPassword: minioPassword)
    .WithDataVolume("dms-minio-data");

    builder.AddDmsServices(
        minio,
        rabbit,
        databases,
        hostMount,
        targetMount);
}

builder.Build().Run();