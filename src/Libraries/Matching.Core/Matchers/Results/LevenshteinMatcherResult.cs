namespace Matching.Core.Matchers.Results;

public class LevenshteinMatcherResult : MatcherResult, ILevenshteinStringMatcherResult
{
    public int LevenshteinEditDistance { get; set; }
}

public interface ILevenshteinStringMatcherResult
{
    public int LevenshteinEditDistance { get; set; }

}