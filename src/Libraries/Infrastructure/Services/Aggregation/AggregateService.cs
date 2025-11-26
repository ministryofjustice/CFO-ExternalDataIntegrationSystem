using API.Extensions;
using Infrastructure.Contexts;
using Infrastructure.Entities.Aggregation;
using Infrastructure.Entities.Delius;
using Infrastructure.Entities.Offloc;
using Infrastructure.Repositories.Clustering;

namespace Infrastructure.Services.Aggregation;

public class AggregateService(IClusteringRepository ClusteringRepository, DeliusContext DeliusContext, OfflocContext OfflocContext)
{
    public async Task<ClusterAggregate?> GetClusterAggregateAsync(string UPCI)
    {
        var cluster = await ClusteringRepository.GetByUpciAsync(UPCI);

        if(cluster is null || cluster.Members.Count is 0)
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
        .Include(x => x.OffenderAgencies)
        .Include(x => x.Pncs)
        .Include(x => x.SexOffenders)
        .AsSplitQuery()
        .Where
        (
            n => nomisIds.Contains(n.NomsNumber)
        )
        .ToListAsync();

        var deliusRecords = await DeliusContext.Offenders
        .Include(x => x.OffenderToOffenderManagerMappings)
        .Include(x => x.RegistrationDetails)
        .Include(x => x.Disposals)
        .AsSplitQuery()
        .Where
        (
            n => deliusIds.Contains(n.Crn)
        )
        .ToListAsync();

        var aggregate =  Aggregate(UPCI, (nomisRecords, deliusRecords));

        aggregate.StickyLocation = await ClusteringRepository.GetStickyLocation(UPCI);

        return aggregate;
    }

    private ClusterAggregate Aggregate(string UPCI, (IEnumerable<PersonalDetail> Nomis, IEnumerable<Offender> Delius) records)
    {
        var aggregates = records.Nomis.Select(n =>
            new ClusterAggregate
            {
                Identifier = UPCI,
                Primary = "NOMIS",
                NomisNumber = n.NomsNumber,
                FirstName = n.FirstName,
                SecondName = n.SecondName,
                LastName = n.Surname,
                DateOfBirth = n.DateOfBirth,
                ValidFrom = OfflocContext.Entry(n).Property<DateTime>("ValidFrom").CurrentValue,
                ValidTo = OfflocContext.Entry(n).Property<DateTime>("ValidTo").CurrentValue,
                IsActive = n.IsActive,
                Nationality = n.Nationality,
                Gender = n.Gender,
                Ethnicity = n.EthnicGroup,
                EstCode = n.OffenderAgencies
                    .OrderByDescending(x => x.IsActive)
                    .ThenByDescending(x => x.NomsNumber)
                    .FirstOrDefault()?.EstablishmentCode,
                PncNumber = n.Pncs
                    .OrderByDescending(x => x.IsActive)
                    .ThenByDescending(x => x.NomsNumber)
                    .FirstOrDefault()?.Details,
                RegistrationDetails = n.SexOffenders.Select((x) =>
                {
                    return string.Format("NOMIS - {0} ({1})", 
                        x.Schedule1Sexoffender, 
                        x.IsActive ? "Active" : "Inactive");
                })
            })
            .Concat(records.Delius.Select(n =>
                new ClusterAggregate
                {
                    Identifier = UPCI,
                    Primary = "DELIUS",
                    Crn = n.Crn,
                    FirstName = n.FirstName,
                    SecondName = n.SecondName,
                    LastName = n.Surname,
                    DateOfBirth = n.DateOfBirth,
                    ValidFrom = DeliusContext.Entry(n).Property<DateTime>("ValidFrom").CurrentValue,
                    ValidTo = DeliusContext.Entry(n).Property<DateTime>("ValidTo").CurrentValue,
                    IsActive = n.IsActive,
                    Nationality = n.NationalityDescription,
                    Gender = n.GenderDescription,
                    Ethnicity = n.EthnicityDescription,
                    OrgCode = n.OffenderToOffenderManagerMappings.FirstOrDefault(x => x.EndDate is null)?.OrgCode,
                    PncNumber = n.Pncnumber,
                    RegistrationDetails = n.RegistrationDetails.Select(x =>
                    {
                        string formattedDescription = $"{x.CategoryDescription} {x.RegisterDescription}";

                        return string.Format("DELIUS - {0}: {1} - {2} ({3})", 
                            x.TypeDescription,
                            string.IsNullOrWhiteSpace(formattedDescription) ? "No description available" : formattedDescription,
                            x.Date?.ToShortDateString(),
                            x.DeRegistered is "Y" ? "Inactive" : "Active");
                    })
                })
            )
            .OrderByHierarchy(e => e.IsActive)
            .ToList();

        ClusterAggregate primary = aggregates?
            .FirstOrDefault()
            ?? throw new ArgumentNullException();

        var nomisNumber = aggregates
            .OrderByHierarchy(e => e.NomisNumber is not null)?
            .FirstOrDefault()?
            .NomisNumber;

        var crn = aggregates
            .OrderByHierarchy(e => e.Crn is not null)?
            .FirstOrDefault()?
            .Crn;

        var nationality = aggregates
            .OrderByHierarchy(e => e.Nationality is not null)?
            .FirstOrDefault()?
            .Nationality;

        var gender = aggregates
            .OrderByHierarchy(e => e.Gender is not null)?
            .FirstOrDefault()?
            .Gender;

        var ethnicity = aggregates
            .OrderByHierarchy(e => e.Ethnicity is not null)?
            .FirstOrDefault()?
            .Ethnicity;

        var orgCode = aggregates
            .OrderByHierarchy(e => e.OrgCode is not null)?
            .FirstOrDefault()?
            .OrgCode;

        var estCode = aggregates
            .OrderByHierarchy(e => e.EstCode is not null)?
            .FirstOrDefault()?
            .EstCode;

        var pncNumber = aggregates
            .OrderByHierarchy(e => e.PncNumber is not null)?
            .FirstOrDefault()?
            .PncNumber;

        var registrationDetails = aggregates
            .SelectMany(e => e.RegistrationDetails)
            .OrderBy(details => details);


        var aggregate = new ClusterAggregate
        {
            Identifier = UPCI,
            FirstName = primary.FirstName,
            SecondName = primary.SecondName,
            LastName = primary.LastName,
            DateOfBirth = primary.DateOfBirth,
            IsActive = primary.IsActive,
            Primary = primary.Primary,
            NomisNumber = nomisNumber,
            Crn = crn,
            Nationality = nationality,
            Gender = gender,
            Ethnicity = ethnicity,
            EstCode = estCode,
            OrgCode = orgCode,
            PncNumber = pncNumber,
            RegistrationDetails = registrationDetails
        };

        return aggregate;
    }

}
