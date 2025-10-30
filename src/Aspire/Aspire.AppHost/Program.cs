using Aspire.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

// Parameters
var password = builder.AddParameter("password", true);
var apiKey = builder.AddParameter("apikey", true);
var isDevelopment = builder.AddParameter("IsDevelopment");

// Database setup
var db = builder.AddDmsDatabase(password);
var databases = builder.AddDmsDatabases(db);

// API setup
var apiService = builder.AddDmsApi(databases, apiKey, isDevelopment);

// Visualiser setup
builder.AddDmsVisualiser(apiService, apiKey);

builder.Build().Run();