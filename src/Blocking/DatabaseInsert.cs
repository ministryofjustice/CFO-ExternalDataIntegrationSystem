
using Blocking.ConfigurationModels;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlTypes;
using System.Runtime.InteropServices.JavaScript;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace Blocking;

public class DatabaseInsert
{
	private readonly string connString;

    private readonly string truncateProcedure;
    private readonly string insertCandidatesProcedure;
    private readonly BlockingQueriesConfig blockingQueriesConfig;

    public DatabaseInsert(ServerConfiguration config)
    {
        connString = config.connectionString;

        (truncateProcedure, insertCandidatesProcedure, blockingQueriesConfig) =
            (config.storedProceduresConfig.TruncateStoredProcedure,
            config.storedProceduresConfig.InsertCandidatesStoredProcedure,
            config.blockingQueriesConfig);
    }

    public async Task ClearTables()
    {
        SqlConnection conn = new SqlConnection(connString);
		
		await conn.OpenAsync();

		using (conn)
		{
			SqlCommand command = new SqlCommand(truncateProcedure, conn);
			command.CommandTimeout = 120;
			command.CommandType = CommandType.StoredProcedure;

			try
			{
				await command.ExecuteNonQueryAsync();
			}
			catch (SqlException ex)
			{
				Console.WriteLine(ex.ToString());
				throw ex;
			}
		}
	}

    public async Task InsertCandidates()
    {
		var blockingQueriesGroups = blockingQueriesConfig.BlockingQueriesGroups;

        if (blockingQueriesGroups == null || blockingQueriesGroups.Length == 0)
		{
			//throw exception? or publish a different message? or may be next phase
			return;
		}
		var jsonQueries = JsonConvert.SerializeObject(blockingQueriesGroups);
		SqlConnection conn = new SqlConnection(connString);

		await conn.OpenAsync();

		using (conn)
		{
			SqlCommand command = new SqlCommand(insertCandidatesProcedure, conn);
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
				throw ex;
			}
		}
	}

}