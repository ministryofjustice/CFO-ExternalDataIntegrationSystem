using Aspire.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

// Parameters
var password = builder.AddParameter("password", true);
var apiKey = builder.AddParameter("apikey", true);
var isDevelopment = builder.AddParameter("IsDevelopment");

// Database setup
var sql = builder.AddDmsSqlServer(password);
var databases = builder.AddDmsDatabases(sql, seedData: false);

// API setup
var apiService = builder.AddDmsApi(databases, apiKey, isDevelopment);

// Visualiser setup
builder.AddDmsVisualiser(apiService);

var rabbit = builder
    .AddRabbitMQ("RabbitMQ")
    .WithManagementPlugin();

builder.AddDmsServices(rabbit, databases);

builder.Build().Run();