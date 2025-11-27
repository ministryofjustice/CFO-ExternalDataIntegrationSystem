using API.Endpoints;
using API.Services;
using Infrastructure.Contexts;
using Infrastructure.Entities.Clustering;
using Infrastructure.Repositories.Clustering;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests;

public class SearchEndpointsTests : IDisposable
{
    private readonly ClusteringContext _clusteringContext;
    private readonly ClusteringRepository _clusteringRepository;
    private readonly ApiServices _apiServices;
    private readonly string _dbName;

    public SearchEndpointsTests()
    {
        _dbName = $"SearchEndpointsTestDb_{Guid.NewGuid()}";

        var clusteringOptions = new DbContextOptionsBuilder<ClusteringContext>()
            .UseInMemoryDatabase($"{_dbName}_Clustering")
            .Options;

        _clusteringContext = new ClusteringContext(clusteringOptions);
        _clusteringRepository = new ClusteringRepository(_clusteringContext);
        _apiServices = new ApiServices(_clusteringRepository, null!, null!, null!, null!, null!);
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ReturnsNotFound()
    {
        // Arrange - no data in database
        var identifier = "A1234BC";
        var lastName = "Smith";
        var dateOfBirth = new DateOnly(1990, 5, 15);

        // Act
        var result = await SearchEndpoints.SearchAsync(_apiServices, identifier, lastName, dateOfBirth);

        // Assert
        Assert.IsType<NotFound>(result);
    }

    [Fact]
    public async Task SearchAsync_WithExactIdentifierMatch_ReturnsMatchWithPrecedence1()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI001",
            RecordCount = 1,
            Attributes = new List<ClusterAttribute>
            {
                new ClusterAttribute
                {
                    ClusterId = 1,
                    UPCI = "UPCI001",
                    Identifier = "A1234BC",
                    RecordSource = "NOMIS",
                    LastName = "Smith",
                    DateOfBirth = new DateOnly(1990, 5, 15)
                }
            }
        };

        _clusteringContext.Clusters.Add(cluster);
        await _clusteringContext.SaveChangesAsync();

        // Act - search with exact match on all fields
        var result = await SearchEndpoints.SearchAsync(
            _apiServices,
            identifier: "A1234BC",
            lastName: "Smith",
            dateOfBirth: new DateOnly(1990, 5, 15));

        // Assert
        var okResult = Assert.IsType<Ok<IOrderedEnumerable<SearchResult>>>(result);
        var searchResults = okResult.Value!.ToList();
        Assert.Single(searchResults);
        Assert.Equal("UPCI001", searchResults[0].Upci);
        Assert.Equal(1, searchResults[0].Precedence); // Identical match on all fields
    }

    [Fact]
    public async Task SearchAsync_WithNameAndDobMatch_ReturnsMatch()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI002",
            RecordCount = 1,
            Attributes = new List<ClusterAttribute>
            {
                new ClusterAttribute
                {
                    ClusterId = 1,
                    UPCI = "UPCI002",
                    Identifier = "A9999XX",
                    RecordSource = "NOMIS",
                    LastName = "Jones",
                    DateOfBirth = new DateOnly(1985, 3, 20)
                }
            }
        };

        _clusteringContext.Clusters.Add(cluster);
        await _clusteringContext.SaveChangesAsync();

        // Act - search by name and DOB (different identifier)
        var result = await SearchEndpoints.SearchAsync(
            _apiServices,
            identifier: "A1111AA",
            lastName: "Jones",
            dateOfBirth: new DateOnly(1985, 3, 20));

        // Assert
        var okResult = Assert.IsType<Ok<IOrderedEnumerable<SearchResult>>>(result);
        var searchResults = okResult.Value!.ToList();
        Assert.Single(searchResults);
        Assert.Equal("UPCI002", searchResults[0].Upci);
        Assert.Equal(10, searchResults[0].Precedence); // Different identifier, Identical name, Identical DOB
    }

    [Fact]
    public async Task SearchAsync_WithMultipleAttributesForSameUpci_ReturnsMinimumPrecedence()
    {
        // Arrange - one cluster with multiple attributes
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI003",
            RecordCount = 2,
            Attributes = new List<ClusterAttribute>
            {
                new ClusterAttribute
                {
                    ClusterId = 1,
                    UPCI = "UPCI003",
                    Identifier = "A1234BC",
                    RecordSource = "NOMIS",
                    LastName = "Smith",
                    DateOfBirth = new DateOnly(1990, 5, 15)
                },
                new ClusterAttribute
                {
                    ClusterId = 1,
                    UPCI = "UPCI003",
                    Identifier = "CRN001",
                    RecordSource = "DELIUS",
                    LastName = "Smyth", // Similar but not identical
                    DateOfBirth = new DateOnly(1990, 5, 15)
                }
            }
        };

        _clusteringContext.Clusters.Add(cluster);
        await _clusteringContext.SaveChangesAsync();

        // Act - search for exact NOMIS identifier
        var result = await SearchEndpoints.SearchAsync(
            _apiServices,
            identifier: "A1234BC",
            lastName: "Smith",
            dateOfBirth: new DateOnly(1990, 5, 15));

        // Assert - should get minimum precedence (best match) from the two attributes
        var okResult = Assert.IsType<Ok<IOrderedEnumerable<SearchResult>>>(result);
        var searchResults = okResult.Value!.ToList();
        Assert.Single(searchResults); // Grouped by UPCI
        Assert.Equal("UPCI003", searchResults[0].Upci);
        Assert.Equal(1, searchResults[0].Precedence); // Best match wins (exact NOMIS match)
    }

    [Fact]
    public async Task SearchAsync_WithMultipleClusters_ReturnsAllOrderedByPrecedence()
    {
        // Arrange - multiple clusters with different match quality
        var cluster1 = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI001",
            RecordCount = 1,
            Attributes = new List<ClusterAttribute>
            {
                new ClusterAttribute
                {
                    ClusterId = 1,
                    UPCI = "UPCI001",
                    Identifier = "A1234BC",
                    RecordSource = "NOMIS",
                    LastName = "Smith",
                    DateOfBirth = new DateOnly(1990, 5, 15)
                }
            }
        };

        var cluster2 = new Cluster
        {
            ClusterId = 2,
            UPCI = "UPCI002",
            RecordCount = 1,
            Attributes = new List<ClusterAttribute>
            {
                new ClusterAttribute
                {
                    ClusterId = 2,
                    UPCI = "UPCI002",
                    Identifier = "A5678DE",
                    RecordSource = "NOMIS",
                    LastName = "Smith",
                    DateOfBirth = new DateOnly(1990, 5, 15)
                }
            }
        };

        var cluster3 = new Cluster
        {
            ClusterId = 3,
            UPCI = "UPCI003",
            RecordCount = 1,
            Attributes = new List<ClusterAttribute>
            {
                new ClusterAttribute
                {
                    ClusterId = 3,
                    UPCI = "UPCI003",
                    Identifier = "A9999XX",
                    RecordSource = "NOMIS",
                    LastName = "Smythe",
                    DateOfBirth = new DateOnly(1990, 5, 15)
                }
            }
        };

        _clusteringContext.Clusters.AddRange(cluster1, cluster2, cluster3);
        await _clusteringContext.SaveChangesAsync();

        // Act
        var result = await SearchEndpoints.SearchAsync(
            _apiServices,
            identifier: "A1234BC",
            lastName: "Smith",
            dateOfBirth: new DateOnly(1990, 5, 15));

        // Assert
        var okResult = Assert.IsType<Ok<IOrderedEnumerable<SearchResult>>>(result);
        var searchResults = okResult.Value!.ToList();
        Assert.Equal(2, searchResults.Count); // Only UPCI001 and UPCI002 match (cluster3 doesn't match identifier or name+DOB)
        
        // Should be ordered by precedence (best match first)
        Assert.Equal("UPCI001", searchResults[0].Upci);
        Assert.Equal(1, searchResults[0].Precedence); // Exact match on all fields
        
        Assert.Equal("UPCI002", searchResults[1].Upci);
        Assert.Equal(10, searchResults[1].Precedence); // Different identifier, same name and DOB
    }

    [Fact]
    public async Task SearchAsync_WithSimilarName_CalculatesCorrectPrecedence()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI004",
            RecordCount = 1,
            Attributes = new List<ClusterAttribute>
            {
                new ClusterAttribute
                {
                    ClusterId = 1,
                    UPCI = "UPCI004",
                    Identifier = "A1234BC",
                    RecordSource = "NOMIS",
                    LastName = "Johnny", // Similar to "John"
                    DateOfBirth = new DateOnly(1990, 5, 15)
                }
            }
        };

        _clusteringContext.Clusters.Add(cluster);
        await _clusteringContext.SaveChangesAsync();

        // Act
        var result = await SearchEndpoints.SearchAsync(
            _apiServices,
            identifier: "A1234BC",
            lastName: "John",
            dateOfBirth: new DateOnly(1990, 5, 15));

        // Assert
        var okResult = Assert.IsType<Ok<IOrderedEnumerable<SearchResult>>>(result);
        var searchResults = okResult.Value!.ToList();
        Assert.Single(searchResults);
        Assert.Equal("UPCI004", searchResults[0].Upci);
        Assert.Equal(2, searchResults[0].Precedence); // Identical identifier, Similar name, Identical DOB
    }

    [Fact]
    public async Task SearchAsync_WithSimilarDate_CalculatesCorrectPrecedence()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI005",
            RecordCount = 1,
            Attributes = new List<ClusterAttribute>
            {
                new ClusterAttribute
                {
                    ClusterId = 1,
                    UPCI = "UPCI005",
                    Identifier = "A1234BC",
                    RecordSource = "NOMIS",
                    LastName = "Smith",
                    DateOfBirth = new DateOnly(1990, 11, 10) // Similar to 1990-10-11 (transposed day/month)
                }
            }
        };

        _clusteringContext.Clusters.Add(cluster);
        await _clusteringContext.SaveChangesAsync();

        // Act
        var result = await SearchEndpoints.SearchAsync(
            _apiServices,
            identifier: "A1234BC",
            lastName: "Smith",
            dateOfBirth: new DateOnly(1990, 10, 11));

        // Assert
        var okResult = Assert.IsType<Ok<IOrderedEnumerable<SearchResult>>>(result);
        var searchResults = okResult.Value!.ToList();
        Assert.Single(searchResults);
        Assert.Equal("UPCI005", searchResults[0].Upci);
        Assert.Equal(3, searchResults[0].Precedence); // Identical identifier, Identical name, Similar DOB
    }

    [Fact]
    public async Task SearchAsync_WithIdentifierOnlyMatch_ReturnsResult()
    {
        // Arrange
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI006",
            RecordCount = 1,
            Attributes = new List<ClusterAttribute>
            {
                new ClusterAttribute
                {
                    ClusterId = 1,
                    UPCI = "UPCI006",
                    Identifier = "A1234BC",
                    RecordSource = "NOMIS",
                    LastName = "Jones",
                    DateOfBirth = new DateOnly(1985, 1, 1)
                }
            }
        };

        _clusteringContext.Clusters.Add(cluster);
        await _clusteringContext.SaveChangesAsync();

        // Act - matching identifier only
        var result = await SearchEndpoints.SearchAsync(
            _apiServices,
            identifier: "A1234BC",
            lastName: "Smith",
            dateOfBirth: new DateOnly(1990, 5, 15));

        // Assert
        var okResult = Assert.IsType<Ok<IOrderedEnumerable<SearchResult>>>(result);
        var searchResults = okResult.Value!.ToList();
        Assert.Single(searchResults);
        Assert.Equal("UPCI006", searchResults[0].Upci);
        Assert.Equal(11, searchResults[0].Precedence); // Identical identifier, Different name, Different DOB
    }

    [Fact]
    public async Task SearchAsync_WithMultipleSourcesInSameCluster_GroupsCorrectly()
    {
        // Arrange - cluster with both NOMIS and DELIUS attributes
        var cluster = new Cluster
        {
            ClusterId = 1,
            UPCI = "UPCI007",
            RecordCount = 2,
            Attributes = new List<ClusterAttribute>
            {
                new ClusterAttribute
                {
                    ClusterId = 1,
                    UPCI = "UPCI007",
                    Identifier = "A1234BC",
                    RecordSource = "NOMIS",
                    LastName = "Smith",
                    DateOfBirth = new DateOnly(1990, 5, 15)
                },
                new ClusterAttribute
                {
                    ClusterId = 1,
                    UPCI = "UPCI007",
                    Identifier = "CRN001",
                    RecordSource = "DELIUS",
                    LastName = "Smith",
                    DateOfBirth = new DateOnly(1990, 5, 15)
                }
            }
        };

        _clusteringContext.Clusters.Add(cluster);
        await _clusteringContext.SaveChangesAsync();

        // Act - search with CRN
        var result = await SearchEndpoints.SearchAsync(
            _apiServices,
            identifier: "CRN001",
            lastName: "Smith",
            dateOfBirth: new DateOnly(1990, 5, 15));

        // Assert
        var okResult = Assert.IsType<Ok<IOrderedEnumerable<SearchResult>>>(result);
        var searchResults = okResult.Value!.ToList();
        Assert.Single(searchResults); // Both attributes grouped into one UPCI result
        Assert.Equal("UPCI007", searchResults[0].Upci);
        Assert.Equal(1, searchResults[0].Precedence); // Best match (exact CRN match)
    }

    public void Dispose()
    {
        _clusteringContext.Database.EnsureDeleted();
        _clusteringContext.Dispose();
    }
}
