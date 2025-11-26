namespace Matching.Core.Matchers.Results;

public class DateMatcherResult : MatcherResult, IDateMatcherResult
{
    public bool SameDay { get; set; }
    public bool SameMonth { get; set; }
    public bool SameYear { get; set; }
    public bool DayAndMonthTransposed { get; set; }

    public static DateMatcherResult Identical(DateTime source, DateTime target) => new()
    {
        Source = source,
        Target = target,
        SameDay = true,
        SameMonth = true,
        SameYear = true
    };

}

public interface IDateMatcherResult
{
    public bool SameDay { get; set; }
    public bool SameMonth { get; set; }
    public bool SameYear { get; set; }
    public bool DayAndMonthTransposed { get; set; }
}
