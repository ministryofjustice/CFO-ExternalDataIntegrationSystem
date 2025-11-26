using Matching.Core.Attributes;
using Matching.Core.Matchers.Results;
using Matching.Core.Utils;

namespace Matching.Core.Matchers;

[Matcher("Caver")]
public class CaverMatcher : Matcher<string, CaverMatcherResult>
{
    protected override CaverMatcherResult Match(string? source, string? target)
    {
        var result = new CaverMatcherResult()
        {
            IsPhoneticallySimilar = CaverUtils.IsPhoneticallySimilar(source, target),
            Source = source,
            Target = target,
        };

        return result;
    }
}
