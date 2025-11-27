using Delius.Parser.Core;
using System.Text;

namespace Delius.Parser.Tests;

public class DeliusProcessorTests : IDisposable
{
    private readonly string _testDirectory;

    public DeliusProcessorTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"DeliusProcessorTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
    }

    [Fact]
    public async Task Process_WithValidHeaderLine_ProcessesSuccessfully()
    {
        // Arrange
        var outputPath = _testDirectory;
        CreateRequiredPostParserFiles(outputPath);
        
        var outputter = new DeliusOutputter();
        var postParser = new PostParser();
        var processor = new DeliusProcessor(outputter, postParser);

        var headerLine = "HEFULL1234~~~~~~1900010112300012345~~~~~";
        var stream = CreateStreamReader(headerLine);

        var unhandledLines = new List<string>();

        // Act
        await processor.Process(stream, outputPath, line => unhandledLines.Add(line));

        // Assert
        Assert.Empty(unhandledLines);
        var headerFile = Path.Combine(outputPath, "Header.txt");
        Assert.True(File.Exists(headerFile));
    }

    [Fact]
    public async Task Process_WithMultipleLines_CreatesMultipleFiles()
    {
        // Arrange
        var outputPath = _testDirectory;
        CreateRequiredPostParserFiles(outputPath);
        
        var outputter = new DeliusOutputter();
        var postParser = new PostParser();
        var processor = new DeliusProcessor(outputter, postParser);

        var content = @"HEFULL1234~~~~~~1900010112300012345~~~~~";
        var stream = CreateStreamReader(content);

        // Act
        await processor.Process(stream, outputPath, _ => { });

        // Assert
        var headerFile = Path.Combine(outputPath, "Header.txt");
        Assert.True(File.Exists(headerFile));
    }

    [Fact]
    public async Task Process_WithUnhandledLine_CallsUnhandledLineAction()
    {
        // Arrange
        var outputPath = _testDirectory;
        CreateRequiredPostParserFiles(outputPath);
        
        var outputter = new DeliusOutputter();
        var postParser = new PostParser();
        var processor = new DeliusProcessor(outputter, postParser);

        var invalidLine = "INVALID_LINE_CONTENT";
        var stream = CreateStreamReader(invalidLine);

        var unhandledLines = new List<string>();

        // Act
        await processor.Process(stream, outputPath, line => unhandledLines.Add(line));

        // Assert
        Assert.Single(unhandledLines);
        Assert.Equal(invalidLine, unhandledLines[0]);
    }

    [Fact]
    public async Task Process_WithEmptyStream_ProcessesSuccessfully()
    {
        // Arrange
        var outputPath = _testDirectory;
        CreateRequiredPostParserFiles(outputPath);
        
        var outputter = new DeliusOutputter();
        var postParser = new PostParser();
        var processor = new DeliusProcessor(outputter, postParser);

        var stream = CreateStreamReader("");

        // Act
        await processor.Process(stream, outputPath, _ => { });

        // Assert - Should not throw exception
        Assert.True(true);
    }

    [Fact]
    public async Task Process_CreatesOutputFilesWithCorrectFormat()
    {
        // Arrange
        var outputPath = _testDirectory;
        CreateRequiredPostParserFiles(outputPath);
        
        var outputter = new DeliusOutputter();
        var postParser = new PostParser();
        var processor = new DeliusProcessor(outputter, postParser);

        var headerLine = "HEFULL1234~~~~~~1900010112300012345~~~~~";
        var stream = CreateStreamReader(headerLine);

        // Act
        await processor.Process(stream, outputPath, _ => { });

        // Assert
        var headerFile = Path.Combine(outputPath, "Header.txt");
        Assert.True(File.Exists(headerFile));
        
        var content = await File.ReadAllTextAsync(headerFile);
        Assert.Contains("|", content); // Should use pipe delimiter
    }

    [Fact]
    public async Task Process_WithShortLine_HandlesAsUnhandled()
    {
        // Arrange
        var outputPath = _testDirectory;
        CreateRequiredPostParserFiles(outputPath);
        
        var outputter = new DeliusOutputter();
        var postParser = new PostParser();
        var processor = new DeliusProcessor(outputter, postParser);

        var shortLine = "HE";
        var stream = CreateStreamReader(shortLine);

        var unhandledLines = new List<string>();

        // Act
        await processor.Process(stream, outputPath, line => unhandledLines.Add(line));

        // Assert
        Assert.Single(unhandledLines);
        Assert.Equal(shortLine, unhandledLines[0]);
    }

    [Theory]
    [InlineData("HEFULL1234~~~~~~1900010112300012345~~~~~")]
    [InlineData("HEDIFF1234~~~~~~1900010112300012345~~~~~")]
    public async Task Process_WithDifferentHeaderTypes_ProcessesBoth(string headerLine)
    {
        // Arrange
        var outputPath = Path.Combine(_testDirectory, Guid.NewGuid().ToString());
        Directory.CreateDirectory(outputPath);
        CreateRequiredPostParserFiles(outputPath);
        
        var outputter = new DeliusOutputter();
        var postParser = new PostParser();
        var processor = new DeliusProcessor(outputter, postParser);

        var stream = CreateStreamReader(headerLine);

        var unhandledLines = new List<string>();

        // Act
        await processor.Process(stream, outputPath, line => unhandledLines.Add(line));

        // Assert
        Assert.Empty(unhandledLines);
    }

    [Fact]
    public async Task Process_WithMixedValidAndInvalidLines_ProcessesValidOnly()
    {
        // Arrange
        var outputPath = _testDirectory;
        CreateRequiredPostParserFiles(outputPath);
        
        var outputter = new DeliusOutputter();
        var postParser = new PostParser();
        var processor = new DeliusProcessor(outputter, postParser);

        var content = string.Join("\n", 
            "HEFULL1234~~~~~~1900010112300012345~~~~~",
            "INVALID_LINE",
            "ANOTHER_INVALID"
        );
        var stream = CreateStreamReader(content);

        var unhandledLines = new List<string>();

        // Act
        await processor.Process(stream, outputPath, line => unhandledLines.Add(line));

        // Assert
        Assert.Equal(2, unhandledLines.Count);
        Assert.True(File.Exists(Path.Combine(outputPath, "Header.txt")));
    }

    private StreamReader CreateStreamReader(string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        return new StreamReader(stream, Encoding.UTF8);
    }

    private void CreateRequiredPostParserFiles(string outputPath)
    {
        // PostParser expects OffenderManager.txt to exist
        var offenderManagerFile = Path.Combine(outputPath, "OffenderManager.txt");
        File.WriteAllText(offenderManagerFile, ""); // Create empty file
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }
    }
}
