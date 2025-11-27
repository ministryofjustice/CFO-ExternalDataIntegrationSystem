using Offloc.Parser.Writers;

namespace Offloc.Parser.Tests;

public class CustomWriterTests : IDisposable
{
    private readonly string _testDirectory;

    public CustomWriterTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"CustomWriterTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
    }

    [Fact]
    public async Task WriteAsync_WithMultipleCustomRecords_WritesAllCustomRecords()
    {
        // Arrange
        var writer = new CustomWriter(_testDirectory);

        // Act
        await writer.WriteAsync("12345", ["Field1", "Field2", "Field3"]);
        await writer.WriteAsync("67890", ["FieldA", "FieldB", "FieldC"]);
        writer.Dispose();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Custom.txt");
        string[] lines = await File.ReadAllLinesAsync(outputFile);

        Assert.Equal(2, lines.Length);
        Assert.Equal("Field1|Field2|Field3", lines[0]);
        Assert.Equal("FieldA|FieldB|FieldC", lines[1]);
    }

    [Fact]
    public async Task WriteAsync_UsesCrlfLineEndings()
    {
        // Arrange
        var writer = new CustomWriter(_testDirectory);

        // Act
        await writer.WriteAsync("12345", ["Line1"]);
        await writer.WriteAsync("67890", ["Line2"]);
        writer.Dispose();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Custom.txt");
        string fileContent = await File.ReadAllTextAsync(outputFile);

        Assert.Contains("\r\n", fileContent);
        Assert.DoesNotContain("\n", fileContent.Replace("\r\n", string.Empty));
        Assert.DoesNotContain("\r", fileContent.Replace("\r\n", string.Empty));
    }

    [Fact]
    public async Task WriteAsync_WithNoRecords_CreatesFile()
    {
        // Arrange
        var writer = new CustomWriter(_testDirectory);

        // Act
        await writer.WriteAsync("12345", []);
        writer.Dispose();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Custom.txt");
        string[] lines = await File.ReadAllLinesAsync(outputFile);

        Assert.Single(lines);
        Assert.True(File.Exists(outputFile));
        Assert.Equal(string.Empty, lines[0]);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }
    }
}

class CustomWriter(string path) : WriterBase($"{path}/Custom.txt"), IWriter
{
    public Task WriteAsync(string NOMSNumber, string[] contents) => StreamWriter!.WriteLineAsync(string.Join('|', contents)); 
}