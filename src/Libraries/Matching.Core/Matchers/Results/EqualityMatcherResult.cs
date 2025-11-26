namespace Matching.Core.Matchers.Results;

public class EqualityMatcherResult : MatcherResult, IExactMatcherResult
{
    public bool Equal { get; set; }
}

public interface IExactMatcherResult
{
    public bool Equal { get; set; }
}