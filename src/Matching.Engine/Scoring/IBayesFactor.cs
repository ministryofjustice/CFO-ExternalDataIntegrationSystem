using Matching.Engine.Scoring.Validators;

namespace Matching.Engine.Scoring;

public interface IBayesFactor : IBayesValidator
{
    Dictionary<string, double> Factors { get; set; }

    double GetScore(MatcherResult result);
}
