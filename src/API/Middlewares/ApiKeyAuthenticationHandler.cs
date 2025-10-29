using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace API.Middlewares;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{

    private const string ApiKeyHeaderName = "X-API-KEY";
    private readonly IConfiguration _configuration;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IConfiguration configuration)
        : base(options, logger, encoder, clock)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

        if (apiKeyHeaderValues.Count == 0 || string.IsNullOrEmpty(providedApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key provided."));
        }

        var configuredApiKey = _configuration.GetValue<string>("Authentication:ApiKey");

        if (providedApiKey == configuredApiKey)
        {
            var claims = new[] 
            { 
                new Claim(ClaimTypes.Name, "ApiUser"),
                new Claim("scp", "dms.read"),
                new Claim("scp", "dms.write")
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var identities = new[] { identity };
            var principal = new ClaimsPrincipal(identities);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        return Task.FromResult(AuthenticateResult.Fail("Invalid API Key provided."));
    }
}


public class LegacyApiKeyDefaults
{
    public const string AuthenticationScheme = "ApiKey";
}