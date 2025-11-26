using Infrastructure.Contexts;
using Infrastructure.Entities.Offloc;

namespace Infrastructure.Repositories.Offloc;

public class OfflocRepository(OfflocContext context) : IOfflocRepository
{
    public async Task<PersonalDetail> GetByNomsNumberAsync(string nomsNumber)
    {
        return await context.PersonalDetails
            .Include(e => e.Activities)
            .Include(e => e.Addresses)
            .Include(e => e.Assessments)
            .Include(e => e.Bookings)
            .Include(e => e.Employments)
            .Include(e => e.Flags)
            .Include(e => e.Identifiers)
            .Include(e => e.IncentiveLevels)
            .Include(e => e.Locations)
            .Include(e => e.MainOffences)
            .Include(e => e.Movements)
            .Include(e => e.SentenceInformation)
            .Include(e => e.Statuses)
            .Include(e => e.OtherOffences)
            .Include(e => e.Pncs)
            .Include(e => e.PreviousPrisonNumbers)
            .Include(e => e.SexOffenders)
            .Include(e => e.VeteranFlagLogs)
            .AsSplitQuery()
            .SingleAsync(e => nomsNumber == e.NomsNumber);
    }
}
