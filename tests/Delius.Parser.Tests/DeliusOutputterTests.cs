using Delius.Parser.Core;

namespace Delius.Parser.Tests;

public class DeliusOutputterTests : IDisposable
{
    private readonly string _testDirectory;

    public DeliusOutputterTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"DeliusOutputterTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
    }

    [Fact]
    public async Task WriteAsync_CreatesFileWithCorrectName()
    {
        // Arrange
        var outputter = new DeliusOutputter();
        outputter.SetOffenderId(12345);
        outputter.StartOutput("Header", _testDirectory);

        // Act
        await outputter.WriteAsync("TestValue");
        await outputter.EndLine();
        outputter.Finish();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Header.txt");
        Assert.True(File.Exists(outputFile));
    }

    [Fact]
    public async Task WriteAsync_WithOffenderId_PrependsOffenderIdToLine()
    {
        // Arrange
        var outputter = new DeliusOutputter();
        outputter.SetOffenderId(12345);
        outputter.StartOutput("Header", _testDirectory);

        // Act
        await outputter.WriteAsync("Field1");
        await outputter.WriteAsync("Field2");
        await outputter.EndLine();
        outputter.Finish();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Header.txt");
        var content = await File.ReadAllTextAsync(outputFile);
        Assert.StartsWith("12345|", content);
    }

    [Fact]
    public async Task WriteAsync_UsesPipeDelimiter()
    {
        // Arrange
        var outputter = new DeliusOutputter();
        outputter.SetOffenderId(12345);
        outputter.StartOutput("Header", _testDirectory);

        // Act
        await outputter.WriteAsync("Field1");
        await outputter.WriteAsync("Field2");
        await outputter.WriteAsync("Field3");
        await outputter.EndLine();
        outputter.Finish();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Header.txt");
        var content = await File.ReadAllTextAsync(outputFile);
        Assert.Contains("|Field1|Field2|Field3", content);
    }

    [Fact]
    public async Task WriteAsync_WithMultipleLines_WritesAllLines()
    {
        // Arrange
        var outputter = new DeliusOutputter();
        
        // Act - Write first line
        outputter.SetOffenderId(11111);
        outputter.StartOutput("Header", _testDirectory);
        await outputter.WriteAsync("Value1");
        await outputter.EndLine();
        
        // Write second line
        outputter.SetOffenderId(22222);
        outputter.StartOutput("Header", _testDirectory);
        await outputter.WriteAsync("Value2");
        await outputter.EndLine();
        
        outputter.Finish();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Header.txt");
        var lines = await File.ReadAllLinesAsync(outputFile);
        Assert.Equal(2, lines.Length);
        Assert.Contains("11111", lines[0]);
        Assert.Contains("22222", lines[1]);
    }

    [Fact]
    public async Task WriteAsync_WithNullValue_WritesEmptyField()
    {
        // Arrange
        var outputter = new DeliusOutputter();
        outputter.SetOffenderId(12345);
        outputter.StartOutput("Header", _testDirectory);

        // Act
        await outputter.WriteAsync(null!);
        await outputter.WriteAsync("Field2");
        await outputter.EndLine();
        outputter.Finish();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Header.txt");
        var content = await File.ReadAllTextAsync(outputFile);
        Assert.Contains("12345||Field2", content);
    }

    [Fact]
    public void SetOffenderId_UpdatesOffenderId()
    {
        // Arrange
        var outputter = new DeliusOutputter();

        // Act
        outputter.SetOffenderId(99999);
        outputter.StartOutput("Header", _testDirectory);
        outputter.Write("Test");
        outputter.Finish();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Header.txt");
        var content = File.ReadAllText(outputFile);
        Assert.StartsWith("99999|", content);
    }

    [Fact]
    public async Task Finish_ClosesAllWriters()
    {
        // Arrange
        var outputter = new DeliusOutputter();
        outputter.SetOffenderId(12345);
        outputter.StartOutput("Header", _testDirectory);
        await outputter.WriteAsync("Test");
        await outputter.EndLine();

        // Act
        outputter.Finish();

        // Assert - File should be readable after Finish()
        var outputFile = Path.Combine(_testDirectory, "Header.txt");
        var content = await File.ReadAllTextAsync(outputFile);
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task WriteAsync_MultipleOutputs_CreatesSeparateFiles()
    {
        // Arrange
        var outputter = new DeliusOutputter();
        outputter.SetOffenderId(12345);

        // Act
        outputter.StartOutput("Header", _testDirectory);
        await outputter.WriteAsync("HeaderData");
        await outputter.EndLine();

        outputter.Finish();

        // Assert
        var headerFile = Path.Combine(_testDirectory, "Header.txt");
        Assert.True(File.Exists(headerFile));
    }

    [Fact]
    public void Write_WithLong_WritesValueAsString()
    {
        // Arrange
        var outputter = new DeliusOutputter();
        outputter.SetOffenderId(12345);
        outputter.StartOutput("Header", _testDirectory);

        // Act
        outputter.Write(67890L);
        outputter.Finish();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Header.txt");
        var content = File.ReadAllText(outputFile);
        Assert.Contains("67890", content);
    }

    [Fact]
    public void Write_WithDateTime_WritesFormattedDate()
    {
        // Arrange
        var outputter = new DeliusOutputter();
        outputter.SetOffenderId(12345);
        outputter.StartOutput("Header", _testDirectory);
        var testDate = new DateTime(2024, 11, 27);

        // Act
        outputter.Write(testDate);
        outputter.Finish();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Header.txt");
        var content = File.ReadAllText(outputFile);
        Assert.Contains("2024", content);
    }

    [Fact]
    public void Write_WithDefaultDateTime_WritesEmptyField()
    {
        // Arrange
        var outputter = new DeliusOutputter();
        outputter.SetOffenderId(12345);
        outputter.StartOutput("Header", _testDirectory);

        // Act
        outputter.Write(default(DateTime));
        outputter.Write("NextField");
        outputter.Finish();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Header.txt");
        var content = File.ReadAllText(outputFile);
        Assert.Contains("||NextField", content);
    }

    [Fact]
    public async Task WriteAsync_UsesCrlfLineEndings()
    {
        // Arrange
        var outputter = new DeliusOutputter();

        // Act - Write first line
        outputter.SetOffenderId(11111);
        outputter.StartOutput("Header", _testDirectory);
        await outputter.WriteAsync("Value1");
        await outputter.EndLine();
        
        // Write second line
        outputter.SetOffenderId(22222);
        outputter.StartOutput("Header", _testDirectory);
        await outputter.WriteAsync("Value2");
        await outputter.EndLine();
        
        outputter.Finish();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Header.txt");
        var fileContent = await File.ReadAllTextAsync(outputFile);
        
        Assert.Contains("\r\n", fileContent);
        Assert.DoesNotContain("\n", fileContent.Replace("\r\n", string.Empty));
        Assert.DoesNotContain("\r", fileContent.Replace("\r\n", string.Empty));
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }
    }
}
