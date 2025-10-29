using API.Authentication;
using API.Middlewares;
using Infrastructure;
using Infrastructure.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Namotion.Reflection;
using OpenApiContact = NSwag.OpenApiContact;
using OpenApiInfo = NSwag.OpenApiInfo;
using OpenApiSecurityScheme = NSwag.OpenApiSecurityScheme;


var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.Services.AddOpenApiDocument(options =>
{
    options.PostProcess = document =>
    {
        document.Info = new OpenApiInfo()
        {
            Version = "v1",
            Title = "DMS Api",
            Description = "The DMS API",
            Contact = new OpenApiContact()
            {
                Name = "CFO ICT",
                Email = "cfo-ict@justice.gov.uk"
            }
        };
    };
    
    options.AddSecurity("ApiKey", new OpenApiSecurityScheme()
    {
        Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
        Name = "X-API-KEY",
        In = NSwag.OpenApiSecurityApiKeyLocation.Header,
        Description = "API Key needed to access endpoints"
    });

    options.OperationProcessors.Add(new AddApiKeyHeaderOperationProcessor());
});

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);

// The below configures a policy scheme called "Smart" that can handle both JWT Bearer and (legacy) API Key authentication.
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Smart";
    options.DefaultChallengeScheme = "Smart";
}).AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(LegacyApiKeyDefaults.AuthenticationScheme, _ => { })
  .AddPolicyScheme("Smart", $"Supports {LegacyApiKeyDefaults.AuthenticationScheme} and {JwtBearerDefaults.AuthenticationScheme} schemes", options =>
  {
      options.ForwardDefaultSelector = context =>
      {
          var authHeader = context.Request.Headers.Authorization.FirstOrDefault();

          if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
          {
              return JwtBearerDefaults.AuthenticationScheme;
          }

          return LegacyApiKeyDefaults.AuthenticationScheme;
      };
  });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("read", policy => policy.RequireScope("dms.read"));
    options.AddPolicy("write", policy => policy.RequireScope("dms.write"));
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder
    .AddDatabaseServices()
    .AddApplicationServices();

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "static")),
    RequestPath = "/static"
});

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (configuration["IsDevelopment"] is not null && configuration.GetValue<bool>("IsDevelopment"))
{
    using var scope = app.Services.CreateScope();
    
    var delius = scope.ServiceProvider.GetRequiredService<DeliusContext>();
    delius.Database.EnsureCreated();

    var offloc = scope.ServiceProvider.GetRequiredService<OfflocContext>();
    offloc.Database.EnsureCreated();

    var cluster = scope.ServiceProvider.GetRequiredService<ClusteringContext>();
    cluster.Database.EnsureCreated();

    var audit = scope.ServiceProvider.GetRequiredService<AuditContext>();
    audit.Database.EnsureCreated();

    app.RegisterDevelopmentEndpoints();

    app.UseOpenApi();

    app.UseSwaggerUi(options =>
    {
        options.CustomStylesheetPath = "/static/dms-swagger.css";
    });

    var db = scope.ServiceProvider.GetRequiredService<ClusteringContext>();

    var baseDir = AppContext.BaseDirectory;

    var scriptsPath = Path.Combine(baseDir, "Scripts");

    if(Path.Exists(scriptsPath) == false)
    {
        throw new DirectoryNotFoundException($"Scripts directory not found at {scriptsPath}");
    }

    foreach(var file in Directory.GetFiles(scriptsPath, "*.sql").OrderBy(file => file))
    {
        var sql = await File.ReadAllTextAsync(file);
        await db.Database.ExecuteSqlRawAsync(sql);
    }

}

app.UseAuthentication();
app.UseAuthorization();

app.RegisterClusteringEndpoints()
   .RegisterDeliusEndpoints()
   .RegisterOfflocEndpoints()
   .RegisterSearchEndpoints()
   .RegisterReferenceEndpoints()
   .RegisterVisualisationEndpoints();

app.MapDefaultEndpoints();

app.Run();
