using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class FakeDataSeederBackgroundService(
    ILogger<FakeDataSeederBackgroundService> logger,
    IHostApplicationLifetime lifetime, 
    IConfiguration configuration) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("FakeDataSeeder started");

        var path = Path.Combine(AppContext.BaseDirectory, "Scripts");

        if (Path.Exists(path) is false)
        {
            throw new DirectoryNotFoundException($"Scripts directory not found at path: {path}");
        }

        var connectionString = configuration.GetConnectionString("ClusterDb");
        using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync(cancellationToken);

        var files = Directory.GetFiles(path, "*.sql");

        foreach (var file in files.OrderBy(file => file))
        {
            logger.LogInformation($"Seeding: {Path.GetFileName(file)}");
            var sql = await File.ReadAllTextAsync(file, cancellationToken);
            using var cmd = new SqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync(cancellationToken);
            logger.LogInformation($"Seeded: {Path.GetFileName(file)}");
        }

        logger.LogInformation($"Successfully seeded {files.Length} file(s)");

        lifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("FakeDataSeeder stopped");
        return Task.CompletedTask;
    }
}