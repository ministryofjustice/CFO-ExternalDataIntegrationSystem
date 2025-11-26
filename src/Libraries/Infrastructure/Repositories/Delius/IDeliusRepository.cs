using Infrastructure.Entities.Delius;

namespace Infrastructure.Repositories.Delius;

public interface IDeliusRepository
{
    public Task<Offender> GetByIdAsync(int id);
    public Task<Offender> GetByCrnAsync(string crn);    
}
