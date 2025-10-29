using Matching.Core.Matchers.Results;

namespace Matching.Engine.Scoring.Bayes;

[Bayes("Equality")]
public class EqualityBayesFactor : BayesFactor<EqualityMatcherResult>
{
    public override double GetBayes(EqualityMatcherResult result)
    {
        List<double> bayes = [Factors["Different"]];

        if (result.Equal && result.IsWhole())
        {
            bayes.Add(Factors["Identical"]);
        }

        if (result.MissingInBoth)
        {
            bayes.Add(Factors["MissingFromBoth"]);
        }

        if (result.MissingInSource)
        {
            bayes.Add(Factors["MissingFromSource"]);
        }

        if (result.MissingInTarget)
        {
            bayes.Add(Factors["MissingFromTarget"]);
        }

        return bayes.Max();
    }

    public override bool IsValid(out string[] requiredFields)
    {
        requiredFields = [
            "Different",
            "Identical",
            "MissingFromBoth",
            "MissingFromSource",
            "MissingFromTarget"
        ];

        return requiredFields
            .All(Factors.ContainsKey);
    }

}
