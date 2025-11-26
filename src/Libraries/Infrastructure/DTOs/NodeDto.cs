namespace Infrastructure.DTOs;

public class NodeDto
{
    public required string Id { get; set; }
    public required string Group { get; set; }
    public string Type { get; set; } = "node";
    public string? Source { get; set; }
    public bool HardLink { get; set; }
    public NodeMetadataDto? Metadata { get; set; }
}
