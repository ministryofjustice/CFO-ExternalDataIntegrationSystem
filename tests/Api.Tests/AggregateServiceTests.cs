using API.Extensions;
using Infrastructure.Contexts;
using Infrastructure.Entities.Aggregation;
using Infrastructure.Entities.Clustering;
using Infrastructure.Entities.Delius;
using Infrastructure.Entities.Offloc;
using Infrastructure.Repositories.Clustering;
using Infrastructure.Services.Aggregation;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests;

public class AggregateServiceTests : IDisposable
{
    private readonly ClusteringContext _clusteringContext;
    private readonly DeliusContext _deliusContext;
    private readonly OfflocContext _offlocContext;
    private readonly ClusteringRepository _clusteringRepository;
    private readonly AggregateService _service;
    private readonly string _dbName;

    public AggregateServiceTests()
    {
        _dbName = $"AggregateServiceTestDb_{Guid.NewGuid()}";

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
        _service = new AggregateService(_clusteringRepository, _deliusContext, _offlocContext);
    }

    [Fact]
    public async Task GetClusterAggregateAsync_WithInvalidUpci_ReturnsNull()
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
        var result = await _service.GetClusterAggregateAsync("INVALID");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetClusterAggregateAsync_WithEmptyCluster_ReturnsNull()
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
        var result = await _service.GetClusterAggregateAsync("UPCI001");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetClusterAggregateAsync_WithNomisOnly_ReturnsAggregateWithNomisData()
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
            SecondName = "Michael",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 5, 15),
            Gender = "M",
            Nationality = "British",
            EthnicGroup = "White",
            IsActive = true,
            OffenderAgencies = new List<OffenderAgency>
            {
                new OffenderAgency { NomsNumber = "A1234BC", EstablishmentCode = "BMI", IsActive = true }
            },
            Pncs = new List<Pnc>
            {
                new Pnc { NomsNumber = "A1234BC", Details = "2020/1234567A", IsActive = true }
            },
            SexOffenders = new List<SexOffender>
            {
                new SexOffender { NomsNumber = "A1234BC", Schedule1Sexoffender = "Yes", IsActive = true }
            }
        };

        _clusteringContext.Clusters.Add(cluster);
        _offlocContext.PersonalDetails.Add(personalDetail);
        await _clusteringContext.SaveChangesAsync();
        await _offlocContext.SaveChangesAsync();

        // Act
        var result = await _service.GetClusterAggregateAsync("UPCI001");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("UPCI001", result.Identifier);
        Assert.Equal("NOMIS", result.Primary);
        Assert.Equal("A1234BC", result.NomisNumber);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Michael", result.SecondName);
        Assert.Equal("Smith", result.LastName);
        Assert.Equal(new DateOnly(1990, 5, 15), result.DateOfBirth);
        Assert.Equal("M", result.Gender);
        Assert.Equal("British", result.Nationality);
        Assert.Equal("White", result.Ethnicity);
        Assert.Equal("BMI", result.EstCode);
        Assert.Equal("2020/1234567A", result.PncNumber);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetClusterAggregateAsync_WithDeliusOnly_ReturnsAggregateWithDeliusData()
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
            SecondName = "Elizabeth",
            Surname = "Doe",
            DateOfBirth = new DateOnly(1985, 3, 20),
            GenderDescription = "F",
            NationalityDescription = "British",
            EthnicityDescription = "Asian",
            Pncnumber = "2019/9876543B",
            Deleted = "N",
            Disposals = new List<Disposal>
            {
                new Disposal { OffenderId = 1, EventId = 1, TerminationDate = null, Deleted = "N" }
            },
            OffenderToOffenderManagerMappings = new List<OffenderToOffenderManagerMapping>
            {
                new OffenderToOffenderManagerMapping { OffenderId = 1, OmCode = "OM001", TeamCode = "T01", OrgCode = "N01", EndDate = null }
            },
            RegistrationDetails = new List<RegistrationDetail>
            {
                new RegistrationDetail
                {
                    OffenderId = 1,
                    TypeDescription = "MAPPA",
                    CategoryDescription = "Level 1",
                    RegisterDescription = "High Risk",
                    Date = new DateOnly(2020, 1, 1),
                    DeRegistered = "N"
                }
            }
        };

        _clusteringContext.Clusters.Add(cluster);
        _deliusContext.Offenders.Add(offender);
        await _clusteringContext.SaveChangesAsync();
        await _deliusContext.SaveChangesAsync();

        // Act
        var result = await _service.GetClusterAggregateAsync("UPCI002");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("UPCI002", result.Identifier);
        Assert.Equal("DELIUS", result.Primary);
        Assert.Equal("CRN001", result.Crn);
        Assert.Equal("Jane", result.FirstName);
        Assert.Equal("Elizabeth", result.SecondName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal(new DateOnly(1985, 3, 20), result.DateOfBirth);
        Assert.Equal("F", result.Gender);
        Assert.Equal("British", result.Nationality);
        Assert.Equal("Asian", result.Ethnicity);
        Assert.Equal("N01", result.OrgCode);
        Assert.Equal("2019/9876543B", result.PncNumber);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetClusterAggregateAsync_WithBothNomisAndDelius_MergesDataCorrectly()
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
            Nationality = "British",
            IsActive = true
        };

        var offender = new Offender
        {
            OffenderId = 1,
            Id = 1,
            Crn = "CRN001",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 5, 15),
            GenderDescription = "Male",
            NationalityDescription = "British",
            Deleted = "N"
        };

        _clusteringContext.Clusters.Add(cluster);
        _offlocContext.PersonalDetails.Add(personalDetail);
        _deliusContext.Offenders.Add(offender);
        await _clusteringContext.SaveChangesAsync();
        await _offlocContext.SaveChangesAsync();
        await _deliusContext.SaveChangesAsync();

        // Act
        var result = await _service.GetClusterAggregateAsync("UPCI003");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("UPCI003", result.Identifier);
        Assert.Equal("NOMIS", result.Primary); // NOMIS is active
        Assert.Equal("A1234BC", result.NomisNumber);
        Assert.Equal("CRN001", result.Crn);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Smith", result.LastName);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetClusterAggregateAsync_WithInactiveNomisAndActiveDelius_PrioritizesDelius()
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
            IsActive = false
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
                new Disposal { OffenderId = 1, EventId = 1, TerminationDate = null, Deleted = "N" }
            }
        };

        _clusteringContext.Clusters.Add(cluster);
        _offlocContext.PersonalDetails.Add(personalDetail);
        _deliusContext.Offenders.Add(offender);
        await _clusteringContext.SaveChangesAsync();
        await _offlocContext.SaveChangesAsync();
        await _deliusContext.SaveChangesAsync();

        // Act
        var result = await _service.GetClusterAggregateAsync("UPCI004");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("DELIUS", result.Primary);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetClusterAggregateAsync_WithStickyLocation_IncludesStickyLocation()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI005",
            RecordCount = 1,
            Members = new List<ClusterMembership>
            {
                new ClusterMembership { ClusterId = 1, NodeName = "NOMIS", NodeKey = "A1234BC" }
            }
        };

        var stickyLocation = new StickyLocation
        {
            Upci = "UPCI005",
            OrgCode = "ORG123"
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
        _clusteringContext.StickyLocations.Add(stickyLocation);
        _offlocContext.PersonalDetails.Add(personalDetail);
        await _clusteringContext.SaveChangesAsync();
        await _offlocContext.SaveChangesAsync();

        // Act
        var result = await _service.GetClusterAggregateAsync("UPCI005");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ORG123", result.StickyLocation);
    }

    [Fact]
    public async Task GetClusterAggregateAsync_WithRegistrationDetails_IncludesFormattedDetails()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI006",
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
                new Disposal { OffenderId = 1, EventId = 1, TerminationDate = null, Deleted = "N" }
            },
            RegistrationDetails = new List<RegistrationDetail>
            {
                new RegistrationDetail
                {
                    OffenderId = 1,
                    TypeDescription = "MAPPA",
                    CategoryDescription = "Level 1",
                    RegisterDescription = "High Risk",
                    Date = new DateOnly(2020, 1, 1),
                    DeRegistered = "Y"
                }
            }
        };

        _clusteringContext.Clusters.Add(cluster);
        _deliusContext.Offenders.Add(offender);
        await _clusteringContext.SaveChangesAsync();
        await _deliusContext.SaveChangesAsync();

        // Act
        var result = await _service.GetClusterAggregateAsync("UPCI006");

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.RegistrationDetails);
        var registrationDetail = result.RegistrationDetails.First();
        Assert.Contains("DELIUS", registrationDetail);
        Assert.Contains("MAPPA", registrationDetail);
        Assert.Contains("Inactive", registrationDetail);
    }

    [Fact]
    public async Task GetClusterAggregateAsync_WithMultipleNomisRecords_MergesAllData()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI007",
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
            Nationality = "British",
            IsActive = true
        };

        var personalDetail2 = new PersonalDetail
        {
            NomsNumber = "A5678DE",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 5, 15),
            Gender = "M",
            EthnicGroup = "White",
            IsActive = false
        };

        _clusteringContext.Clusters.Add(cluster);
        _offlocContext.PersonalDetails.AddRange(personalDetail1, personalDetail2);
        await _clusteringContext.SaveChangesAsync();
        await _offlocContext.SaveChangesAsync();

        // Act
        var result = await _service.GetClusterAggregateAsync("UPCI007");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("A1234BC", result.NomisNumber); // Active record preferred
        Assert.Equal("British", result.Nationality);
        Assert.Equal("White", result.Ethnicity);
    }

    [Fact]
    public async Task GetClusterAggregateAsync_WithNullableFields_HandlesNullsCorrectly()
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
        var result = await _service.GetClusterAggregateAsync("UPCI008");

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.SecondName);
        Assert.Null(result.Nationality);
        Assert.Null(result.Ethnicity);
        Assert.Null(result.EstCode);
        Assert.Null(result.PncNumber);
    }

    [Fact]
    public async Task GetClusterAggregateAsync_WithMultipleActiveNomisRecords_SelectsPrimaryByHierarchy()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI009",
            RecordCount = 3,
            Members = new List<ClusterMembership>
            {
                new ClusterMembership { ClusterId = 1, NodeName = "NOMIS", NodeKey = "A1111AA" },
                new ClusterMembership { ClusterId = 1, NodeName = "NOMIS", NodeKey = "A3333CC" },
                new ClusterMembership { ClusterId = 1, NodeName = "NOMIS", NodeKey = "A2222BB" }
            }
        };

        // Create 3 active NOMIS records - OrderByHierarchy will use NomisNumber as final tiebreaker (descending)
        var personalDetail1 = new PersonalDetail
        {
            NomsNumber = "A1111AA",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 5, 15),
            Gender = "M",
            Nationality = "British",
            IsActive = true
        };

        var personalDetail2 = new PersonalDetail
        {
            NomsNumber = "A2222BB",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 5, 15),
            Gender = "M",
            Nationality = "Irish",
            IsActive = true
        };

        var personalDetail3 = new PersonalDetail
        {
            NomsNumber = "A3333CC",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 5, 15),
            Gender = "M",
            Nationality = "Welsh",
            IsActive = true
        };

        _clusteringContext.Clusters.Add(cluster);
        _offlocContext.PersonalDetails.AddRange(personalDetail1, personalDetail2, personalDetail3);
        await _clusteringContext.SaveChangesAsync();
        await _offlocContext.SaveChangesAsync();

        // Act
        var result = await _service.GetClusterAggregateAsync("UPCI009");

        // Assert
        Assert.NotNull(result);
        // OrderByHierarchy sorts by: IsActive (desc) -> Primary is NOMIS (desc) -> ValidFrom (desc) -> NomisNumber (desc)
        // Since all are active NOMIS with same ValidFrom (default), A3333CC should win (highest NomisNumber)
        Assert.Equal("A3333CC", result.NomisNumber);
        Assert.Equal("Welsh", result.Nationality); // Should get nationality from the primary record
        Assert.True(result.IsActive);
        Assert.Equal("NOMIS", result.Primary);
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
