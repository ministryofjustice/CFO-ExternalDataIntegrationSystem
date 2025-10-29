namespace Delius.Parser.AppConfig.Models;

public class NextCloudCredentials
{
    public const string Key = nameof(NextCloudCredentials);

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
