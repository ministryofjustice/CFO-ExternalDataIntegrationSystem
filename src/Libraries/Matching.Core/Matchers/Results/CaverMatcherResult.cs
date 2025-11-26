namespace Matching.Core.Matchers.Results;

public class CaverMatcherResult : MatcherResult, ICaverMatcherResult
{
    public bool IsPhoneticallySimilar { get; set; }
}

public interface ICaverMatcherResult
{
    public bool IsPhoneticallySimilar { get; set; }
}