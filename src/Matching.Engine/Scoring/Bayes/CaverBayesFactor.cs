using Matching.Core.Matchers.Results;

namespace Matching.Engine.Scoring.Bayes;

[Bayes("Caver")]
public class CaverBayesFactor : BayesFactor<CaverMatcherResult>
{
    public override double GetBayes(CaverMatcherResult result)
    {
        List<double> bayes = [Factors["Different"]];

        if (result.IsPhoneticallySimilar && result.IsWhole())
        {
            bayes.Add(Factors["Similar"]);
        }

        return bayes.Max();
    }

    public override bool IsValid(out string[] requiredFields)
    {
        requiredFields = [
            "Different",
            "Similar"
        ];

        return requiredFields
            .All(Factors.ContainsKey);
    }
}
