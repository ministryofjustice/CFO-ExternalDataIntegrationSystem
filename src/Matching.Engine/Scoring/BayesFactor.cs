namespace Matching.Engine.Scoring;

public abstract class BayesFactor<R> : IBayesFactor
    where R : MatcherResult
{
    public Dictionary<string, double> Factors { get; set; } = [];

    public abstract double GetBayes(R result);

    public double GetScore(MatcherResult result)
    {
        if (result is R matchResult)
        {
            return GetBayes(matchResult);
        }

        return 0;
    }

    public abstract bool IsValid(out string[] requiredFields);
}
