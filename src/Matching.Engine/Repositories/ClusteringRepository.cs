
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Matching.Engine.Repositories;

public class ClusteringRepository(
    ILogger<ClusteringRepository> logger,
    IConfiguration configuration) : IClusteringRepository
{
    private IConfiguration configuration = configuration;
    private ILogger<ClusteringRepository> logger = logger;

    public async Task<IEnumerable<dynamic>> GetAllAsync()
    {
        using var connection = CreateAndOpenConnection();

        string query = "SELECT * FROM Clustering.RecordMatchingView";

        logger.LogInformation($"Executing query: '{query}'");

        var edges = await connection.QueryAsync<dynamic>(query);

        logger.LogInformation($"Query returned {edges.Count()} records.");

        return edges;
    }

    public async Task ClusterPostProcessAsync()
    {
        var procedure = "processing.Phase2";

        logger.LogInformation($"Executing stored procedure '{procedure}'");

        using var connection = CreateAndOpenConnection();
        await connection.ExecuteAsync(procedure, commandType: CommandType.StoredProcedure, commandTimeout: 1800);

        logger.LogInformation($"Procedure executed successfully.");
    }

    public async Task ClusterPreProcessAsync()
    {
        var procedure = "processing.Phase1";

        logger.LogInformation($"Executing stored procedure '{procedure}'");

        using var connection = CreateAndOpenConnection();
        await connection.ExecuteAsync(procedure, commandType: CommandType.StoredProcedure, commandTimeout: 600);

        logger.LogInformation($"Procedure executed successfully.");
    }

    public async Task BulkInsertAsync(Dictionary<ComparisonResult, double> records)
    {
        var batches = Batch(records, 1000) ?? [];

        int numOfBatches = batches.Count();

        int concurrency = 16;
        using var semaphore = new SemaphoreSlim(concurrency);

        logger.LogInformation($"Beginning batch insert of {records.Count} records into Clustering dataset(s) in {numOfBatches} batches...");

        var tasks = batches.Select(async (batch, batchIndex) =>
        {
            await semaphore.WaitAsync();
            
            int id = batchIndex + 1;

            logger.LogInformation($"Inserting batch {id} (of {numOfBatches})...");

            string insertQuery = @"
                INSERT INTO [ClusterDb].[input].[EdgeProbabilities_Pass2] (SourceName, SourceKey, TargetName, TargetKey, Probability) 
                VALUES (@SourceName, @SourceKey, @TargetName, @TargetKey, @Probability);";

            try
            {
                using var connection = CreateAndOpenConnection();
                await connection.ExecuteAsync(insertQuery, batch, commandTimeout: 600);
            }
            finally
            {
                semaphore.Release();
            }

            logger.LogInformation($"Batch {id} inserted successfully.");
        });

        await Task.WhenAll(tasks);
        
        logger.LogInformation($"Inserted {numOfBatches} batches into Clustering dataset(s).");
    }

    private static IEnumerable<IEnumerable<object>> Batch(Dictionary<ComparisonResult, double> records, int batchSize)
    {
        var batchNos = (int)Math.Ceiling((double)records.Count / batchSize);

        for (int i = 0; i < batchNos; i++)
        {
            var batch = records.Skip(i * batchSize).Take(batchSize);

            yield return batch.Select(r => new
            {
                SourceName = r.Key.Record.l_SourceName,
                SourceKey = r.Key.Record.l_SourceKey,
                TargetName = r.Key.Record.r_SourceName,
                TargetKey = r.Key.Record.r_SourceKey,
                Probability = r.Value
            });
        }
    }

    private SqlConnection CreateAndOpenConnection()
    {
        var conn = new SqlConnection(configuration.GetConnectionString("ClusterDb"));
        conn.Open();
        return conn;
    }

}
