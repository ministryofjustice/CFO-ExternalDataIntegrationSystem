using System.Collections.Concurrent;

namespace Matching.Engine;

public class MatchingQueue
{
    public List<ComparisonResult> Results { get; set; } = [];
}
