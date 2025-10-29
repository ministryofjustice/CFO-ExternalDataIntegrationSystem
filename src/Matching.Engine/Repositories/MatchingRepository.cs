using Dapper;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace Matching.Engine.Repositories;

public class MatchingRepository(
    ILogger<MatchingRepository> logger, 
    IConfiguration configuration) : IMatchingRepository
{ 
    public async Task<IEnumerable<dynamic>> GetAllAsync()
    {
        using var connection = CreateAndOpenConnection();

        string query = "SELECT * FROM Matching.Candidates";

        logger.LogInformation($"Executing query: '{query}'");

        var offenders = await connection.QueryAsync<dynamic>(query, commandTimeout: 180);

        logger.LogInformation($"Query returned {offenders.Count()} records.");

        return offenders;
    }

    public async Task InsertAsync(int candidateId, double probability, string json)
    {
        using var connection = CreateAndOpenConnection();

        string query = "INSERT INTO [MatchingDb].[Matching].[Matches] (CandidateId, Probability, JSON) VALUES (@candidateId, @probability, @json)";

        logger.LogInformation($"Executing query '{query}'");

        await connection.ExecuteAsync(query, new
        {
            candidateId,
            probability,
            json
        });
    }

    public async Task BulkInsertAsync(Dictionary<ComparisonResult, double> records)
    {
        var batches = Batch(records, 1000) ?? [];

        int numOfBatches = batches.Count();

        int concurrency = 16;
        using var semaphore = new SemaphoreSlim(concurrency);

        logger.LogInformation($"Beginning batch insert of {records.Count} records into Matching dataset(s) in {numOfBatches} batches...");

        var tasks = batches.Select(async (batch, batchIndex) =>
        {
            await semaphore.WaitAsync();

            int id = batchIndex + 1;

            logger.LogInformation($"Inserting batch {id} (of {numOfBatches})...");

            string insertQuery = @"
                INSERT INTO [MatchingDb].[Matching].[Matches] (CandidateId, Probability, JSON) 
                VALUES (@CandidateId, @Probability, @Json);";

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

        logger.LogInformation($"Inserted {numOfBatches} batches into Matching dataset(s).");
    }

    private static IEnumerable<IEnumerable<object>> Batch(Dictionary<ComparisonResult, double> records, int batchSize)
    {
        var batchNos = (int)Math.Ceiling((double)records.Count / batchSize);

        for (int i = 0; i < batchNos; i++)
        {
            var batch = records.Skip(i * batchSize).Take(batchSize);

            yield return batch.Select(r => new
            {
                CandidateId = (int)r.Key.Record.CandidateId,
                Probability = r.Value,
                Json = JsonConvert.SerializeObject(r.Key)
            });
        }
    }

    private SqlConnection CreateAndOpenConnection()
    {
        var conn = new SqlConnection(configuration.GetConnectionString("MatchingDb"));
        conn.Open();
        return conn;
    }

}
