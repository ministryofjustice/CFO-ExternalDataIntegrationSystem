using Matching.Core.Matchers.Results;

namespace Matching.Engine.Scoring.Bayes;

[Bayes("Levenshtein")]
public class LevenshteinBayesFactor : BayesFactor<LevenshteinMatcherResult>
{
    public override double GetBayes(LevenshteinMatcherResult result)
    {
        List<double> bayes = [Factors["Different"]];

        double maxEditDistance = Factors["MaxEditDistance"];

        int distance = result.LevenshteinEditDistance;

        if (distance <= maxEditDistance && result.IsWhole())
        {
            bayes.Add(Factors["Similar"]);
        }

        return bayes.Max();
    }

    public override bool IsValid(out string[] requiredFields)
    {
        requiredFields = [
            "Different",
            "Similar",
            "MaxEditDistance"
        ];

        return requiredFields
            .All(Factors.ContainsKey);
    }


}
