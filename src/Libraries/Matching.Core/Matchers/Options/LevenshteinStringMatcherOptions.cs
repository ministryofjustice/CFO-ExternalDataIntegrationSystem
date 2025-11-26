namespace Matching.Core.Matchers.Options;

public class LevenshteinStringMatcherOptions : IMatcherOptions
{
    public int MaximumEditDistance { get; set; }
}
