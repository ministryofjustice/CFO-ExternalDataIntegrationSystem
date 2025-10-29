namespace Matching.Engine.Repositories;

public interface IMatchingRepository
{
    public Task<IEnumerable<dynamic>> GetAllAsync();
    public Task InsertAsync(int candidateId, double probability, string json);
    public Task BulkInsertAsync(Dictionary<ComparisonResult, double> records);
}
