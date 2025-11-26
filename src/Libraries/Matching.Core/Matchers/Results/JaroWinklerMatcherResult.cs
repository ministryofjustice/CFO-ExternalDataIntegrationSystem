namespace Matching.Core.Matchers.Results;

public class JaroWinklerMatcherResult : MatcherResult, IJaroWinklerStringMatcherResult
{
    public double JaroWinklerSimilarity { get; set; }
    public override bool MissingInSource => Source is null || string.IsNullOrEmpty(Source.ToString());
    public override bool MissingInTarget => Target is null || string.IsNullOrEmpty(Target.ToString());
}

public interface IJaroWinklerStringMatcherResult
{
    public double JaroWinklerSimilarity { get; set; }
}
