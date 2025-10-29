
using Blocking.ConfigurationModels;

namespace Blocking;

public class StoredProceduresConfig
{
	public string TruncateStoredProcedure { get; set; } = string.Empty;
	public string InsertCandidatesStoredProcedure { get; set; } = string.Empty;
	public BlockingQueriesConfig blockingQueriesConfig { get; set; } = new BlockingQueriesConfig();
}
