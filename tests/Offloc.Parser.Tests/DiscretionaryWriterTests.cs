using Offloc.Parser.Services.TrimmerContext;
using Offloc.Parser.Services.TrimmerContext.SecondaryContexts;
using Offloc.Parser.Writers.DiscretionaryWriters;

namespace Offloc.Parser.Tests;

public class DiscretionaryWriterTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly DateTimeFieldContext _dateTimeContext;

    public DiscretionaryWriterTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"DiscretionaryWriterTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
        _dateTimeContext = new DateTimeFieldContext([]);
    }

    [Fact]
    public async Task WriteAsync_WithIncludeId_PrependsNomsNumber()
    {
        // Arrange
        var outputFile = Path.Combine(_testDirectory, "WithId.txt");
        var context = new DiscretionaryWriterContext
        {
            TableName = "TestTable",
            RelevantFields = [0, 1, 2],
            IncludeId = true
        };
        var writer = new DiscretionaryWriter(outputFile, context, _dateTimeContext);
        var contents = new[] { "Field0", "Field1", "Field2" };

        // Act
        await writer.WriteAsync("A1234BC", contents);
        writer.Dispose();

        // Assert
        var lines = await File.ReadAllLinesAsync(outputFile);
        Assert.Single(lines);
        Assert.StartsWith("A1234BC|", lines[0]);
        Assert.Equal("A1234BC|Field0|Field1|Field2", lines[0]);
    }

    [Fact]
    public async Task WriteAsync_WithoutIncludeId_OmitsNomsNumber()
    {
        // Arrange
        var outputFile = Path.Combine(_testDirectory, "WithoutId.txt");
        var context = new DiscretionaryWriterContext
        {
            TableName = "TestTable",
            RelevantFields = [0, 1],
            IncludeId = false
        };
        var writer = new DiscretionaryWriter(outputFile, context, _dateTimeContext);
        var contents = new[] { "Field0", "Field1" };

        // Act
        await writer.WriteAsync("A1234BC", contents);
        writer.Dispose();

        // Assert
        var lines = await File.ReadAllLinesAsync(outputFile);
        Assert.Single(lines);
        Assert.Equal("|Field0|Field1", lines[0]); // Starts with empty string + pipe
    }

    [Fact]
    public async Task WriteAsync_ExtractsOnlyRelevantFields()
    {
        // Arrange
        var outputFile = Path.Combine(_testDirectory, "Selective.txt");
        var context = new DiscretionaryWriterContext
        {
            TableName = "TestTable",
            RelevantFields = [1, 3, 5], // Skip 0, 2, 4
            IncludeId = true
        };
        var writer = new DiscretionaryWriter(outputFile, context, _dateTimeContext);
        var contents = new[] { "Skip0", "Keep1", "Skip2", "Keep3", "Skip4", "Keep5" };

        // Act
        await writer.WriteAsync("A1234BC", contents);
        writer.Dispose();

        // Assert
        var lines = await File.ReadAllLinesAsync(outputFile);
        Assert.Single(lines);
        Assert.Equal("A1234BC|Keep1|Keep3|Keep5", lines[0]);
    }

    [Fact]
    public async Task WriteAsync_WithAllEmptyFields_SkipsLine()
    {
        // Arrange
        var outputFile = Path.Combine(_testDirectory, "AllEmpty.txt");
        var context = new DiscretionaryWriterContext
        {
            TableName = "TestTable",
            RelevantFields = [0, 1, 2],
            IncludeId = true
        };
        var writer = new DiscretionaryWriter(outputFile, context, _dateTimeContext);
        var contents = new[] { "", "", "" };

        // Act
        await writer.WriteAsync("A1234BC", contents);
        writer.Dispose();

        // Assert
        var lines = await File.ReadAllLinesAsync(outputFile);
        Assert.Empty(lines); // No line should be written
    }

    [Fact]
    public async Task WriteAsync_WithOneNonEmptyField_WritesLine()
    {
        // Arrange
        var outputFile = Path.Combine(_testDirectory, "OneNonEmpty.txt");
        var context = new DiscretionaryWriterContext
        {
            TableName = "TestTable",
            RelevantFields = [0, 1, 2],
            IncludeId = true
        };
        var writer = new DiscretionaryWriter(outputFile, context, _dateTimeContext);
        var contents = new[] { "", "HasValue", "" };

        // Act
        await writer.WriteAsync("A1234BC", contents);
        writer.Dispose();

        // Assert
        var lines = await File.ReadAllLinesAsync(outputFile);
        Assert.Single(lines);
        Assert.Equal("A1234BC||HasValue|", lines[0]);
    }

    [Fact]
    public async Task WriteAsync_TrimsQuotesFromFields()
    {
        // Arrange
        var outputFile = Path.Combine(_testDirectory, "Quotes.txt");
        var context = new DiscretionaryWriterContext
        {
            TableName = "TestTable",
            RelevantFields = [0, 1],
            IncludeId = true
        };
        var writer = new DiscretionaryWriter(outputFile, context, _dateTimeContext);
        var contents = new[] { "\"QuotedValue\"", "\"AnotherQuoted\"" };

        // Act
        await writer.WriteAsync("A1234BC", contents);
        writer.Dispose();

        // Assert
        var lines = await File.ReadAllLinesAsync(outputFile);
        Assert.Single(lines);
        Assert.Equal("A1234BC|QuotedValue|AnotherQuoted", lines[0]);
    }

    [Fact]
    public async Task WriteAsync_WithMixedEmptyAndNonEmpty_WritesLine()
    {
        // Arrange
        var outputFile = Path.Combine(_testDirectory, "Mixed.txt");
        var context = new DiscretionaryWriterContext
        {
            TableName = "Assessments",
            RelevantFields = [0, 1, 2, 3],
            IncludeId = true
        };
        var writer = new DiscretionaryWriter(outputFile, context, _dateTimeContext);
        var contents = new[] { "Category A", "", "01/01/2024", "" };

        // Act
        await writer.WriteAsync("A1234BC", contents);
        writer.Dispose();

        // Assert
        var lines = await File.ReadAllLinesAsync(outputFile);
        Assert.Single(lines);
        Assert.Equal("A1234BC|Category A||01/01/2024|", lines[0]);
    }

    [Fact]
    public async Task WriteAsync_WithMultipleRecords_AppendsToFile()
    {
        // Arrange
        var outputFile = Path.Combine(_testDirectory, "Multiple.txt");
        var context = new DiscretionaryWriterContext
        {
            TableName = "Bookings",
            RelevantFields = [0, 1],
            IncludeId = true
        };
        var writer = new DiscretionaryWriter(outputFile, context, _dateTimeContext);

        // Act
        await writer.WriteAsync("A1111AA", new[] { "Booking1", "01/01/2024" });
        await writer.WriteAsync("B2222BB", new[] { "Booking2", "02/02/2024" });
        await writer.WriteAsync("C3333CC", new[] { "Booking3", "03/03/2024" });
        writer.Dispose();

        // Assert
        var lines = await File.ReadAllLinesAsync(outputFile);
        Assert.Equal(3, lines.Length);
        Assert.Equal("A1111AA|Booking1|01/01/2024", lines[0]);
        Assert.Equal("B2222BB|Booking2|02/02/2024", lines[1]);
        Assert.Equal("C3333CC|Booking3|03/03/2024", lines[2]);
    }

    [Fact]
    public async Task WriteAsync_UsesCrlfLineEndings()
    {
        // Arrange
        var outputFile = Path.Combine(_testDirectory, "CRLF.txt");
        var context = new DiscretionaryWriterContext
        {
            TableName = "TestTable",
            RelevantFields = [0],
            IncludeId = true
        };
        var writer = new DiscretionaryWriter(outputFile, context, _dateTimeContext);

        // Act
        await writer.WriteAsync("A1111AA", new[] { "Line1" });
        await writer.WriteAsync("B2222BB", new[] { "Line2" });
        writer.Dispose();

        // Assert
        var fileContent = await File.ReadAllTextAsync(outputFile);
        Assert.Contains("\r\n", fileContent);
        Assert.DoesNotContain("\n", fileContent.Replace("\r\n", string.Empty));
    }

    [Fact]
    public async Task WriteAsync_WithComplexFieldIndexes_ExtractsCorrectly()
    {
        // Arrange
        var outputFile = Path.Combine(_testDirectory, "Complex.txt");
        var context = new DiscretionaryWriterContext
        {
            TableName = "SentenceInformation",
            RelevantFields = [5, 10, 15, 20], // Non-sequential indices
            IncludeId = true
        };
        var writer = new DiscretionaryWriter(outputFile, context, _dateTimeContext);
        var contents = new string[25];
        for (int i = 0; i < 25; i++)
        {
            contents[i] = $"Field{i}";
        }

        // Act
        await writer.WriteAsync("A1234BC", contents);
        writer.Dispose();

        // Assert
        var lines = await File.ReadAllLinesAsync(outputFile);
        Assert.Single(lines);
        Assert.Equal("A1234BC|Field5|Field10|Field15|Field20", lines[0]);
    }

    [Fact]
    public async Task WriteAsync_WithWhitespaceFields_WritesLineWithWhitespace()
    {
        // Arrange
        var outputFile = Path.Combine(_testDirectory, "Whitespace.txt");
        var context = new DiscretionaryWriterContext
        {
            TableName = "TestTable",
            RelevantFields = [0, 1, 2],
            IncludeId = true
        };
        var writer = new DiscretionaryWriter(outputFile, context, _dateTimeContext);
        var contents = new[] { "  ", "  ", "  " }; // All whitespace

        // Act
        await writer.WriteAsync("A1234BC", contents);
        writer.Dispose();

        // Assert
        var lines = await File.ReadAllLinesAsync(outputFile);
        Assert.Single(lines);
        Assert.Equal("A1234BC|  |  |  ", lines[0]);
    }

    [Fact]
    public async Task WriteAsync_WithSingleField_WritesCorrectly()
    {
        // Arrange
        var outputFile = Path.Combine(_testDirectory, "SingleField.txt");
        var context = new DiscretionaryWriterContext
        {
            TableName = "Identifiers",
            RelevantFields = [0], // Single field
            IncludeId = true
        };
        var writer = new DiscretionaryWriter(outputFile, context, _dateTimeContext);
        var contents = new[] { "CRO12345" };

        // Act
        await writer.WriteAsync("A1234BC", contents);
        writer.Dispose();

        // Assert
        var lines = await File.ReadAllLinesAsync(outputFile);
        Assert.Single(lines);
        Assert.Equal("A1234BC|CRO12345", lines[0]);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            try
            {
                Directory.Delete(_testDirectory, recursive: true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
