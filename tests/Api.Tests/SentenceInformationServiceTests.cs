using API.Services.SentenceInformation;
using Infrastructure.Contexts;
using Infrastructure.Entities.Clustering;
using Infrastructure.Entities.Delius;
using Infrastructure.Entities.Offloc;
using Infrastructure.Repositories.Clustering;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests;

public class SentenceInformationServiceTests : IDisposable
{
    private readonly ClusteringContext _clusteringContext;
    private readonly DeliusContext _deliusContext;
    private readonly OfflocContext _offlocContext;
    private readonly ClusteringRepository _clusteringRepository;
    private readonly SentenceInformationService _service;
    private readonly string _dbName;

    public SentenceInformationServiceTests()
    {
        _dbName = $"SentenceInfoTestDb_{Guid.NewGuid()}";

        var clusteringOptions = new DbContextOptionsBuilder<ClusteringContext>()
            .UseInMemoryDatabase($"{_dbName}_Clustering")
            .Options;

        var deliusOptions = new DbContextOptionsBuilder<DeliusContext>()
            .UseInMemoryDatabase($"{_dbName}_Delius")
            .Options;

        var offlocOptions = new DbContextOptionsBuilder<OfflocContext>()
            .UseInMemoryDatabase($"{_dbName}_Offloc")
            .Options;

        _clusteringContext = new ClusteringContext(clusteringOptions);
        _deliusContext = new DeliusContext(deliusOptions);
        _offlocContext = new OfflocContext(offlocOptions);

        _clusteringRepository = new ClusteringRepository(_clusteringContext);
        _service = new SentenceInformationService(_clusteringRepository, _deliusContext, _offlocContext);
    }

    [Fact]
    public async Task GetSentenceInformationAsyncFull_WithInvalidUpci_ReturnsNull()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI001",
            RecordCount = 0
        };

        _clusteringContext.Clusters.Add(cluster);
        await _clusteringContext.SaveChangesAsync();

        // Act
        var result = await _service.GetSentenceInformationAsyncFull("INVALID");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetSentenceInformationAsyncFull_WithEmptyCluster_ReturnsNull()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI001",
            RecordCount = 0,
            Members = []
        };

        _clusteringContext.Clusters.Add(cluster);
        await _clusteringContext.SaveChangesAsync();

        // Act
        var result = await _service.GetSentenceInformationAsyncFull("UPCI001");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetSentenceInformationAsyncFull_WithNomisOnly_ReturnsSentenceInformationWithNomisData()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI001",
            RecordCount = 1,
            Members = new List<ClusterMembership>
            {
                new ClusterMembership { ClusterId = 1, NodeName = "NOMIS", NodeKey = "A1234BC" }
            }
        };

        var personalDetail = new PersonalDetail
        {
            NomsNumber = "A1234BC",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 5, 15),
            Gender = "M",
            IsActive = true,
            Pncs = new List<Pnc>
            {
                new Pnc { NomsNumber = "A1234BC", Details = "2020/1234567A", IsActive = true }
            },
            Bookings = new List<Booking>
            {
                new Booking { NomsNumber = "A1234BC", PrisonNumber = "B001", IsActive = true }
            },
            SentenceInformation = new List<Infrastructure.Entities.Offloc.SentenceInformation>
            {
                new Infrastructure.Entities.Offloc.SentenceInformation { NomsNumber = "A1234BC", SentenceYears = 5, IsActive = true }
            },
            Locations = new List<Location>
            {
                new Location { NomsNumber = "A1234BC", Location1 = "BMI", IsActive = true }
            },
            SexOffenders = new List<SexOffender>
            {
                new SexOffender { NomsNumber = "A1234BC", Schedule1Sexoffender = "Yes", IsActive = true }
            },
            Assessments = new List<Assessment>
            {
                new Assessment { NomsNumber = "A1234BC", SecurityCategory = "C", IsActive = true }
            }
        };

        _clusteringContext.Clusters.Add(cluster);
        _offlocContext.PersonalDetails.Add(personalDetail);
        await _clusteringContext.SaveChangesAsync();
        await _offlocContext.SaveChangesAsync();

        // Act
        var result = await _service.GetSentenceInformationAsyncFull("UPCI001");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        
        var info = result.First();
        Assert.Equal("UPCI001", info.UPCI);
        Assert.Equal("A1234BC", info.NomsNumber);
        Assert.Single(info.Pncs);
        Assert.Single(info.Bookings);
        Assert.Single(info.SentenceInformation);
        Assert.Single(info.Locations);
        Assert.Single(info.SexOffenders);
        Assert.Single(info.Assessments);
        Assert.Null(info.Crn);
    }

    [Fact]
    public async Task GetSentenceInformationAsyncFull_WithDeliusOnly_ReturnsSentenceInformationWithDeliusData()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI002",
            RecordCount = 1,
            Members = new List<ClusterMembership>
            {
                new ClusterMembership { ClusterId = 1, NodeName = "DELIUS", NodeKey = "CRN001" }
            }
        };

        var offender = new Offender
        {
            OffenderId = 1,
            Id = 1,
            Crn = "CRN001",
            FirstName = "Jane",
            Surname = "Doe",
            DateOfBirth = new DateOnly(1985, 3, 20),
            Pncnumber = "2019/9876543B",
            Deleted = "N",
            Disposals = new List<Disposal>
            {
                new Disposal { OffenderId = 1, EventId = 1, DisposalDetail = "Community Order", Deleted = "N" }
            },
            EventDetails = new List<EventDetail>
            {
                new EventDetail { OffenderId = 1, Id = 1, Deleted = "N" }
            },
            MainOffences = new List<Infrastructure.Entities.Delius.MainOffence>
            {
                new Infrastructure.Entities.Delius.MainOffence { OffenderId = 1, EventId = 1, OffenceDescription = "Theft", Deleted = "N" }
            }
        };

        _clusteringContext.Clusters.Add(cluster);
        _deliusContext.Offenders.Add(offender);
        await _clusteringContext.SaveChangesAsync();
        await _deliusContext.SaveChangesAsync();

        // Act
        var result = await _service.GetSentenceInformationAsyncFull("UPCI002");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        
        var info = result.First();
        Assert.Equal("UPCI002", info.UPCI);
        Assert.Equal("CRN001", info.Crn);
        Assert.Equal("2019/9876543B", info.PncNumber);
        Assert.NotNull(info.Disposals);
        Assert.NotEmpty(info.Disposals!);
        Assert.Single(info.EventDetails);
        Assert.Single(info.MainOffences);
        Assert.Null(info.NomsNumber);
    }

    [Fact]
    public async Task GetSentenceInformationAsyncFull_WithBothNomisAndDelius_ReturnsSingleRecordWithBothSets()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI003",
            RecordCount = 2,
            Members = new List<ClusterMembership>
            {
                new ClusterMembership { ClusterId = 1, NodeName = "NOMIS", NodeKey = "A1234BC" },
                new ClusterMembership { ClusterId = 1, NodeName = "DELIUS", NodeKey = "CRN001" }
            }
        };

        var personalDetail = new PersonalDetail
        {
            NomsNumber = "A1234BC",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 5, 15),
            Gender = "M",
            IsActive = true,
            Bookings = new List<Booking>
            {
                new Booking { NomsNumber = "A1234BC", PrisonNumber = "B001", IsActive = true }
            }
        };

        var offender = new Offender
        {
            OffenderId = 1,
            Id = 1,
            Crn = "CRN001",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 5, 15),
            Deleted = "N",
            Disposals = new List<Disposal>
            {
                new Disposal { OffenderId = 1, EventId = 1, DisposalDetail = "Community Order", TerminationDate = null, Deleted = "N" }
            }
        };

        _clusteringContext.Clusters.Add(cluster);
        _offlocContext.PersonalDetails.Add(personalDetail);
        _deliusContext.Offenders.Add(offender);
        await _clusteringContext.SaveChangesAsync();
        await _offlocContext.SaveChangesAsync();
        await _deliusContext.SaveChangesAsync();

        // Act
        var result = await _service.GetSentenceInformationAsyncFull("UPCI003");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        
        var info = result.First();
        Assert.Equal("UPCI003", info.UPCI);
        Assert.Equal("A1234BC", info.NomsNumber);
        Assert.Equal("CRN001", info.Crn);
        Assert.Single(info.Bookings);
        Assert.NotNull(info.Disposals);
        Assert.NotEmpty(info.Disposals!);
    }

    [Fact]
    public async Task GetSentenceInformationAsyncFull_WithMultipleNomisRecords_ReturnsMultipleRecords()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI004",
            RecordCount = 2,
            Members = new List<ClusterMembership>
            {
                new ClusterMembership { ClusterId = 1, NodeName = "NOMIS", NodeKey = "A1234BC" },
                new ClusterMembership { ClusterId = 1, NodeName = "NOMIS", NodeKey = "A5678DE" }
            }
        };

        var personalDetail1 = new PersonalDetail
        {
            NomsNumber = "A1234BC",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 5, 15),
            Gender = "M",
            IsActive = true
        };

        var personalDetail2 = new PersonalDetail
        {
            NomsNumber = "A5678DE",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 5, 15),
            Gender = "M",
            IsActive = false
        };

        _clusteringContext.Clusters.Add(cluster);
        _offlocContext.PersonalDetails.AddRange(personalDetail1, personalDetail2);
        await _clusteringContext.SaveChangesAsync();
        await _offlocContext.SaveChangesAsync();

        // Act
        var result = await _service.GetSentenceInformationAsyncFull("UPCI004");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.NomsNumber == "A1234BC");
        Assert.Contains(result, r => r.NomsNumber == "A5678DE");
        Assert.Equal("A1234BC", result[0].NomsNumber); // Active record first
    }

    [Fact]
    public async Task GetSentenceInformationAsyncFull_WithMultipleDeliusRecords_ReturnsMultipleRecords()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI005",
            RecordCount = 2,
            Members = new List<ClusterMembership>
            {
                new ClusterMembership { ClusterId = 1, NodeName = "DELIUS", NodeKey = "CRN001" },
                new ClusterMembership { ClusterId = 1, NodeName = "DELIUS", NodeKey = "CRN002" }
            }
        };

        var offender1 = new Offender
        {
            OffenderId = 1,
            Id = 1,
            Crn = "CRN001",
            FirstName = "Jane",
            Surname = "Doe",
            DateOfBirth = new DateOnly(1985, 3, 20),
            Deleted = "N",
            Disposals = new List<Disposal>
            {
                new Disposal { OffenderId = 1, EventId = 1, DisposalDetail = "Active Order", TerminationDate = null, Deleted = "N" }
            }
        };

        var offender2 = new Offender
        {
            OffenderId = 2,
            Id = 2,
            Crn = "CRN002",
            FirstName = "Jane",
            Surname = "Doe",
            DateOfBirth = new DateOnly(1985, 3, 20),
            Deleted = "N"
        };

        _clusteringContext.Clusters.Add(cluster);
        _deliusContext.Offenders.AddRange(offender1, offender2);
        await _clusteringContext.SaveChangesAsync();
        await _deliusContext.SaveChangesAsync();

        // Act
        var result = await _service.GetSentenceInformationAsyncFull("UPCI005");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Crn == "CRN001");
        Assert.Contains(result, r => r.Crn == "CRN002");
        Assert.Equal("CRN001", result[0].Crn); // Active record first
    }

    [Fact]
    public async Task GetSentenceInformationAsyncFull_SortsRecordsByActiveStatusFirst()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI006",
            RecordCount = 3,
            Members = new List<ClusterMembership>
            {
                new ClusterMembership { ClusterId = 1, NodeName = "NOMIS", NodeKey = "A1111AA" },
                new ClusterMembership { ClusterId = 1, NodeName = "NOMIS", NodeKey = "A2222BB" },
                new ClusterMembership { ClusterId = 1, NodeName = "NOMIS", NodeKey = "A3333CC" }
            }
        };

        var personalDetail1 = new PersonalDetail
        {
            NomsNumber = "A1111AA",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 5, 15),
            Gender = "M",
            IsActive = false
        };

        var personalDetail2 = new PersonalDetail
        {
            NomsNumber = "A2222BB",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 5, 15),
            Gender = "M",
            IsActive = true
        };

        var personalDetail3 = new PersonalDetail
        {
            NomsNumber = "A3333CC",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 5, 15),
            Gender = "M",
            IsActive = false
        };

        _clusteringContext.Clusters.Add(cluster);
        _offlocContext.PersonalDetails.AddRange(personalDetail1, personalDetail2, personalDetail3);
        await _clusteringContext.SaveChangesAsync();
        await _offlocContext.SaveChangesAsync();

        // Act
        var result = await _service.GetSentenceInformationAsyncFull("UPCI006");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("A2222BB", result[0].NomsNumber); // Active record is first
    }

    [Fact]
    public async Task GetSentenceInformationAsyncFull_WithMismatchedCounts_CreatesCorrectNumberOfRecords()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI007",
            RecordCount = 4,
            Members = new List<ClusterMembership>
            {
                new ClusterMembership { ClusterId = 1, NodeName = "NOMIS", NodeKey = "A1234BC" },
                new ClusterMembership { ClusterId = 1, NodeName = "DELIUS", NodeKey = "CRN001" },
                new ClusterMembership { ClusterId = 1, NodeName = "DELIUS", NodeKey = "CRN002" },
                new ClusterMembership { ClusterId = 1, NodeName = "DELIUS", NodeKey = "CRN003" }
            }
        };

        var personalDetail = new PersonalDetail
        {
            NomsNumber = "A1234BC",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 5, 15),
            Gender = "M",
            IsActive = true
        };

        var offender1 = new Offender { OffenderId = 1, Id = 1, Crn = "CRN001", FirstName = "John", Surname = "Smith", DateOfBirth = new DateOnly(1990, 5, 15), Deleted = "N", Disposals = new List<Disposal> { new Disposal { OffenderId = 1, EventId = 1, TerminationDate = null, Deleted = "N" } } };
        var offender2 = new Offender { OffenderId = 2, Id = 2, Crn = "CRN002", FirstName = "John", Surname = "Smith", DateOfBirth = new DateOnly(1990, 5, 15), Deleted = "N" };
        var offender3 = new Offender { OffenderId = 3, Id = 3, Crn = "CRN003", FirstName = "John", Surname = "Smith", DateOfBirth = new DateOnly(1990, 5, 15), Deleted = "N" };

        _clusteringContext.Clusters.Add(cluster);
        _offlocContext.PersonalDetails.Add(personalDetail);
        _deliusContext.Offenders.AddRange(offender1, offender2, offender3);
        await _clusteringContext.SaveChangesAsync();
        await _offlocContext.SaveChangesAsync();
        await _deliusContext.SaveChangesAsync();

        // Act
        var result = await _service.GetSentenceInformationAsyncFull("UPCI007");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count); // Max of 1 NOMIS and 3 DELIUS
        Assert.Equal("A1234BC", result[0].NomsNumber);
        Assert.Equal("CRN001", result[0].Crn);
        Assert.Null(result[1].NomsNumber); // Second record has no NOMIS data
        Assert.NotNull(result[1].Crn);
        Assert.Null(result[2].NomsNumber);
        Assert.NotNull(result[2].Crn);
    }

    [Fact]
    public async Task GetSentenceInformationAsyncFull_WithEmptyCollections_ReturnsEmptyCollections()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI008",
            RecordCount = 1,
            Members = new List<ClusterMembership>
            {
                new ClusterMembership { ClusterId = 1, NodeName = "NOMIS", NodeKey = "A1234BC" }
            }
        };

        var personalDetail = new PersonalDetail
        {
            NomsNumber = "A1234BC",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 5, 15),
            Gender = "M",
            IsActive = true
        };

        _clusteringContext.Clusters.Add(cluster);
        _offlocContext.PersonalDetails.Add(personalDetail);
        await _clusteringContext.SaveChangesAsync();
        await _offlocContext.SaveChangesAsync();

        // Act
        var result = await _service.GetSentenceInformationAsyncFull("UPCI008");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        
        var info = result.First();
        Assert.Empty(info.Pncs);
        Assert.Empty(info.Bookings);
        Assert.Empty(info.SentenceInformation);
        Assert.Empty(info.Locations);
        Assert.Empty(info.SexOffenders);
        Assert.Empty(info.Assessments);
    }

    [Fact]
    public async Task GetSentenceInformationAsyncFull_OnlyIncludesActiveDisposals()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI009",
            RecordCount = 1,
            Members = new List<ClusterMembership>
            {
                new ClusterMembership { ClusterId = 1, NodeName = "DELIUS", NodeKey = "CRN001" }
            }
        };

        var offender = new Offender
        {
            OffenderId = 1,
            Id = 1,
            Crn = "CRN001",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 5, 15),
            Deleted = "N",
            Disposals = new List<Disposal>
            {
                new Disposal { Id = 1, OffenderId = 1, EventId = 1, DisposalDetail = "Active Order", TerminationDate = null, Deleted = "N" },
                new Disposal { Id = 2, OffenderId = 1, EventId = 2, DisposalDetail = "Terminated Order", TerminationDate = new DateOnly(2023, 1, 1), Deleted = "N" }
            }
        };

        _clusteringContext.Clusters.Add(cluster);
        _deliusContext.Offenders.Add(offender);
        await _clusteringContext.SaveChangesAsync();
        await _deliusContext.SaveChangesAsync();

        // Act
        var result = await _service.GetSentenceInformationAsyncFull("UPCI009");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        
        var info = result.First();
        Assert.NotNull(info.Disposals);
        Assert.NotEmpty(info.Disposals!);
        Assert.Equal("Active Order", info.Disposals!.First().DisposalDetail);
    }

    public void Dispose()
    {
        _clusteringContext.Database.EnsureDeleted();
        _deliusContext.Database.EnsureDeleted();
        _offlocContext.Database.EnsureDeleted();
        _clusteringContext.Dispose();
        _deliusContext.Dispose();
        _offlocContext.Dispose();
    }
}
