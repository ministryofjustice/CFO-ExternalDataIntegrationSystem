using Matching.Core.Matchers;
using Matching.Core.Matchers.Results;

namespace Matching.Engine.Tests;

public class DateMatcherTests
{
    private readonly DateMatcher _matcher;

    public DateMatcherTests()
    {
        _matcher = new DateMatcher();
    }

    [Fact]
    public void Match_IdenticalDates_ReturnsAllTrue()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15);

        // Act
        var result = _matcher.Match(date, date) as DateMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.SameDay);
        Assert.True(result.SameMonth);
        Assert.True(result.SameYear);
        Assert.False(result.DayAndMonthTransposed);
    }

    [Fact]
    public void Match_SameDayDifferentTime_ReturnsAllTrue()
    {
        // Arrange
        var source = new DateTime(2024, 1, 15, 10, 30, 0);
        var target = new DateTime(2024, 1, 15, 14, 45, 0);

        // Act
        var result = _matcher.Match(source, target) as DateMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.SameDay);
        Assert.True(result.SameMonth);
        Assert.True(result.SameYear);
    }

    [Fact]
    public void Match_DayAndMonthTransposed_DetectsTransposition()
    {
        // Arrange
        var source = new DateTime(2024, 1, 12); // Jan 12
        var target = new DateTime(2024, 12, 1); // Dec 1

        // Act
        var result = _matcher.Match(source, target) as DateMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.False(result.SameDay);
        Assert.False(result.SameMonth);
        Assert.True(result.SameYear);
        Assert.True(result.DayAndMonthTransposed);
    }

    [Theory]
    [InlineData(2024, 1, 15, 2024, 1, 15, true, true, true)]
    [InlineData(2024, 1, 15, 2024, 1, 20, false, true, true)]
    [InlineData(2024, 1, 15, 2024, 2, 15, true, false, true)] // Same day number (15), different months
    [InlineData(2024, 1, 15, 2023, 1, 15, true, true, false)]
    public void Match_VariousDates_ReturnsExpectedResults(
        int y1, int m1, int d1, 
        int y2, int m2, int d2,
        bool expectSameDay, bool expectSameMonth, bool expectSameYear)
    {
        // Arrange
        var source = new DateTime(y1, m1, d1);
        var target = new DateTime(y2, m2, d2);

        // Act
        var result = _matcher.Match(source, target) as DateMatcherResult;

        // Assert
        Assert.NotNull(result);
        // Note: SameDay means same day NUMBER (e.g., both 15th), not same calendar date
        Assert.Equal(expectSameDay, result.SameDay);
        Assert.Equal(expectSameMonth, result.SameMonth);
        Assert.Equal(expectSameYear, result.SameYear);
    }
}
