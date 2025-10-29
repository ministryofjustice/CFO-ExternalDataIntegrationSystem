namespace Matching.Engine.Repositories;

public interface IClusteringRepository
{
    public Task<IEnumerable<dynamic>> GetAllAsync();
    public Task ClusterPostProcessAsync();
    public Task ClusterPreProcessAsync();
    public Task BulkInsertAsync(Dictionary<ComparisonResult, double> records);
}
