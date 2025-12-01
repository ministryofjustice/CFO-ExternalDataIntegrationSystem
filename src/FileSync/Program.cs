using Messaging.Extensions;

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.AddDmsCoreWorkerService();
    builder.Services.AddDmsRabbitMQ(builder.Configuration);

    builder.Services.AddOptions<SyncOptions>().BindConfiguration("SyncOptions");

    var source = builder.Configuration.GetValue<string>("FileSourceProvider")?.ToLowerInvariant();

    if (source == "s3")
    {
        builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
        builder.Services.AddAWSService<IAmazonS3>();
        builder.Services.AddSingleton<FileSource, S3FileSource>();
        builder.Services.AddSingleton(sp => new FileSourceOptions(builder.Configuration.GetValue<string>("AWS:S3:BucketName")!));
    }
    else if (source == "filesystem")
    {
        builder.Services.AddSingleton<FileSource, SystemFileSource>();
        builder.Services.AddSingleton(sp => new FileSourceOptions(builder.Configuration.GetValue<string>("FileSystem:SourceLocation")!));
    }
    else if(source == "minio")
    {
        builder.AddMinioClient("minio");
        builder.Services.AddSingleton<FileSource, MinioFileSource>();
        builder.Services.AddSingleton(sp => new FileSourceOptions(builder.Configuration.GetValue<string>("MinIO:BucketName")!));
    }
    else
    {
        throw new InvalidOperationException("Set 'FileSource' to 'S3', 'FileSystem', or 'MinIO' in configuration.");
    }

    builder.Services.AddHostedService<FileSyncBackgroundService>();

    var app = builder.Build();
    Log.Information("Starting application");
    app.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.Information("Application stopping");
    await Log.CloseAndFlushAsync();
}
