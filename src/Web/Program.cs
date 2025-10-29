
using Web.ConfigClasses;
using Web.Components;
using Messaging.Services;
using Messaging.Interfaces;
using EnvironmentSetup;
using FileStorage;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json").AddEnvironmentVariables();

builder.Configuration.ConfigureByEnvironment();

builder.Services.AddSingleton(new DownloadFilesStatus
{
    offlocStatus = builder.Configuration.GetValue<bool>("DownloadOfflocFiles"),
    deliusStatus = builder.Configuration.GetValue<bool>("DownloadDeliusFiles")
});

builder.Services.ConfigureServices(builder.Configuration);

//Bit naff but only necessary because it's a web app- will be console app eventually.
//if (builder.Configuration.GetValue<bool>("RUNNING_IN_CONTAINER"))
//{
	builder.Services.AddDataProtection()
	.PersistKeysToFileSystem(
		new DirectoryInfo(new DockerFileLocations().keyLocations)
	);
//}
//else
//{
//	builder.Services.AddDataProtection()
//	.PersistKeysToFileSystem(
//		new DirectoryInfo(new LocalFileLocations().keyLocations)
//	);
//}

builder.Services.AddSingleton<IStagingMessagingService, RabbitService>();
builder.Services.AddSingleton<IStatusMessagingService, RabbitService>();
builder.Services.AddSingleton<IDbMessagingService, RabbitService>();

//Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
