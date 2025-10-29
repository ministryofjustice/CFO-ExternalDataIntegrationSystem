using API.Services;
using API.Services.SentenceInformation;
using Microsoft.AspNetCore.Http.Json;

namespace API.Extensions;

public static class Extensions
{
    public static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<JsonOptions>(options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        builder.Services.AddScoped<AggregateService>();

        builder.Services.AddScoped<ApiServices>();
        
        builder.Services.AddScoped<SentenceInformationService>();

        return builder;
    }
    
    public static WebApplication UseApplicationServices(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseRouting();

        return app;
    }}

