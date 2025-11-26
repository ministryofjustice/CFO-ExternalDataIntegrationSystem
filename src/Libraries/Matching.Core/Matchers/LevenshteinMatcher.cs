using Matching.Core.Attributes;
using Matching.Core.Matchers.Results;
using Matching.Core.Utils;

namespace Matching.Core.Matchers;

[Matcher("Levenshtein")]
public class LevenshteinMatcher : Matcher<string, LevenshteinMatcherResult>
{
    protected override LevenshteinMatcherResult Match(string? source, string? target)
    {
        return new LevenshteinMatcherResult
        {
            LevenshteinEditDistance = LevenshteinUtils.CalculateDistance(source, target),
            Source = source,
            Target = target
        };
    }
}
