namespace Infrastructure.DTOs;

public class NodeMetadataDto
{
    public required string Key { get; set; }
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public required string LastName { get; set; }
    public string? Gender { get; set; }
    public required DateOnly DateOfBirth { get; set; }
    public string[] CroNumbers { get; set; } = [];
    public string[] PncNumbers { get; set; } = [];
    public string[] NomisNumbers { get; set; } = [];
    public bool IsActive { get; set; }
}
