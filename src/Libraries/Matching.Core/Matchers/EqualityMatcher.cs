using Matching.Core.Attributes;
using Matching.Core.Matchers.Results;
using Matching.Core.Utils;

namespace Matching.Core.Matchers;

[Matcher("Equality")]
public class EqualityMatcher : Matcher<string, EqualityMatcherResult>
{
    protected override EqualityMatcherResult Match(string? source, string? target)
    {
        var result = new EqualityMatcherResult()
        {
            Source = source,
            Target = target,
            Equal = StringUtils.Equal(source, target)
        };

        return result;
    }
}
