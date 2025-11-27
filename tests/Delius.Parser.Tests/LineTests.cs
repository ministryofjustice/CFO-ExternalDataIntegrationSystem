using Delius.Parser.Configuration.Models;

namespace Delius.Parser.Tests;

public class LineTests
{
    [Fact]
    public void Split_WithValidLine_ReturnsCorrectFieldCount()
    {
        // Arrange
        var line = CreateTestLine();
        var input = "HEFULL1234~~~~~~1900010112300012345~~~~~";

        // Act
        var result = line.Split(input);

        // Assert
        Assert.Equal(4, result.Length);
    }

    [Fact]
    public void Split_WithValidHeaderLine_ParsesAllFields()
    {
        // Arrange
        var line = CreateTestLine();
        var input = "HEFULL1234~~~~~~1900010112300012345~~~~~";

        // Act
        var result = line.Split(input);

        // Assert
        Assert.Equal("FULL", result[0]); // FileType
        Assert.Equal("1234", result[1]); // Sequence
        Assert.Contains("1900", result[2]); // RunDate
        Assert.Equal("12345", result[3]); // SectionCount
    }

    [Fact]
    public void Split_RemovesPipeCharactersFromFields()
    {
        // Arrange
        var line = new Line
        {
            Id = 1,
            Name = "TestLine",
            Length = 12,
            StartingKey = "TE",
            OutputToFile = true,
            Fields = new List<Field>
            {
                new Field { Name = "Field1", StartingPoint = 0, Length = 6, Type = FieldType.String },
                new Field { Name = "Field2", StartingPoint = 6, Length = 6, Type = FieldType.String }
            }
        };
        
        var input = "Test|1Field2";

        // Act
        var result = line.Split(input);

        // Assert
        Assert.Equal("Test 1", result[0]);
        Assert.Equal("Field2", result[1]);
    }

    [Fact]
    public void Split_OrdersFieldsByStartingPoint()
    {
        // Arrange
        var line = new Line
        {
            Id = 1,
            Name = "TestLine",
            Length = 15,
            StartingKey = "TE",
            OutputToFile = true,
            OutputToLog = false,
            Fields = new List<Field>
            {
                new Field { Name = "Field2", StartingPoint = 5, Length = 5, Type = FieldType.String },
                new Field { Name = "Field1", StartingPoint = 0, Length = 5, Type = FieldType.String },
                new Field { Name = "Field3", StartingPoint = 10, Length = 5, Type = FieldType.String }
            }
        };

        var input = "FirstSecndThird";

        // Act
        var result = line.Split(input);

        // Assert
        Assert.Equal("First", result[0]);
        Assert.Equal("Secnd", result[1]);
        Assert.Equal("Third", result[2]);
    }

    [Fact]
    public void Split_WithOffenderId_ParsesCorrectly()
    {
        // Arrange
        var line = new Line
        {
            Id = 7,
            Name = "Offenders",
            Length = 50,
            StartingKey = "LD",
            OutputToFile = true,
            OutputToLog = false,
            Fields = new List<Field>
            {
                new Field { Name = "OffenderId", StartingPoint = 2, Length = 10, Type = FieldType.Long },
                new Field { Name = "FirstName", StartingPoint = 12, Length = 20, Type = FieldType.String }
            }
        };

        var input = "LD1234567890John~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~";

        // Act
        var result = line.Split(input);

        // Assert
        Assert.Equal("1234567890", result[0]);
        Assert.Equal("John", result[1]);
    }

    [Fact]
    public void Split_ThrowsException_WhenFieldParsingFails()
    {
        // Arrange
        var line = CreateTestLine();
        var input = "SHORT"; // Too short for the expected line

        // Act & Assert
        var exception = Assert.Throws<ApplicationException>(() => line.Split(input));
        Assert.Contains("Error reading field", exception.Message);
    }

    [Theory]
    [InlineData("HEFULL")]
    [InlineData("HEDIFF")]
    public void Split_HandlesHeaderVariations(string headerType)
    {
        // Arrange
        var line = new Line
        {
            Id = 1,
            Name = "Header",
            Length = 6,
            StartingKey = "HE",
            OutputToFile = true,
            Fields = new List<Field>
            {
                new Field { Name = "FileType", StartingPoint = 2, Length = 4, Type = FieldType.String }
            }
        };

        // Act
        var result = line.Split(headerType);

        // Assert
        Assert.Single(result);
        Assert.True(result[0] == "FULL" || result[0] == "DIFF");
    }

    private Line CreateTestLine()
    {
        return new Line
        {
            Id = 4,
            Name = "Header",
            Length = 40,
            StartingKey = "HE",
            OutputToFile = true,
            OutputToLog = true,
            AllowSplit = false,
            Fields = new List<Field>
            {
                new Field
                {
                    Id = 7,
                    LineId = 4,
                    Name = "FileType",
                    Length = 4,
                    StartingPoint = 2,
                    Type = FieldType.String
                },
                new Field
                {
                    Id = 8,
                    LineId = 4,
                    Name = "Sequence",
                    Length = 10,
                    StartingPoint = 6,
                    Type = FieldType.Long
                },
                new Field
                {
                    Id = 9,
                    LineId = 4,
                    Name = "RunDate",
                    Length = 14,
                    StartingPoint = 16,
                    Type = FieldType.LongDate
                },
                new Field
                {
                    Id = 10,
                    LineId = 4,
                    Name = "SectionCount",
                    Length = 10,
                    StartingPoint = 30,
                    Type = FieldType.Long
                }
            }
        };
    }

    private Line CreateSimpleTestLine()
    {
        return new Line
        {
            Id = 1,
            Name = "TestLine",
            Length = 23,
            StartingKey = "TE",
            OutputToFile = true,
            Fields = new List<Field>
            {
                new Field { Name = "Field1", StartingPoint = 0, Length = 11, Type = FieldType.String },
                new Field { Name = "Field2", StartingPoint = 11, Length = 7, Type = FieldType.String }
            }
        };
    }
}
