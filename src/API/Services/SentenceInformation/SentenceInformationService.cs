using Infrastructure.Entities.SentenceInformation;
using Infrastructure.Repositories.Clustering;

namespace API.Services.SentenceInformation
{
    public class SentenceInformationService(IClusteringRepository ClusteringRepository, DeliusContext DeliusContext, OfflocContext OfflocContext)
    {
        public async Task<List<SentenceInformationFull>?> GetSentenceInformationAsyncFull(string UPCI)
        {
            var cluster = await ClusteringRepository.GetByUpciAsync(UPCI);

            if (cluster is null || cluster.Members.Count is 0)
            {
                return null;
            }

            var nomisIds = cluster.Members
                .Where(record => record.NodeName is "NOMIS")
                .Select(record => record.NodeKey);

            var deliusIds = cluster.Members
                .Where(record => record.NodeName is "DELIUS")
                .Select(record => record.NodeKey);

            var nomisRecords = await OfflocContext.PersonalDetails
                .Include(x => x.Pncs)
                .Include(x => x.Bookings)
                .Include(x => x.SentenceInformation)
                .Include(x => x.Locations)
                .Include(x => x.SexOffenders)
                .Include(x => x.Assessments)
                .Where
                (
                    n => nomisIds.Contains(n.NomsNumber)
                )
                .AsSplitQuery()
                .ToListAsync();

            var deliusRecords = await DeliusContext.Offenders
                .Include(x => x.OffenderToOffenderManagerMappings)            
                .Include(d => d.Disposals.Where(d => d.TerminationDate == null))
                .Include(x => x.EventDetails)
                .Include(x => x.MainOffences)            
                .Where
                (
                    n => deliusIds.Contains(n.Crn)    
                )
                .AsSplitQuery()
                .ToListAsync();

            var sortedNomisRecords = nomisRecords
                .OrderByDescending(x => x.IsActive)
                .ThenByDescending(x => OfflocContext.Entry(x).Property<DateTime>("ValidFrom").CurrentValue)
                .ThenByDescending(x => x.NomsNumber)
                .ToList();

            var sortedDeliusRecords = deliusRecords                
                .OrderByDescending(x => x.IsActive)
                .ThenByDescending(x => DeliusContext.Entry(x).Property<DateTime>("ValidFrom").CurrentValue)
                .ThenByDescending(x => x.Crn)
                .ToList();

            var maxRecords = Math.Max(sortedNomisRecords.Count, sortedDeliusRecords.Count);

            List<SentenceInformationFull> sentenceInformationList = new(maxRecords);

            for (int i = 0; i < maxRecords; i++)
            {
                var nomisRecord = sortedNomisRecords.ElementAtOrDefault(i);
                var deliusRecord = sortedDeliusRecords.ElementAtOrDefault(i);

                var sentenceInformation = new SentenceInformationFull
                {
                    UPCI = UPCI,
                    NomsNumber = nomisRecord?.NomsNumber,
                    Pncs = nomisRecord?.Pncs ?? [],
                    Bookings = nomisRecord?.Bookings ?? [],
                    SentenceInformation = nomisRecord?.SentenceInformation ?? [],
                    Locations = nomisRecord?.Locations ?? [],
                    SexOffenders = nomisRecord?.SexOffenders ?? [],
                    Assessments = nomisRecord?.Assessments ?? [],
                    Crn = deliusRecord?.Crn,
                    PncNumber = deliusRecord?.Pncnumber,
                    Disposals = deliusRecord?.Disposals,
                    EventDetails = deliusRecord?.EventDetails ?? [],
                    MainOffences = deliusRecord?.MainOffences ?? []

                };
               
                sentenceInformationList.Add(sentenceInformation);
            }
            
            return sentenceInformationList;
        }
    }
}