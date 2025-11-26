namespace Infrastructure.Entities.Delius;

public partial class Offender
{
    public long OffenderId { get; set; }

    public long Id { get; set; }

    public string? FirstName { get; set; }

    public string? SecondName { get; set; }

    public string? ThirdName { get; set; }

    public string? Surname { get; set; }

    public string? PreviousSurname { get; set; }

    public string? TitleCode { get; set; }

    public string? TitleDescription { get; set; }

    public string Crn { get; set; }

    public string? Cro { get; set; }

    public string? Nomisnumber { get; set; }

    public string? Pncnumber { get; set; }

    public string? Nino { get; set; }

    public string? ImmigrationNumber { get; set; }

    public string? GenderCode { get; set; }

    public string? GenderDescription { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string? NationalityCode { get; set; }

    public string? NationalityDescription { get; set; }

    public string? SecondNationalityCode { get; set; }

    public string? SecondNationalityDescription { get; set; }

    public string? ImmigrationStatusCode { get; set; }

    public string? ImmigrationStatusDescription { get; set; }

    public string? EthnicityCode { get; set; }

    public string? EthnicityDescription { get; set; }

    public string? PrisonNumber { get; set; }

    public string Deleted { get; set; }

    public bool IsActive => Disposals.Any(e => e.TerminationDate is null);

    #region Relationships
    public ICollection<AdditionalIdentifier> AdditionalIdentifiers { get; set; } = [];
    public ICollection<AliasDetail> AliasDetails { get; set; } = [];
    public ICollection<Disability> Disabilities { get; set; } = [];
    public ICollection<Disposal> Disposals { get; set; } = [];
    public ICollection<EventDetail> EventDetails { get; set; } = [];
    public ICollection<MainOffence> MainOffences { get; set; } = [];
    public ICollection<Oa> OAs { get; set; } = [];
    public ICollection<OffenderAddress> Addresses { get; set; } = [];
    public ICollection<OffenderTransfer> Transfers { get; set; } = [];
    public ICollection<OffenderToOffenderManagerMapping> OffenderToOffenderManagerMappings { get; set; } = [];
    public ICollection<PersonalCircumstance> PersonalCircumstances { get; set; } = [];
    public ICollection<Provision> Provisions { get; set; } = [];
    public ICollection<RegistrationDetail> RegistrationDetails { get; set; } = [];
    public ICollection<Requirement> Requirements { get; set; } = [];
    #endregion
}