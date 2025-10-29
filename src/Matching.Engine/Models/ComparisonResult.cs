namespace Matching.Engine.Models;

public class ComparisonResult
{
    public string MatchingKey { get; set; }
    public dynamic Record { get; set; }
    public Dictionary<string, IEnumerable<MatcherResult>> Comparisons { get; set; } = [];
}
