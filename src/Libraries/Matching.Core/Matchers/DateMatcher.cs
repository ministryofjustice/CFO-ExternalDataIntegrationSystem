using Matching.Core.Attributes;
using Matching.Core.Matchers.Results;
using Matching.Core.Utils;

namespace Matching.Core.Matchers;

[Matcher("Date")]
public class DateMatcher : Matcher<DateTime, DateMatcherResult>
{
    protected override DateMatcherResult Match(DateTime source, DateTime target)
    {
        if (source == target)
            return DateMatcherResult.Identical(source, target);

        var result = new DateMatcherResult()
        {
            Source = source,
            Target = target,
            SameDay = source.HasSameDay(target),
            SameMonth = source.HasSameMonth(target),
            SameYear = source.HasSameYear(target),
            DayAndMonthTransposed = source.HasDayAndMonthTransposed(target)
        };

        return result;
    }

}
