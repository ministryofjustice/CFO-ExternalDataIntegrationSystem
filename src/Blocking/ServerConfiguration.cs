
using Blocking.ConfigurationModels;

namespace Blocking;

public class ServerConfiguration
{
	public string connectionString = string.Empty;
	public StoredProceduresConfig storedProceduresConfig = new();
    public BlockingQueriesConfig blockingQueriesConfig = new();
}
