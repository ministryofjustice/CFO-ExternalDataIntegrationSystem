namespace Meow.Features.Participants.Events;

public class ParticipantCreatedIntegrationEvent
{
    public string ParticipantId { get; set; } = string.Empty;
    public string? PrimaryRecordKeyAtCreation { get; set; }
    public DateTime OccurredOn { get; set; }
}
