using Blocking.ConfigurationModels;
using Microsoft.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Blocking;

public class DatabaseInsert(
    IConfiguration configuration,
    IOptions<StoredProceduresConfig> storedProceduresOptions,
    IOptions<BlockingQueriesConfig> blockingQueriesOptions)
{
    public async Task ClearTables()
    {
        var connectionString = configuration.GetConnectionString("MatchingDb") 
            ?? throw new InvalidOperationException("Connection string 'MatchingDb' is not configured.");

        SqlConnection conn = new SqlConnection(connectionString);
		
		await conn.OpenAsync();

		using (conn)
		{
			SqlCommand command = new SqlCommand(storedProceduresOptions.Value.TruncateStoredProcedure, conn);
			command.CommandTimeout = 120;
			command.CommandType = CommandType.StoredProcedure;

			try
			{
				await command.ExecuteNonQueryAsync();
			}
			catch (SqlException ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}
	}

    public async Task InsertCandidates()
    {
		var blockingQueriesGroups = blockingQueriesOptions.Value.BlockingQueriesGroups;

        if (blockingQueriesGroups == null || blockingQueriesGroups.Length == 0)
		{
			//throw exception? or publish a different message? or may be next phase
			return;
		}

		var connectionString = configuration.GetConnectionString("MatchingDb") 
            ?? throw new InvalidOperationException("Connection string 'MatchingDb' is not configured.");

		var jsonQueries = JsonConvert.SerializeObject(blockingQueriesGroups);
		SqlConnection conn = new SqlConnection(connectionString);

		await conn.OpenAsync();

		using (conn)
		{
			SqlCommand command = new SqlCommand(storedProceduresOptions.Value.InsertCandidatesStoredProcedure, conn);
			command.CommandTimeout = 600;
			command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@jsonQueries", jsonQueries);

            try
			{
				await command.ExecuteNonQueryAsync();
			}
			catch (SqlException ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}
	}

}