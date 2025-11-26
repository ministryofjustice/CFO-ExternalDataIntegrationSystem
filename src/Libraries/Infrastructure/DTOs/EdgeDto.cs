namespace Infrastructure.DTOs;

public class EdgeDto
{
    public required string From { get; set; }
    public required string To { get; set; }
    public required double Probability { get; set; }
}
