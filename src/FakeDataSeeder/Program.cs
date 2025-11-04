using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<FakeDataSeederBackgroundService>();

var app = builder.Build();

await app.RunAsync();