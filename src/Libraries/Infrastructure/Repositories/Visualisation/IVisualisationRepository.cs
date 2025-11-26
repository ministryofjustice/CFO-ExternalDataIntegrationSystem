using Infrastructure.DTOs;

namespace Infrastructure.Repositories.Visualisation;

public interface IVisualisationRepository
{
    public Task<ClusterDto?> GetDetailsByUpciAsync(string upci);

    public Task<bool> SaveNetworkAsync(NetworkDto network);
}
