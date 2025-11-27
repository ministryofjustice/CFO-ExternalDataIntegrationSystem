using Infrastructure.Contexts;
using Infrastructure.Entities.Delius;
using Infrastructure.Repositories.Delius;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests;

public class DeliusRepositoryTests : IDisposable
{
    private readonly DeliusContext _context;
    private readonly DeliusRepository _repository;
    private readonly string _dbName;

    public DeliusRepositoryTests()
    {
        _dbName = $"DeliusTestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<DeliusContext>()
            .UseInMemoryDatabase(_dbName)
            .Options;

        _context = new DeliusContext(options);
        _repository = new DeliusRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsOffender()
    {
        // Arrange
        var offender = new Offender
        {
            OffenderId = 1,
            Id = 1,
            Crn = "CRN001",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1985, 3, 15),
            Deleted = "N"
        };

        _context.Offenders.Add(offender);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.OffenderId);
        Assert.Equal("CRN001", result.Crn);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Smith", result.Surname);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ThrowsException()
    {
        // Arrange
        var offender = new Offender
        {
            OffenderId = 1,
            Id = 1,
            Crn = "CRN001",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1985, 3, 15),
            Deleted = "N"
        };

        _context.Offenders.Add(offender);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _repository.GetByIdAsync(999));
    }

    [Fact]
    public async Task GetByCrnAsync_WithValidCrn_ReturnsOffender()
    {
        // Arrange
        var offender = new Offender
        {
            OffenderId = 1,
            Id = 1,
            Crn = "X123456",
            FirstName = "Jane",
            Surname = "Doe",
            DateOfBirth = new DateOnly(1990, 7, 20),
            Deleted = "N"
        };

        _context.Offenders.Add(offender);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCrnAsync("X123456");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("X123456", result.Crn);
        Assert.Equal("Jane", result.FirstName);
        Assert.Equal("Doe", result.Surname);
    }

    [Fact]
    public async Task GetByCrnAsync_WithInvalidCrn_ThrowsException()
    {
        // Arrange
        var offender = new Offender
        {
            OffenderId = 1,
            Id = 1,
            Crn = "X123456",
            Surname = "Doe",
            DateOfBirth = new DateOnly(1990, 7, 20),
            Deleted = "N"
        };

        _context.Offenders.Add(offender);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _repository.GetByCrnAsync("INVALID"));
    }

    [Fact]
    public async Task GetByCrnAsync_IncludesAllRelatedEntities()
    {
        // Arrange
        var offender = new Offender
        {
            OffenderId = 1,
            Id = 1,
            Crn = "X123456",
            FirstName = "Jane",
            Surname = "Doe",
            DateOfBirth = new DateOnly(1990, 7, 20),
            Deleted = "N",
            AdditionalIdentifiers = new List<AdditionalIdentifier>
            {
                new AdditionalIdentifier { OffenderId = 1, Pnc = "12345/90B", Deleted = "N" }
            },
            Addresses = new List<OffenderAddress>
            {
                new OffenderAddress { OffenderId = 1, BuildingName = "123", StreetName = "Main St", Deleted = "N" }
            },
            MainOffences = new List<MainOffence>
            {
                new MainOffence { OffenderId = 1, EventId = 1, OffenceDescription = "Theft", Deleted = "N" }
            },
            Disposals = new List<Disposal>
            {
                new Disposal { OffenderId = 1, EventId = 1, DisposalDetail = "Community Order", Deleted = "N" }
            }
        };

        _context.Offenders.Add(offender);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCrnAsync("X123456");

        // Assert
        Assert.Single(result.AdditionalIdentifiers);
        Assert.Single(result.Addresses);
        Assert.Single(result.MainOffences);
        Assert.Single(result.Disposals);
    }

    [Fact]
    public async Task GetByIdAsync_IncludesAllRelatedEntities()
    {
        // Arrange
        var offender = new Offender
        {
            OffenderId = 1,
            Id = 1,
            Crn = "X123456",
            FirstName = "Jane",
            Surname = "Doe",
            DateOfBirth = new DateOnly(1990, 7, 20),
            Deleted = "N",
            AliasDetails = new List<AliasDetail>
            {
                new AliasDetail { OffenderId = 1, FirstName = "Janet", Surname = "Doe" }
            },
            Disabilities = new List<Disability>
            {
                new Disability { OffenderId = 1, Deleted = "N" }
            },
            Requirements = new List<Requirement>
            {
                new Requirement { OffenderId = 1, CategoryDescription = "Unpaid Work", Deleted = "N" }
            },
            Transfers = new List<OffenderTransfer>
            {
                new OffenderTransfer { OffenderId = 1, Deleted = "N" }
            }
        };

        _context.Offenders.Add(offender);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.Single(result.AliasDetails);
        Assert.Single(result.Disabilities);
        Assert.Single(result.Requirements);
        Assert.Single(result.Transfers);
    }

    [Fact]
    public async Task GetByCrnAsync_WithNoRelatedEntities_ReturnsEmptyCollections()
    {
        // Arrange
        var offender = new Offender
        {
            OffenderId = 1,
            Id = 1,
            Crn = "X123456",
            FirstName = "Jane",
            Surname = "Doe",
            DateOfBirth = new DateOnly(1990, 7, 20),
            Deleted = "N"
        };

        _context.Offenders.Add(offender);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCrnAsync("X123456");

        // Assert
        Assert.Empty(result.AdditionalIdentifiers);
        Assert.Empty(result.AliasDetails);
        Assert.Empty(result.Disabilities);
        Assert.Empty(result.Disposals);
        Assert.Empty(result.EventDetails);
        Assert.Empty(result.MainOffences);
        Assert.Empty(result.OAs);
        Assert.Empty(result.Addresses);
        Assert.Empty(result.Transfers);
        Assert.Empty(result.PersonalCircumstances);
        Assert.Empty(result.Provisions);
        Assert.Empty(result.RegistrationDetails);
        Assert.Empty(result.Requirements);
    }

    [Fact]
    public async Task GetByCrnAsync_WithComplexData_LoadsAllCorrectly()
    {
        // Arrange
        var offender = new Offender
        {
            OffenderId = 1,
            Id = 1,
            Crn = "X123456",
            FirstName = "John",
            SecondName = "Michael",
            ThirdName = "James",
            Surname = "Smith",
            PreviousSurname = "Jones",
            TitleCode = "MR",
            TitleDescription = "Mr",
            Cro = "CRO123",
            Nomisnumber = "A1234BC",
            Pncnumber = "12345/90A",
            Nino = "AB123456C",
            GenderCode = "M",
            GenderDescription = "Male",
            DateOfBirth = new DateOnly(1985, 3, 15),
            NationalityCode = "GBR",
            NationalityDescription = "British",
            EthnicityCode = "W1",
            EthnicityDescription = "White British",
            Deleted = "N"
        };

        _context.Offenders.Add(offender);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCrnAsync("X123456");

        // Assert
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Michael", result.SecondName);
        Assert.Equal("James", result.ThirdName);
        Assert.Equal("Smith", result.Surname);
        Assert.Equal("Jones", result.PreviousSurname);
        Assert.Equal("A1234BC", result.Nomisnumber);
        Assert.Equal("12345/90A", result.Pncnumber);
        Assert.Equal("British", result.NationalityDescription);
    }

    [Fact]
    public async Task GetByCrnAsync_WithMultipleOffences_LoadsAll()
    {
        // Arrange
        var offender = new Offender
        {
            OffenderId = 1,
            Id = 1,
            Crn = "X123456",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1985, 3, 15),
            Deleted = "N",
            MainOffences = new List<MainOffence>
            {
                new MainOffence { Id = 1, OffenderId = 1, EventId = 1, OffenceDescription = "Theft", Deleted = "N" },
                new MainOffence { Id = 2, OffenderId = 1, EventId = 2, OffenceDescription = "Assault", Deleted = "N" },
                new MainOffence { Id = 3, OffenderId = 1, EventId = 3, OffenceDescription = "Burglary", Deleted = "N" }
            }
        };

        _context.Offenders.Add(offender);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCrnAsync("X123456");

        // Assert
        Assert.Equal(3, result.MainOffences.Count);
        Assert.Contains(result.MainOffences, o => o.OffenceDescription == "Theft");
        Assert.Contains(result.MainOffences, o => o.OffenceDescription == "Assault");
        Assert.Contains(result.MainOffences, o => o.OffenceDescription == "Burglary");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
