namespace Infrastructure.Entities.Offloc;

public partial class PersonalDetail
{
    public string NomsNumber { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? SecondName { get; set; }

    public string? Surname { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public string Gender { get; set; } = null!;

    public string? MaternityStatus { get; set; }

    public string? Nationality { get; set; }

    public string? Religion { get; set; }

    public string? MaritalStatus { get; set; }

    public string? EthnicGroup { get; set; }

    public bool IsActive { get; set; }

    #region Relationships
    public ICollection<Activity> Activities { get; set; } = [];
    public ICollection<Address> Addresses { get; set; } = [];
    public ICollection<Assessment> Assessments { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<Employment> Employments { get; set; } = [];
    public ICollection<Flag> Flags { get; set; } = [];
    public ICollection<Identifier> Identifiers { get; set; } = [];
    public ICollection<IncentiveLevel> IncentiveLevels { get; set; } = [];
    public ICollection<Location> Locations { get; set; } = [];
    public ICollection<MainOffence> MainOffences { get; set; } = [];
    public ICollection<Movement> Movements { get; set; } = [];
    public ICollection<OffenderAgency> OffenderAgencies { get; set; } = [];
    public ICollection<OffenderStatus> Statuses { get; set; } = [];
    public ICollection<OtherOffence> OtherOffences { get; set; } = [];
    public ICollection<Pnc> Pncs { get; set; } = [];
    public ICollection<PreviousPrisonNumber> PreviousPrisonNumbers { get; set; } = [];
    public ICollection<SentenceInformation> SentenceInformation { get; set; } = [];
    public ICollection<SexOffender> SexOffenders { get; set; } = [];
    public ICollection<VeteranFlagLog> VeteranFlagLogs { get; set; } = [];
    #endregion
}
