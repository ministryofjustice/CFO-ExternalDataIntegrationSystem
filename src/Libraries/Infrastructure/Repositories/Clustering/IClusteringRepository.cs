using Infrastructure.DTOs;
using Infrastructure.Entities.Clustering;

namespace Infrastructure.Repositories.Clustering;

public interface IClusteringRepository
{
    public Task<Cluster?> GetByIdAsync(int id);
    public Task<Cluster?> GetByUpciAsync(string upci);
    public Task<ClusterAttribute[]> SearchAsync(string identifier, string lastName, DateOnly dateOfBirth);

    public Task<string?> GetStickyLocation(string upci);
    public Task<Cluster?> GenerateClusterAsync();

}
