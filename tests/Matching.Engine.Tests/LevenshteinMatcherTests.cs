using Matching.Core.Matchers;
using Matching.Core.Matchers.Results;

namespace Matching.Engine.Tests;

public class LevenshteinMatcherTests
{
    private readonly LevenshteinMatcher _matcher;

    public LevenshteinMatcherTests()
    {
        _matcher = new LevenshteinMatcher();
    }

    [Fact]
    public void Match_IdenticalStrings_ReturnsZeroDistance()
    {
        // Arrange
        var source = "hello";
        var target = "hello";

        // Act
        var result = _matcher.Match(source, target) as LevenshteinMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.LevenshteinEditDistance);
        Assert.Equal(source, result.Source);
        Assert.Equal(target, result.Target);
    }

    [Fact]
    public void Match_CompletelyDifferentStrings_ReturnsMaxDistance()
    {
        // Arrange
        var source = "abc";
        var target = "xyz";

        // Act
        var result = _matcher.Match(source, target) as LevenshteinMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.LevenshteinEditDistance);
    }

    [Fact]
    public void Match_OneCharacterDifference_ReturnsOne()
    {
        // Arrange
        var source = "kitten";
        var target = "sitten";

        // Act
        var result = _matcher.Match(source, target) as LevenshteinMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.LevenshteinEditDistance);
    }

    [Theory]
    [InlineData("kitten", "sitting", 3)]
    [InlineData("saturday", "sunday", 3)]
    [InlineData("book", "back", 2)]
    [InlineData("", "abc", 3)]
    [InlineData("abc", "", 3)]
    public void Match_VariousStrings_ReturnsCorrectDistance(
        string source, string target, int expectedDistance)
    {
        // Act
        var result = _matcher.Match(source, target) as LevenshteinMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDistance, result.LevenshteinEditDistance);
    }

    [Fact]
    public void Match_NullStrings_ReturnsMissingInBoth()
    {
        // Act
        var result = _matcher.Match(null, null) as LevenshteinMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.MissingInSource);
        Assert.True(result.MissingInTarget);
        Assert.True(result.MissingInBoth);
    }

    [Fact]
    public void Match_SourceNull_ReturnsMissingInSource()
    {
        // Act
        var result = _matcher.Match(null, "target") as LevenshteinMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.MissingInSource);
        Assert.False(result.MissingInTarget);
    }

    [Fact]
    public void Match_TargetNull_ReturnsMissingInTarget()
    {
        // Act
        var result = _matcher.Match("source", null) as LevenshteinMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.False(result.MissingInSource);
        Assert.True(result.MissingInTarget);
    }

    [Fact]
    public void Match_CaseInsensitive_TreatsAsEqual()
    {
        // Arrange
        var source = "HELLO";
        var target = "hello";

        // Act
        var result = _matcher.Match(source, target) as LevenshteinMatcherResult;

        // Assert
        Assert.NotNull(result);
        // Levenshtein should be case-insensitive due to StringUtils.Equal
        Assert.Equal(0, result.LevenshteinEditDistance);
    }
}
