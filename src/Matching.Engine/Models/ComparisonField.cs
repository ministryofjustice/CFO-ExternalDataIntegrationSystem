namespace Matching.Engine.Models;

public record ComparisonField
{
    public string Key { get; set; }
    public string Source { get; set; }
    public string Target { get; set; }
    public string[] Comparators { get; set; } = [];
}
