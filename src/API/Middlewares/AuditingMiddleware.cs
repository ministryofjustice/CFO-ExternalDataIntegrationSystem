using Serilog.Context;

namespace API.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {

        var requestId = Guid.NewGuid().ToString();
        context.Items["RequestId"] = requestId;

        using (LogContext.PushProperty("RequestId", requestId))
        {
            // Log the request details
            var request = context.Request;
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var method = request.Method;
            var path = request.Path;
            var queryString = request.QueryString.HasValue ? request.QueryString.Value : string.Empty;

            _logger.LogInformation($"Incoming request from IP: {ipAddress}, Method: {method}, Path: {path}, Query String: {queryString}");

            // Invoke the next middleware in the pipeline
            await _next(context);

            // Log the response status code
            var statusCode = context.Response.StatusCode;
            _logger.LogInformation($"Response status code: {statusCode}");
        }


    }
}