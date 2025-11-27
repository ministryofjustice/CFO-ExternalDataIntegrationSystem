using Infrastructure.Contexts;
using Infrastructure.Entities.Clustering;
using Infrastructure.Repositories.Clustering;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests;

public class ClusteringRepositoryTests : IDisposable
{
    private readonly ClusteringContext _context;
    private readonly ClusteringRepository _repository;
    private readonly string _dbName;

    public ClusteringRepositoryTests()
    {
        _dbName = $"ClusteringTestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<ClusteringContext>()
            .UseInMemoryDatabase(_dbName)
            .Options;

        _context = new ClusteringContext(options);
        _repository = new ClusteringRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsClusterWithMembers()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI001",
            RecordCount = 2,
            ContainsInternalDupe = false,
            ContainsLowProbabilityMembers = false,
            Members = new List<ClusterMembership>
            {
                new ClusterMembership { ClusterId = 1, NodeName = "Offloc", NodeKey = "A12345" },
                new ClusterMembership { ClusterId = 1, NodeName = "Delius", NodeKey = "CRN001" }
            }
        };

        _context.Clusters.Add(cluster);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.ClusterId);
        Assert.Equal("UPCI001", result.UPCI);
        Assert.Equal(2, result.Members.Count);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var cluster = new Cluster { ClusterId = 1, UPCI = "UPCI001", RecordCount = 0 };
        _context.Clusters.Add(cluster);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUpciAsync_WithValidUpci_ReturnsCluster()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI-TEST-001",
            RecordCount = 1,
            Members = new List<ClusterMembership>
            {
                new ClusterMembership { ClusterId = 1, NodeName = "Offloc", NodeKey = "A12345" }
            }
        };

        _context.Clusters.Add(cluster);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUpciAsync("UPCI-TEST-001");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("UPCI-TEST-001", result.UPCI);
        Assert.Single(result.Members);
    }

    [Fact]
    public async Task GetByUpciAsync_WithInvalidUpci_ReturnsNull()
    {
        // Arrange
        var cluster = new Cluster { ClusterId = 1, UPCI = "UPCI001", RecordCount = 0 };
        _context.Clusters.Add(cluster);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUpciAsync("INVALID-UPCI");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingIdentifier_ReturnsAttributes()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI001",
            RecordCount = 0,
            Attributes = new List<ClusterAttribute>
            {
                new ClusterAttribute
                {
                    ClusterId = 1,
                    UPCI = "UPCI001",
                    RecordSource = "Offloc",
                    Identifier = "A12345",
                    LastName = "Smith",
                    DateOfBirth = new DateOnly(1990, 1, 1)
                }
            }
        };

        _context.Clusters.Add(cluster);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.SearchAsync("A12345", "Jones", new DateOnly(1985, 5, 5));

        // Assert
        Assert.Single(results);
        Assert.Equal("A12345", results[0].Identifier);
        Assert.Equal("UPCI001", results[0].UPCI);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingLastNameAndDob_ReturnsAttributes()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI001",
            RecordCount = 0,
            Attributes = new List<ClusterAttribute>
            {
                new ClusterAttribute
                {
                    ClusterId = 1,
                    UPCI = "UPCI001",
                    RecordSource = "Delius",
                    Identifier = "CRN001",
                    LastName = "Johnson",
                    DateOfBirth = new DateOnly(1985, 6, 15)
                }
            }
        };

        _context.Clusters.Add(cluster);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.SearchAsync("A99999", "Johnson", new DateOnly(1985, 6, 15));

        // Assert
        Assert.Single(results);
        Assert.Equal("Johnson", results[0].LastName);
        Assert.Equal(new DateOnly(1985, 6, 15), results[0].DateOfBirth);
    }

    [Fact]
    public async Task SearchAsync_WithBothMatches_ReturnsUnionOfResults()
    {
        // Arrange
        var cluster1 = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI001",
            RecordCount = 0,
            Attributes = new List<ClusterAttribute>
            {
                new ClusterAttribute
                {
                    ClusterId = 1,
                    UPCI = "UPCI001",
                    RecordSource = "Offloc",
                    Identifier = "A12345",
                    LastName = "Smith",
                    DateOfBirth = new DateOnly(1990, 1, 1)
                }
            }
        };

        var cluster2 = new Cluster
        {
            ClusterId = 2,
            UPCI = "UPCI002",
            RecordCount = 0,
            Attributes = new List<ClusterAttribute>
            {
                new ClusterAttribute
                {
                    ClusterId = 2,
                    UPCI = "UPCI002",
                    RecordSource = "Delius",
                    Identifier = "CRN002",
                    LastName = "Smith",
                    DateOfBirth = new DateOnly(1990, 1, 1)
                }
            }
        };

        _context.Clusters.AddRange(cluster1, cluster2);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.SearchAsync("A12345", "Smith", new DateOnly(1990, 1, 1));

        // Assert
        Assert.Equal(2, results.Length);
        Assert.Contains(results, r => r.Identifier == "A12345");
        Assert.Contains(results, r => r.Identifier == "CRN002");
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ReturnsEmptyArray()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI001",
            RecordCount = 0,
            Attributes = new List<ClusterAttribute>
            {
                new ClusterAttribute
                {
                    ClusterId = 1,
                    UPCI = "UPCI001",
                    RecordSource = "Offloc",
                    Identifier = "A12345",
                    LastName = "Smith",
                    DateOfBirth = new DateOnly(1990, 1, 1)
                }
            }
        };

        _context.Clusters.Add(cluster);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.SearchAsync("B99999", "Jones", new DateOnly(1985, 5, 5));

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public async Task GetStickyLocation_WithValidUpci_ReturnsOrgCode()
    {
        // Arrange
        var stickyLocation = new StickyLocation
        {
            Upci = "UPCI001",
            OrgCode = "ORG123"
        };

        _context.StickyLocations.Add(stickyLocation);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetStickyLocation("UPCI001");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ORG123", result);
    }

    [Fact]
    public async Task GetStickyLocation_WithInvalidUpci_ReturnsNull()
    {
        // Arrange
        var stickyLocation = new StickyLocation { Upci = "UPCI001", OrgCode = "ORG123" };
        _context.StickyLocations.Add(stickyLocation);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetStickyLocation("INVALID-UPCI");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GenerateClusterAsync_WithAvailableUpci_CreatesNewCluster()
    {
        // Arrange
        var existingCluster = new Cluster { ClusterId = 1, UPCI = "UPCI001", RecordCount = 1 };
        var upci2Record = new UPCI2 { ClusterId = 2, Upci = "UPCI002" };

        _context.Clusters.Add(existingCluster);
        _context.UPCI2s.Add(upci2Record);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GenerateClusterAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.ClusterId);
        Assert.Equal("UPCI002", result.UPCI);
        Assert.Equal(0, result.RecordCount);
        Assert.False(result.ContainsInternalDupe);
        Assert.False(result.ContainsLowProbabilityMembers);
    }

    [Fact]
    public async Task GenerateClusterAsync_WithNoAvailableUpci_ReturnsNull()
    {
        // Arrange
        var existingCluster = new Cluster { ClusterId = 1, UPCI = "UPCI001", RecordCount = 1 };
        _context.Clusters.Add(existingCluster);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GenerateClusterAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GenerateClusterAsync_SavesNewClusterToDatabase()
    {
        // Arrange
        var upci2Record = new UPCI2 { ClusterId = 1, Upci = "UPCI-NEW-001" };
        _context.UPCI2s.Add(upci2Record);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GenerateClusterAsync();

        // Assert
        var savedCluster = await _context.Clusters.FindAsync(1);
        Assert.NotNull(savedCluster);
        Assert.Equal("UPCI-NEW-001", savedCluster.UPCI);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
