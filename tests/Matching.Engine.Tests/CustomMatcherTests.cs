using Matching.Core;
using Matching.Engine.Scoring;

namespace Matching.Engine.Tests;

public class CustomMatcherTests
{
    [Fact]
    public void IsWhole_WhenNotMissingInSource_ReturnsTrue()
    {
        // Arrange
        var matcherResult = new CustomMatcherResult 
        { 
            Source = "value",
            Target = null
        };

        // Act
        var result = matcherResult.IsWhole();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsWhole_WhenNotMissingInTarget_ReturnsTrue()
    {
        // Arrange
        var matcherResult = new CustomMatcherResult 
        { 
            Source = null,
            Target = "value"
        };

        // Act
        var result = matcherResult.IsWhole();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsWhole_WhenMissingInBoth_ReturnsFalse()
    {
        // Arrange
        var matcherResult = new CustomMatcherResult 
        { 
            Source = null,
            Target = null
        };

        // Act
        var result = matcherResult.IsWhole();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsWhole_WhenPresentInBoth_ReturnsTrue()
    {
        // Arrange
        var matcherResult = new CustomMatcherResult 
        { 
            Source = "value1",
            Target = "value2"
        };

        // Act
        var result = matcherResult.IsWhole();

        // Assert
        Assert.True(result);
    }

    private class CustomMatcherResult : MatcherResult
    {
    }
}
