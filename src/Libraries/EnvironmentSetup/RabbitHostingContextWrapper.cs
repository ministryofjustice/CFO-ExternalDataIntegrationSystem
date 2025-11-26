
namespace EnvironmentSetup;

public class RabbitHostingContextWrapper
{
    private const string defaultContext = "host.docker.internal";
    //Default value assumes docker.
    public string Context { get; } = defaultContext;
    public string Username { get; }
    public string Password { get; }

    public Uri? Uri { get; }

    public RabbitHostingContextWrapper(Uri uri) : this(defaultContext)
    {
        Uri = uri;
    }

    public RabbitHostingContextWrapper(string? hostingContext, string username = "guest", string password = "guest")
    {
        Context = hostingContext ?? defaultContext;
        Username = username;
        Password = password;
    }
}
