namespace API.DTOs.Offloc;

public class MainOffenceDto
{
    public string? MainOffence { get; set; }

    public DateOnly? DateFirstConviction { get; set; }

    public bool IsActive { get; set; }
}