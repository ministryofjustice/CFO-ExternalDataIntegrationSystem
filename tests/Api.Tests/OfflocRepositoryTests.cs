using Infrastructure.Contexts;
using Infrastructure.Entities.Offloc;
using Infrastructure.Repositories.Offloc;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests;

public class OfflocRepositoryTests : IDisposable
{
    private readonly OfflocContext _context;
    private readonly OfflocRepository _repository;
    private readonly string _dbName;

    public OfflocRepositoryTests()
    {
        _dbName = $"OfflocTestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<OfflocContext>()
            .UseInMemoryDatabase(_dbName)
            .Options;

        _context = new OfflocContext(options);
        _repository = new OfflocRepository(_context);
    }

    [Fact]
    public async Task GetByNomsNumberAsync_WithValidNomsNumber_ReturnsPersonalDetail()
    {
        // Arrange
        var personalDetail = new PersonalDetail
        {
            NomsNumber = "A1234BC",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = "M",
            IsActive = true
        };

        _context.PersonalDetails.Add(personalDetail);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNomsNumberAsync("A1234BC");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("A1234BC", result.NomsNumber);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Smith", result.Surname);
    }

    [Fact]
    public async Task GetByNomsNumberAsync_WithInvalidNomsNumber_ThrowsException()
    {
        // Arrange
        var personalDetail = new PersonalDetail
        {
            NomsNumber = "A1234BC",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = "M",
            IsActive = true
        };

        _context.PersonalDetails.Add(personalDetail);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _repository.GetByNomsNumberAsync("INVALID"));
    }

    [Fact]
    public async Task GetByNomsNumberAsync_IncludesAllRelatedEntities()
    {
        // Arrange
        var personalDetail = new PersonalDetail
        {
            NomsNumber = "A1234BC",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = "M",
            IsActive = true,
            Activities = new List<Activity>
            {
                new Activity { NomsNumber = "A1234BC", Activity1 = "Test Activity", Location = "HMP Test", IsActive = true }
            },
            Addresses = new List<Address>
            {
                new Address { NomsNumber = "A1234BC", AddressType = "Home", Address1 = "123 Test St", IsActive = true }
            },
            SentenceInformation = new List<SentenceInformation>
            {
                new SentenceInformation { NomsNumber = "A1234BC", SentenceYears = 5, IsActive = true }
            },
            MainOffences = new List<MainOffence>
            {
                new MainOffence { NomsNumber = "A1234BC", MainOffence1 = "Theft", IsActive = true }
            }
        };

        _context.PersonalDetails.Add(personalDetail);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNomsNumberAsync("A1234BC");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Activities);
        Assert.Single(result.Addresses);
        Assert.Single(result.SentenceInformation);
        Assert.Single(result.MainOffences);
    }

    [Fact]
    public async Task GetByNomsNumberAsync_WithMultipleRelatedEntities_LoadsAll()
    {
        // Arrange
        var personalDetail = new PersonalDetail
        {
            NomsNumber = "A1234BC",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = "M",
            IsActive = true,
            Bookings = new List<Booking>
            {
                new Booking { NomsNumber = "A1234BC", PrisonNumber = "B001", FirstReceptionDate = new DateOnly(2024, 1, 1), IsActive = true },
                new Booking { NomsNumber = "A1234BC", PrisonNumber = "B002", FirstReceptionDate = new DateOnly(2024, 2, 1), IsActive = true }
            },
            Flags = new List<Flag>
            {
                new Flag { NomsNumber = "A1234BC", Details = "Flag 1", IsActive = true },
                new Flag { NomsNumber = "A1234BC", Details = "Flag 2", IsActive = true },
                new Flag { NomsNumber = "A1234BC", Details = "Flag 3", IsActive = true }
            }
        };

        _context.PersonalDetails.Add(personalDetail);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNomsNumberAsync("A1234BC");

        // Assert
        Assert.Equal(2, result.Bookings.Count);
        Assert.Equal(3, result.Flags.Count);
    }

    [Fact]
    public async Task GetByNomsNumberAsync_WithNoRelatedEntities_ReturnsEmptyCollections()
    {
        // Arrange
        var personalDetail = new PersonalDetail
        {
            NomsNumber = "A1234BC",
            FirstName = "John",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = "M",
            IsActive = true
        };

        _context.PersonalDetails.Add(personalDetail);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNomsNumberAsync("A1234BC");

        // Assert
        Assert.Empty(result.Activities);
        Assert.Empty(result.Addresses);
        Assert.Empty(result.Assessments);
        Assert.Empty(result.Bookings);
        Assert.Empty(result.Employments);
        Assert.Empty(result.Flags);
        Assert.Empty(result.Identifiers);
        Assert.Empty(result.IncentiveLevels);
        Assert.Empty(result.Locations);
        Assert.Empty(result.MainOffences);
        Assert.Empty(result.Movements);
        Assert.Empty(result.SentenceInformation);
        Assert.Empty(result.Statuses);
        Assert.Empty(result.OtherOffences);
        Assert.Empty(result.Pncs);
        Assert.Empty(result.PreviousPrisonNumbers);
        Assert.Empty(result.SexOffenders);
        Assert.Empty(result.VeteranFlagLogs);
    }

    [Fact]
    public async Task GetByNomsNumberAsync_WithComplexData_LoadsAllCorrectly()
    {
        // Arrange
        var personalDetail = new PersonalDetail
        {
            NomsNumber = "A1234BC",
            FirstName = "John",
            SecondName = "Michael",
            Surname = "Smith",
            DateOfBirth = new DateOnly(1990, 5, 15),
            Gender = "M",
            MaternityStatus = "N",
            Nationality = "British",
            Religion = "None",
            MaritalStatus = "Single",
            EthnicGroup = "White",
            IsActive = true
        };

        _context.PersonalDetails.Add(personalDetail);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNomsNumberAsync("A1234BC");

        // Assert
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Michael", result.SecondName);
        Assert.Equal("Smith", result.Surname);
        Assert.Equal(new DateOnly(1990, 5, 15), result.DateOfBirth);
        Assert.Equal("British", result.Nationality);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
