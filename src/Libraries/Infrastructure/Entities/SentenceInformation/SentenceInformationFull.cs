using Infrastructure.Entities.Delius;
using Infrastructure.Entities.Offloc;

namespace Infrastructure.Entities.SentenceInformation;

public class SentenceInformationFull
{
    public required string UPCI { get; set; } = null!;    

    public string? NomsNumber { get; set; } = null!;

    public ICollection<Pnc> Pncs { get; set; } = [];

    public ICollection<Booking> Bookings { get; set; } = [];

    public ICollection<Offloc.SentenceInformation> SentenceInformation { get; set; } = [];
    
    public ICollection<Location> Locations { get; set; } = [];

    public ICollection<SexOffender> SexOffenders { get; set; } = [];

    public ICollection<Assessment> Assessments { get; set; } = [];

    public string? Crn { get; set; }

    public string? PncNumber { get; set; }

    public ICollection<Disposal>? Disposals { get; set; } = [];

    public ICollection<EventDetail> EventDetails { get; set; } = [];
        
    public ICollection<Delius.MainOffence> MainOffences { get; set; } = [];
}
