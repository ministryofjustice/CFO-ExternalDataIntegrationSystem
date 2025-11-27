using Matching.Core.Matchers;
using Matching.Core.Matchers.Results;

namespace Matching.Engine.Tests;

public class EqualityMatcherTests
{
    private readonly EqualityMatcher _matcher;

    public EqualityMatcherTests()
    {
        _matcher = new EqualityMatcher();
    }

    [Fact]
    public void Match_IdenticalStrings_ReturnsTrue()
    {
        // Arrange
        var source = "hello";
        var target = "hello";

        // Act
        var result = _matcher.Match(source, target) as EqualityMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Equal);
        Assert.Equal(source, result.Source);
        Assert.Equal(target, result.Target);
    }

    [Fact]
    public void Match_DifferentStrings_ReturnsFalse()
    {
        // Arrange
        var source = "hello";
        var target = "world";

        // Act
        var result = _matcher.Match(source, target) as EqualityMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Equal);
    }

    [Fact]
    public void Match_CaseInsensitive_ReturnsTrue()
    {
        // Arrange
        var source = "HELLO";
        var target = "hello";

        // Act
        var result = _matcher.Match(source, target) as EqualityMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Equal);
    }

    [Theory]
    [InlineData("hello", "hello", true)]
    [InlineData("HELLO", "hello", true)]
    [InlineData("Hello", "HELLO", true)]
    [InlineData("hello", "world", false)]
    [InlineData("", "", false)] // Empty strings return false (length must be > 0)
    [InlineData("test", "TEST", true)]
    public void Match_VariousStrings_ReturnsExpectedEquality(
        string source, string target, bool expectedEqual)
    {
        // Act
        var result = _matcher.Match(source, target) as EqualityMatcherResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedEqual, result.Equal);
    }

    [Fact]
    public void Match_BothNull_ReturnsFalse()
    {
        // Act
        var result = _matcher.Match(null, null) as EqualityMatcherResult;

        // Assert
        Assert.NotNull(result);
        // StringUtils.Equal returns false for nulls (requires length > 0)
        Assert.False(result.Equal);
    }

    [Fact]
    public void Match_WhitespaceHandling_NoTrimming()
    {
        // Arrange
        var source = "  hello  ";
        var target = "hello";

        // Act
        var result = _matcher.Match(source, target) as EqualityMatcherResult;

        // Assert
        Assert.NotNull(result);
        // StringUtils.Equal does not trim
        Assert.False(result.Equal);
    }
}
