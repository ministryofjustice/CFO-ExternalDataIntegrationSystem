namespace API.DTOs.Offloc;

public class SentenceDataDto
{
    public string NomsNumber { get; set; } = default!;
    public SentenceInformationDto[] SentenceInformation { get; set; } = [];
    public MainOffenceDto[] MainOffence { get; set; } = [];
    public OtherOffenceDto[] OtherOffences { get; set; } = [];
}