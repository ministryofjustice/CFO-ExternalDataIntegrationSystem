using Infrastructure.Contexts;
using Infrastructure.Entities.Delius;

namespace Infrastructure.Repositories.Delius;

public class DeliusRepository(DeliusContext context) : IDeliusRepository
{

    public async Task<Disposal> GetDisposalAsync(int offenderId)
    {
        return await context.Disposals.FirstOrDefaultAsync(s => s.OffenderId == offenderId);
    }

    public async Task<Offender> GetByCrnAsync(string crn)
    {
        return await context.Offenders
            .Include(e => e.AdditionalIdentifiers)
            .Include(e => e.AliasDetails)
            .Include(e => e.Disabilities)
            .Include(e => e.Disposals)
            .Include(e => e.EventDetails)
            .Include(e => e.MainOffences)
            .Include(e => e.OAs)
            .Include(e => e.Addresses)
            .Include(e => e.Transfers)
            .Include(e => e.PersonalCircumstances)
            .Include(e => e.Provisions)
            .Include(e => e.RegistrationDetails)
            .Include(e => e.Requirements)
            .AsSplitQuery()
            .SingleAsync(e => crn == e.Crn);
    }

    public async Task<Offender> GetByIdAsync(int id)
    {
        return await context.Offenders
            .Include(e => e.AdditionalIdentifiers)
            .Include(e => e.AliasDetails)
            .Include(e => e.Disabilities)
            .Include(e => e.Disposals)
            .Include(e => e.EventDetails)
            .Include(e => e.MainOffences)
            .Include(e => e.OAs)
            .Include(e => e.Addresses)
            .Include(e => e.Transfers)
            .Include(e => e.PersonalCircumstances)
            .Include(e => e.Provisions)
            .Include(e => e.RegistrationDetails)
            .Include(e => e.Requirements)
            .AsSplitQuery()
            .SingleAsync(e => id == e.OffenderId);
    }

}
