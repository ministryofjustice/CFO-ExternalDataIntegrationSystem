using Matching.Core.Attributes;
using Matching.Core.Matchers.Results;
using Matching.Core.Utils;

namespace Matching.Core.Matchers;

[Matcher("JaroWinkler")]
public class JaroWinklerMatcher : Matcher<string, JaroWinklerMatcherResult>
{
    protected override JaroWinklerMatcherResult Match(string? source, string? target)
    {
        var result = new JaroWinklerMatcherResult
        {
            JaroWinklerSimilarity = JaroWinklerUtils.GetJaroWinklerSimilarity(source, target),
            Source = source,
            Target = target
        };

        return result;
    }
}

