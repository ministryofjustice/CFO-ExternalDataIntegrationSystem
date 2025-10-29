using Matching.Core.Matchers.Results;

namespace Matching.Engine.Scoring.Bayes;

[Bayes("Date")]
public class DateBayesFactor : BayesFactor<DateMatcherResult>
{
    public override double GetBayes(DateMatcherResult result)
    {
        List<double> bayes = [Factors["Different"]];

        if (result.SameYear && result.SameMonth)
        {
            bayes.Add(Factors["SameYearAndMonth"]);
        }

        if (result.SameMonth && result.SameDay)
        {
            bayes.Add(Factors["SameMonthAndDay"]);
        }

        if (result.SameYear && result.SameDay)
        {
            bayes.Add(Factors["SameYearAndDay"]);
        }

        if (result.SameYear && result.DayAndMonthTransposed)
        {
            bayes.Add(Factors["SameYearAndDayMonthTransposed"]);
        }

        return bayes.Max();
    }

    public override bool IsValid(out string[] requiredFields)
    {
        requiredFields = [
            "SameYearAndMonth",
            "SameMonthAndDay",
            "SameYearAndDay",
            "SameYearAndDayMonthTransposed",
            "Different"
        ];

        return requiredFields
            .All(Factors.ContainsKey);
    }
}
