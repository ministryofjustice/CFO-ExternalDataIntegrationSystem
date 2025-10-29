using Infrastructure.Entities.Offloc;

namespace Infrastructure.Repositories.Offloc;

public interface IOfflocRepository
{
    public Task<PersonalDetail> GetByNomsNumberAsync(string nomsNumber);    
}
