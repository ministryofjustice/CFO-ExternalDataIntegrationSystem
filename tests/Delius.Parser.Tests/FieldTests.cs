using Delius.Parser.Configuration.Models;

namespace Delius.Parser.Tests;

public class FieldTests
{
    [Theory]
    [InlineData("TestValue~~~~~~~", 0, 10, "TestValue")]
    [InlineData("~~TestValue~~~~", 2, 9, "TestValue")]
    [InlineData("PrefixTestSuffix", 6, 4, "Test")]
    public void Parse_StringField_RemovesTildesAndExtractsCorrectly(string input, int startingPoint, int length, string expected)
    {
        // Arrange
        var field = new Field
        {
            Name = "TestField",
            Type = FieldType.String,
            StartingPoint = startingPoint,
            Length = length
        };

        // Act
        var result = field.Parse(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("12345~~~~~", 0, 10, "12345")]
    [InlineData("~~~~~~~~~~", 0, 10, "0")]
    [InlineData("  123  ~~~", 2, 6, "123  ")]
    public void Parse_LongField_HandlesTildesAndWhitespace(string input, int startingPoint, int length, string expected)
    {
        // Arrange
        var field = new Field
        {
            Name = "TestField",
            Type = FieldType.Long,
            StartingPoint = startingPoint,
            Length = length
        };

        // Act
        var result = field.Parse(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("20241127123456", 0, 14, "27/11/2024")]
    [InlineData("20250101000000", 0, 14, "01/01/2025")]
    [InlineData("~~~~~~~~~~~~~~", 0, 14, "")]
    public void Parse_LongDateField_FormatsDateCorrectly(string input, int startingPoint, int length, string expectedDate)
    {
        // Arrange
        var field = new Field
        {
            Name = "TestField",
            Type = FieldType.LongDate,
            StartingPoint = startingPoint,
            Length = length
        };

        // Act
        var result = field.Parse(input);

        // Assert
        if (string.IsNullOrEmpty(expectedDate))
        {
            Assert.Empty(result);
        }
        else
        {
            Assert.Contains(expectedDate, result);
        }
    }

    [Theory]
    [InlineData("20241127", 0, 8, "27/11/2024")]
    [InlineData("20250101", 0, 8, "01/01/2025")]
    [InlineData("~~~~~~~~", 0, 8, "")]
    public void Parse_ShortDateField_FormatsDateCorrectly(string input, int startingPoint, int length, string expectedDate)
    {
        // Arrange
        var field = new Field
        {
            Name = "TestField",
            Type = FieldType.ShortDate,
            StartingPoint = startingPoint,
            Length = length
        };

        // Act
        var result = field.Parse(input);

        // Assert
        if (string.IsNullOrEmpty(expectedDate))
        {
            Assert.Empty(result);
        }
        else
        {
            Assert.Contains(expectedDate, result);
        }
    }

    [Fact]
    public void Parse_StringField_ExtractsValue()
    {
        // Arrange
        var field = new Field
        {
            Name = "TestField",
            Type = FieldType.String,
            StartingPoint = 0,
            Length = 10
        };

        // Act
        var result = field.Parse("Test|Value");

        // Assert
        Assert.Equal("Test|Value", result);
    }

    [Fact]
    public void Parse_LongField_EmptyString_ReturnsZero()
    {
        // Arrange
        var field = new Field
        {
            Name = "TestField",
            Type = FieldType.Long,
            StartingPoint = 0,
            Length = 10
        };

        // Act
        var result = field.Parse("          ");

        // Assert
        Assert.Equal("0", result);
    }
}
