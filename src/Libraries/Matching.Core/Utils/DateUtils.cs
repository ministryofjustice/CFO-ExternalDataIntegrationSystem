namespace Matching.Core.Utils;

internal static class DateUtils
{
    internal static bool HasSameDay(this DateTime source, DateTime target) => source.Day.Equals(target.Day);
    internal static bool HasSameMonth(this DateTime source, DateTime target) => source.Month.Equals(target.Month);
    internal static bool HasSameYear(this DateTime source, DateTime target) => source.Year.Equals(target.Year);
    internal static bool HasDayAndMonthTransposed(this DateTime source, DateTime target)
    {
        return !source.HasSameDay(target) && source.Day.Equals(target.Month) && target.Day.Equals(source.Month);
    }

}
